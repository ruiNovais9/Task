
using TaskManager.Contracts;
using TaskManager.Entities;

namespace TaskManager.BL.Mappings
{
    public static class LayerToBusinessLogic
    {
        public static List<Contracts.Project> Map(List<Entities.Project> listDomainProjects)
        {
            var listOfProjects = new List<Contracts.Project>();

            if (listDomainProjects == null || listDomainProjects.Count == 0)
            {
                return listOfProjects;
            }

            foreach (var project in listDomainProjects)
            {
                Contracts.Project newProject = SetContractsProjects(project);
                listOfProjects.Add(newProject);
            }

            return listOfProjects;
        }

        private static Contracts.Project SetContractsProjects(Entities.Project project)
        {
            var newProject = new Contracts.Project();
            newProject.Id = project.Id;
            newProject.Name = project.Name;
            newProject.DeadLine = project.DeadLine.HasValue ? project.DeadLine : DateTime.UtcNow.AddMonths(12);
            newProject.ProjectIsCompleted = project.ProjectIsCompleted;
            newProject.DeveloperId = project.DeveloperId;
            newProject.TimeSpend = project.TimeSpend;
            return newProject;
        }

        public static Contracts.Project Map(Entities.Project domainProjects)
        {
            return SetContractsProjects(domainProjects);
        }

    }
}
