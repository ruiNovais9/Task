using Microsoft.EntityFrameworkCore;
using TaskManager.Entities;

namespace TaskManager;

public interface IApiContext
{
    DbSet<Project> Projects { get; }
    List<Project> GetProjects();
    Project? GetProjectById(long id);
    List<Project> GetProjects(int pageIndex, int maxProjects, bool? isDeadLineOrderByAsc, out bool haveMoreProject);
    Project InsertProject(Project project);
    Project UpdateProject(Project project);
}

public class ApiContext : DbContext, IApiContext
{
    public ApiContext(DbContextOptions<ApiContext> options)
        : base(options)
    {
        
    }

    public DbSet<Project> Projects { get; set; }

    public List<Project> GetProjects()
    {
        return Projects.ToList();
    }

    public List<Project> GetProjects(int pageIndex, int maxProjects, bool? isDeadLineOrderByAsc, out bool haveMoreProject)
    {
        if (!Projects.Any())
        {
            haveMoreProject = false;
            return new List<Project>();
        }

        int numberPage = pageIndex + 1;
        haveMoreProject = numberPage * maxProjects < Projects.Count();
        if (isDeadLineOrderByAsc.HasValue)
        {
            if (isDeadLineOrderByAsc.Value)
            {
                return Projects.OrderBy(x => x.DeadLine).Skip(pageIndex * maxProjects).Take(maxProjects).ToList();
            }
            else
            {
                return Projects.OrderByDescending(x => x.DeadLine).Skip(pageIndex * maxProjects).Take(maxProjects).ToList();
            }
        }
        
        return Projects.Skip(pageIndex * maxProjects).Take(maxProjects).ToList();
    }

    public Project? GetProjectById(long id)
    {
        return Projects.FirstOrDefault(x => x.Id == id);
    }

    public Project InsertProject(Project project)
    {
        Projects.Add(project);
        SaveChanges();
        return project;
    }

    public Project UpdateProject(Project project)
    {
        Projects.Update(project);
        SaveChanges();
        return project;
    }
}