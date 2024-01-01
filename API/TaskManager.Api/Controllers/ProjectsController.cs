using Microsoft.AspNetCore.Mvc;
using TaskManager.BL;
using TaskManager.Contracts.Responses;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectsLogic _projectsLogic;

    public ProjectsController(IProjectsLogic projectsLogic)
    {
        _projectsLogic = projectsLogic;
    }

    [HttpGet]
    [Route("hello-world")]
    public string HelloWorld()
    {
        return "Hello Back!";
    }

    [HttpGet]
    public IActionResult Get()
    {
        return BuildActionResponse(_projectsLogic.GetProjects());
    }

    [HttpGet("{isDeadLineByAsc}/{pageIndex}/{numberProjects}")]
    public IActionResult Get(bool isDeadLineByAsc, int pageIndex, int numberProjects)
    {
        return BuildActionResponse(_projectsLogic.GetOrderByDeadLineProjects(isDeadLineByAsc, pageIndex, numberProjects));
    }

    [HttpGet("{pageIndex}/{numberProjects}")]
    public IActionResult Get(int pageIndex, int numberProjects)
    {
        return BuildActionResponse(_projectsLogic.GetProjectsByPagination(pageIndex, numberProjects));
    }

    [HttpPost]
    public IActionResult Insert([FromBody] Contracts.Requests.ProjectRequest project)
    {
        return BuildActionResponse(_projectsLogic.InsertProject(project));
    }
    [HttpPut]
    public IActionResult Update([FromBody] Contracts.Requests.UpdateProjectRequest project)
    {
        return BuildActionResponse(_projectsLogic.UpdateProject(project));
    }

    [HttpPut("{id}/status/{status}")]
    public IActionResult UpdateStatus(long id, bool status)
    {
        return BuildActionResponse(_projectsLogic.UpdateStatusProject(id, status));
    }

    [HttpPut("{id}/timeSpend/{timeUsed}")]
    public IActionResult UpdateTimeSpend(long id, long timeUsed)
    {
        return BuildActionResponse(_projectsLogic.UpdateTimeUsedOnProject(id, timeUsed));
    }

    private IActionResult BuildActionResponse(ProjectResponse response)
    {
        if (response == null || !string.IsNullOrWhiteSpace(response.Error?.Message)
            || !response.IsSucess)
        {
            return BadRequest(response);
        }
        else
        {
            return Ok(response);
        }
    }

}