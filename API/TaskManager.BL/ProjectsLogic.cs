using TaskManager.BL.Mappings;
using TaskManager.Contracts.Requests;
using TaskManager.Contracts.Responses;
using TaskManager.Entities;

namespace TaskManager.BL
{
    public class ProjectsLogic : IProjectsLogic
    {
        private readonly IApiContext _context;

        public ProjectsLogic(IApiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Get first pagination of Projects.
        /// </summary>
        /// <returns></returns>
        public PaginationProjectResponse GetProjects()
        {
            return GetProjectsByPagination(0);
        }
        /// <summary>
        /// Order by DeadLine, can be by Asc or Desc
        /// </summary>
        /// <param name="isDeadLineOrderByAsc">
        /// If true order by Asc DeadLine, otherwise is by Descending.
        /// </param>
        /// <param name="pageIndex"></param>
        /// <param name="numberProjects"></param>
        /// <returns></returns>
        public PaginationProjectResponse GetOrderByDeadLineProjects(bool isDeadLineOrderByAsc, int pageIndex, int numberProjects)
        {
            var response = new PaginationProjectResponse();
            try
            {
                response = GetProjectsByPagination(pageIndex, numberProjects, isDeadLineOrderByAsc);
            }
            catch (Exception ex)
            {
                response.Error = new Error();
                response.Error.Message = ex.Message;
            }

            return response;
        }
        /// <summary>
        /// Get project by pagination.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="numberProjects"></param>
        /// <returns></returns>
        public PaginationProjectResponse GetProjectsByPagination(int pageIndex, int numberProjects = 50, bool? isDeadLineOrderByAsc = null)
        {
            var response = new PaginationProjectResponse();
            try
            {
                if (numberProjects > 50)
                {
                    numberProjects = 50;
                }
                else if (numberProjects < 0)
                {
                    numberProjects = 1;
                }

                List<Contracts.Project> listProjects = LayerToBusinessLogic.Map(_context.GetProjects(pageIndex, numberProjects, isDeadLineOrderByAsc, out bool haveMoreProjects));

                response.HaveMoreProjects = haveMoreProjects;
                response.IsSucess = true;
                response.Projects = listProjects;
            }
            catch (Exception ex)
            {
                response.Error = new Error();
                response.Error.Message = ex.Message;
            }

            return response;
        }
        /// <summary>
        /// Insert new project on database.
        /// </summary>
        /// <param name="projectRequest"></param>
        /// <returns>
        /// The new project inserted on database.
        /// </returns>
        public ProjectResponse InsertProject(ProjectRequest projectRequest)
        {
            var response = new ProjectResponse();
            try
            {
                ArgumentNullException.ThrowIfNull(projectRequest);
                ValidateRequest(projectRequest.Name);

                projectRequest.DeadLine = !projectRequest.DeadLine.HasValue ||
                                            projectRequest.DeadLine == DateTime.MinValue ||
                                            projectRequest.DeadLine.Value < DateTime.UtcNow ?
                                            DateTime.UtcNow.AddMonths(12) :
                                            projectRequest.DeadLine;

                var bussinessProject = BusinessToLayer.MapNewProject(projectRequest);

                Contracts.Project project = LayerToBusinessLogic.Map(_context.InsertProject(bussinessProject));
                response.IsSucess = true;
                response.Projects = new List<Contracts.Project>
                {
                    project
                };
            }
            catch (ArgumentNullException ex)
            {
                response.Error = new Error();
                response.Error.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.Error = new Error();
                response.Error.Message = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Update specific project.
        /// </summary>
        /// <param name="updateProjectRequest"></param>
        /// <returns>
        /// Return the project updated.
        /// </returns>
        public ProjectResponse UpdateProject(UpdateProjectRequest updateProjectRequest)
        {
            var projectResponse = new ProjectResponse();
            try
            {
                ArgumentNullException.ThrowIfNull(updateProjectRequest);
                ValidateRequest(updateProjectRequest.Name);

                Project? projectFromContext = _context.GetProjectById(updateProjectRequest.Id);

                if (projectFromContext != null)
                {
                    if (!updateProjectRequest.DeadLine.HasValue ||
                        updateProjectRequest.DeadLine == DateTime.MinValue ||
                        updateProjectRequest.DeadLine.Value < DateTime.UtcNow)
                    {
                        updateProjectRequest.DeadLine = projectFromContext.DeadLine;
                    }

                    updateProjectRequest.TimeSpend = projectFromContext.TimeSpend;

                    projectFromContext = BusinessToLayer.UpdateMap(updateProjectRequest, projectFromContext);

                    return BuildProjectResponse(projectResponse, projectFromContext);
                }
                else
                {
                    return NotFindProjectResponse(projectResponse);
                }
            }
            catch (ArgumentNullException ex)
            {
                projectResponse.Error = new Error();
                projectResponse.Error.Message = ex.Message;
            }
            catch (Exception ex)
            {
                projectResponse.Error = new Error();
                projectResponse.Error.Message = ex.Message;
            }

            return projectResponse;
        }


        /// <summary>
        /// Update status of one project.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns>
        /// Return the project updated.
        /// </returns>
        public ProjectResponse UpdateStatusProject(long id, bool status)
        {
            var projectResponse = new ProjectResponse();
            try
            {
                Project? projectFromContext = _context.GetProjectById(id);

                if (projectFromContext != null)
                {
                    projectFromContext.ProjectIsCompleted = status;
                    return BuildProjectResponse(projectResponse, projectFromContext);
                }
                else
                {
                    return NotFindProjectResponse(projectResponse);
                }
            }
            catch (Exception ex)
            {
                projectResponse.Error = new Error();
                projectResponse.Error.Message = ex.Message;
            }

            return projectResponse;
        }

        /// <summary>
        /// Update time used on project.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="timeUsed"></param>
        /// <returns>
        /// Return the project updated.
        /// </returns>
        public ProjectResponse UpdateTimeUsedOnProject(long id, long timeUsed)
        {
            var response = new ProjectResponse();
            try
            {
                if (timeUsed < 30)
                {
                    response.IsSucess = false;
                    response.Error = new Error()
                    {
                        Message = "The time spend on project is less than 30. Need to be more than 30 minutes."
                    };
                    return response;
                }

                Project? projectFromContext = _context.GetProjectById(id);
                if (projectFromContext != null)
                {
                    projectFromContext.TimeSpend = projectFromContext.TimeSpend + timeUsed;
                    return BuildProjectResponse(response, projectFromContext);
                }
                else
                {
                    return NotFindProjectResponse(response);
                }
            }
            catch (Exception ex)
            {
                response.Error = new Error();
                response.Error.Message = ex.Message;
            }

            return response;
        }

        private static ProjectResponse NotFindProjectResponse(ProjectResponse response)
        {
            response.IsSucess = false;
            response.Error = new Error
            {
                Message = "Project don't found on Database."
            };
            return response;
        }

        private ProjectResponse BuildProjectResponse(ProjectResponse response, Project projectFromContext)
        {
            Contracts.Project project = LayerToBusinessLogic.Map(_context.UpdateProject(projectFromContext));
            response.IsSucess = true;
            response.Projects = new List<Contracts.Project>
            {
                project
            };
            return response;
        }

        #region Validations
        private void ValidateRequest(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
            {
                throw new ArgumentNullException("Name of project is null or empty.");
            }
        }
        #endregion
    }
}
