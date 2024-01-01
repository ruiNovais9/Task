
namespace TaskManager.BL
{
    public interface IProjectsLogic
    {
        Contracts.Responses.PaginationProjectResponse GetProjects();
        Contracts.Responses.PaginationProjectResponse GetOrderByDeadLineProjects(bool isDeadLineOrderByAsc, int pageIndex, int numberProjects);
        Contracts.Responses.PaginationProjectResponse GetProjectsByPagination(int pageIndex, int numberProjects = 50, bool? isDeadLineOrderByAsc = null);
        Contracts.Responses.ProjectResponse InsertProject(Contracts.Requests.ProjectRequest project);
        Contracts.Responses.ProjectResponse UpdateProject(Contracts.Requests.UpdateProjectRequest project);
        Contracts.Responses.ProjectResponse UpdateStatusProject(long id, bool status);
        Contracts.Responses.ProjectResponse UpdateTimeUsedOnProject(long id, long timeUsed);
    }
}
