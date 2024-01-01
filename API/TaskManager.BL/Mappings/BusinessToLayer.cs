
using TaskManager.Contracts;

namespace TaskManager.BL.Mappings
{
    public static class BusinessToLayer
    {
        public static List<Entities.Project> Map(List<Contracts.Project> listBusinessProjects)
        {
            var listOfDomainProjects = new List<Entities.Project>();

            if (listBusinessProjects == null || listBusinessProjects.Count == 0)
            {
                return listOfDomainProjects;
            }

            foreach (Contracts.Project project in listBusinessProjects)
            {
                Entities.Project newProject = CreateProjectEntity(project);
                listOfDomainProjects.Add(newProject);
            }

            return listOfDomainProjects;
        }

        public static Entities.Project Map(Contracts.Project projectRequest)
        {
            return CreateProjectEntity(projectRequest);
        }

        public static Entities.Project MapNewProject(Contracts.Requests.ProjectRequest projectRequest)
        {
            return CreateProjectEntity(new Project
            {
                Name = projectRequest.Name,
                DeadLine = projectRequest.DeadLine,
                ProjectIsCompleted = false,
                TimeSpend = 0,
                DeveloperId = 1
            });
        }

        public static Entities.Project UpdateMap(Contracts.Project projectRequest, Entities.Project projectFromContext)
        {
            projectFromContext.DeadLine = projectRequest.DeadLine;
            projectFromContext.Name = projectRequest.Name;
            projectFromContext.ProjectIsCompleted = projectRequest.ProjectIsCompleted;
            projectFromContext.TimeSpend = projectRequest.TimeSpend;
            projectFromContext.DeveloperId = projectRequest.DeveloperId;
            return projectFromContext;
        }


        private static Entities.Project CreateProjectEntity(Contracts.Project project)
        {
            return new Entities.Project()
            {
                Name = project.Name,
                Id = project.Id,
                DeadLine = project.DeadLine,
                DeveloperId = project.DeveloperId,
                ProjectIsCompleted = project.ProjectIsCompleted,
                TimeSpend = project.TimeSpend
            };
        }
    }
}
