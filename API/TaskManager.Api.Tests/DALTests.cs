using Microsoft.EntityFrameworkCore;
using System;
using TaskManager.Entities;

namespace TaskManager.Api.Tests
{
    public class DALTests : IDisposable
    {
        private List<Entities.Project> _projectsDb = new List<Entities.Project>
        {
            new Entities.Project
            {
                Name = "teste",
                DeadLine = DateTime.UtcNow.AddDays(30),
                Id = 1,
                ProjectIsCompleted = false,
                TimeSpend = 60
            },
            new Entities.Project
            {
                Name = "teste2",
                DeadLine = DateTime.UtcNow.AddDays(60),
                Id = 2,
                ProjectIsCompleted = false,
                TimeSpend = 150
            },
            new Entities.Project
            {
                Name = "teste3",
                DeadLine = DateTime.UtcNow.AddDays(90),
                Id = 3,
                ProjectIsCompleted = false,
                TimeSpend = 180
            },
            new Entities.Project
            {
                Name = "teste4",
                DeadLine = DateTime.UtcNow.AddDays(20),
                Id = 4,
                ProjectIsCompleted = true,
                TimeSpend = 40
            },
        };

        public void Dispose()
        {
            _apiContext.Database.EnsureDeleted();
            _apiContext.Dispose();
        }

        private ApiContext _apiContext { get; set; }
        public DALTests()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                        .UseInMemoryDatabase("Project")
                        .Options;

            _apiContext = new ApiContext(options);

            foreach (var projects in _projectsDb)
            {
                _apiContext.Projects.Add(projects);
            }
            _apiContext.SaveChanges();
        }

        [Fact]
        public void GetProject_Project_AllProjectsBack()
        {
            List<Project> getProjects = _apiContext.GetProjects();

            Assert.Equal(4, getProjects.Count);

            foreach (var project in getProjects)
            {
                var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
                Assert.True(findProjectFromBd != null);
                Assert.Equal(findProjectFromBd.Name, project.Name);
                Assert.Equal(findProjectFromBd.Id, project.Id);
                Assert.Equal(findProjectFromBd.TimeSpend, project.TimeSpend);
                Assert.Equal(findProjectFromBd.DeadLine, project.DeadLine);
                Assert.Equal(findProjectFromBd.ProjectIsCompleted, project.ProjectIsCompleted);
            }
           
        }

        [Fact]
        public void GetProjectById_Project_GetOneSpecificProjectBack()
        {
            Project? getProjects = _apiContext.GetProjectById(3);

            Assert.True(getProjects != null);

            Assert.Equal(getProjects.Name, "teste3");
            Assert.Equal(getProjects.Id, 3);
            Assert.Equal(getProjects.TimeSpend, 180);
            Assert.Equal(getProjects.DeadLine.Value.ToString("yyyy-MM-dd"), DateTime.UtcNow.AddDays(90).ToString("yyyy-MM-dd"));
            Assert.Equal(getProjects.ProjectIsCompleted, false);
        }

        [Fact]
        public void GetProjectById_Project_NoProjectFindBack()
        {
            Project? getProjects = _apiContext.GetProjectById(6);

            Assert.True(getProjects == null);
        }

        [Fact]
        public void InsertProject_Project_GetNewProjectBack()
        {
            Project insertProject = _apiContext.InsertProject(new Project 
            { 
                Name = "teste6",
                DeadLine = DateTime.UtcNow.AddDays(350),
                DeveloperId = 1,
                Id = 6,
                ProjectIsCompleted = false,
                TimeSpend = 250
            });

            Assert.True(_apiContext.Projects.Count() == 5);
            Assert.True(insertProject != null);

            Assert.Equal(insertProject.Name, "teste6");
            Assert.Equal(insertProject.Id, 6);
            Assert.Equal(insertProject.TimeSpend, 250);
            Assert.Equal(insertProject.DeadLine.Value.ToString("yyyy-MM-dd"), DateTime.UtcNow.AddDays(350).ToString("yyyy-MM-dd"));
            Assert.Equal(insertProject.ProjectIsCompleted, false);

            var findProjectFromBd = _apiContext.Projects.FirstOrDefault(x => x.Id == 6);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(findProjectFromBd.Name, insertProject.Name);
            Assert.Equal(findProjectFromBd.Id, insertProject.Id);
            Assert.Equal(findProjectFromBd.TimeSpend, insertProject.TimeSpend);
            Assert.Equal(findProjectFromBd.DeadLine.Value.ToString("yyyy-MM-dd"), insertProject.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(findProjectFromBd.ProjectIsCompleted, insertProject.ProjectIsCompleted);
        }

        [Fact]
        public void UpdateProject_Project_GetNewProjectBack()
        {
            var project3 = _projectsDb.FirstOrDefault(x => x.Id == 3);
            project3.TimeSpend = 350;
            project3.ProjectIsCompleted = true;
            Project updateProject = _apiContext.UpdateProject(project3);

            Assert.True(_apiContext.Projects.Count() == 4);
            Assert.True(updateProject != null);

            Assert.Equal(updateProject.Name, "teste3");
            Assert.Equal(updateProject.Id, 3);
            Assert.Equal(updateProject.TimeSpend, 350);
            Assert.Equal(updateProject.DeadLine.Value.ToString("yyyy-MM-dd"), DateTime.UtcNow.AddDays(90).ToString("yyyy-MM-dd"));
            Assert.Equal(updateProject.ProjectIsCompleted, true);

            var findProjectFromBd = _apiContext.Projects.FirstOrDefault(x => x.Id == 3);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(findProjectFromBd.Name, updateProject.Name);
            Assert.Equal(findProjectFromBd.Id, updateProject.Id);
            Assert.Equal(findProjectFromBd.TimeSpend, updateProject.TimeSpend);
            Assert.Equal(findProjectFromBd.DeadLine.Value.ToString("yyyy-MM-dd"), updateProject.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(findProjectFromBd.ProjectIsCompleted, updateProject.ProjectIsCompleted);
        }

        [Fact]
        public void GetProject_Project_AllProjectsOrderByDeadLineBack()
        {
            List<Project> getProjects = _apiContext.GetProjects(0, 25, true, out bool haveMoreProjects);

            Assert.Equal(4, getProjects.Count);
            Assert.True(!haveMoreProjects);
            Assert.True(getProjects[0].Id == 4);
            Assert.True(getProjects[3].Id == 3);
        }

        [Fact]
        public void GetProjects_Project_AllProjectsOrderByDeadLineDescBack()
        {
            List<Project> getProjects = _apiContext.GetProjects(0, 25, false, out bool haveMoreProjects);

            Assert.Equal(4, getProjects.Count);
            Assert.True(!haveMoreProjects);
            Assert.True(getProjects[0].Id == 3);
            Assert.True(getProjects[3].Id == 4);
        }

        [Fact]
        public void GetProjects_Project_ByPaginationNoOrderDescBack()
        {
            List<Project> getProjects = _apiContext.GetProjects(0, 25, null, out bool haveMoreProjects);

            Assert.Equal(4, getProjects.Count);
            Assert.True(!haveMoreProjects);
            Assert.True(getProjects[0].Id == 1);
            Assert.True(getProjects[3].Id == 4);
        }

        [Fact]
        public void GetProjects_Project_NoProjectsFoundBack()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                                    .UseInMemoryDatabase("Project_" + DateTime.UtcNow.ToFileTimeUtc())
                                    .Options;
            var apiContext = new ApiContext(options);
            List<Project> getProjects = apiContext.GetProjects(0, 25, null, out bool haveMoreProjects);

            Assert.True(getProjects != null && getProjects.Count == 0);
            Assert.True(!haveMoreProjects);
        }

        [Fact]
        public void GetProjects_Project_ByPaginationNoOrderGiveTwoProjectByPageDescBack()
        {
            List<Project> getProjects = _apiContext.GetProjects(0, 2, null, out bool haveMoreProjects);

            Assert.Equal(2, getProjects.Count);
            Assert.True(haveMoreProjects);
            Assert.True(getProjects[0].Id == 1);
            Assert.True(getProjects[1].Id == 2);

            getProjects = _apiContext.GetProjects(1, 2, null, out haveMoreProjects);

            Assert.Equal(2, getProjects.Count);
            Assert.True(!haveMoreProjects);
            Assert.True(getProjects[0].Id == 3);
            Assert.True(getProjects[1].Id == 4);
        }
    }
}
