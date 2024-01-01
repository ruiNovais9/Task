using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.BL;
using TaskManager.Contracts.Requests;
using TaskManager.Contracts.Responses;
using TaskManager.Entities;

namespace TaskManager.Api.Tests;

public class ProjectControllerTests
{
    private IProjectsLogic _projectsLogic { get; set; }
    private ProjectsController _projectsController { get; set; }
    private Mock<IApiContext> MockApiContext { get; set; }
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
    public ProjectControllerTests()
    {
        MockApiContext = new Mock<IApiContext>(MockBehavior.Default);
        _projectsLogic = new ProjectsLogic(MockApiContext.Object);
        _projectsController = new ProjectsController(_projectsLogic);
    }

    [Fact]
    public void HelloWorld_ShouldReply_HelloBack()
    {
        string actual = _projectsController.HelloWorld();

        Assert.Equal("Hello Back!", actual);
    }

    [Fact]
    public void GetProject_Project_AllProjectBack()
    {
        MockApiContext.Setup(x => x.GetProjects(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>(), out It.Ref<bool>.IsAny)).Returns(_projectsDb);

        IActionResult getProjects = _projectsController.Get();
        OkObjectResult? okResult = getProjects as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Equal(4, getResponse.Projects.Count);
        Assert.True(getResponse.Error == null);

        foreach (var project in getResponse.Projects)
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
    public void GetProjectsByPage_Project_AllProjectBack()
    {
        MockApiContext.Setup(x => x.GetProjects(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>(), out It.Ref<bool>.IsAny)).Returns((int pageIndex, int maxProjects, bool? isDeadLineOrderByAsc, out bool haveMoreProjects) =>
        {
            int numberPage = pageIndex + 1;
            haveMoreProjects = numberPage * maxProjects < _projectsDb.Count();
            return _projectsDb.Skip(pageIndex * maxProjects).Take(maxProjects).ToList();
        });

        bool isGetAllProjects = false;
        int expectedProjects = 4;
        int responseListTotal = 0;
        int index = 0;
        do
        {
            IActionResult getProject = _projectsController.Get(index, 2);
            OkObjectResult? okResult = getProject as OkObjectResult;
            index++;
            Assert.True(okResult != null);
            Assert.Equal(200, okResult.StatusCode);
            Assert.True(okResult.Value is ProjectResponse);

            var getResponse = okResult.Value as ProjectResponse;
            Assert.True(getResponse != null && getResponse.IsSucess);

            if (getResponse.Projects.Count == 0)
            {
                isGetAllProjects = true;
                break;
            }

            Assert.Equal(2, getResponse.Projects.Count);
            Assert.True(getResponse.Error == null);
            responseListTotal += getResponse.Projects.Count;
            foreach (var project in getResponse.Projects)
            {
                var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
                Assert.True(findProjectFromBd != null);
                Assert.Equal(findProjectFromBd.Name, project.Name);
                Assert.Equal(findProjectFromBd.Id, project.Id);
                Assert.Equal(findProjectFromBd.TimeSpend, project.TimeSpend);
                Assert.Equal(findProjectFromBd.DeadLine, project.DeadLine);
                Assert.Equal(findProjectFromBd.ProjectIsCompleted, project.ProjectIsCompleted);
            }
        } while (!isGetAllProjects);
        Assert.Equal(expectedProjects, responseListTotal);
    }

    [Fact]
    public void GetProjectsByPage_MaxPagesExceedProject_AllProjectBack()
    {
        MockApiContext.Setup(x => x.GetProjects(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>(), out It.Ref<bool>.IsAny)).Returns((int pageIndex, int maxProjects, bool? isDeadLineOrderByAsc, out bool haveMoreProjects) =>
        {
            int numberPage = pageIndex + 1;
            haveMoreProjects = numberPage * maxProjects < _projectsDb.Count();
            return _projectsDb.Skip(pageIndex * maxProjects).Take(maxProjects).ToList();
        });

        bool isGetAllProjects = false;
        int expectedProjects = 4;
        int responseListTotal = 0;
        int index = 0;
        do
        {
            IActionResult getProjects = _projectsController.Get(index, 55);
            OkObjectResult? okResult = getProjects as OkObjectResult;
            index++;
            Assert.True(okResult != null);
            Assert.Equal(200, okResult.StatusCode);
            Assert.True(okResult.Value is ProjectResponse);

            var getResponse = okResult.Value as ProjectResponse;
            Assert.True(getResponse != null && getResponse.IsSucess);

            if (getResponse.Projects.Count == 0)
            {
                isGetAllProjects = true;
                break;
            }

            Assert.Equal(4, getResponse.Projects.Count);
            Assert.True(getResponse.Error == null);
            responseListTotal += getResponse.Projects.Count;
            foreach (var project in getResponse.Projects)
            {
                var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
                Assert.True(findProjectFromBd != null);
                Assert.Equal(findProjectFromBd.Name, project.Name);
                Assert.Equal(findProjectFromBd.Id, project.Id);
                Assert.Equal(findProjectFromBd.TimeSpend, project.TimeSpend);
                Assert.Equal(findProjectFromBd.DeadLine, project.DeadLine);
                Assert.Equal(findProjectFromBd.ProjectIsCompleted, project.ProjectIsCompleted);
            }
        } while (!isGetAllProjects);
        Assert.Equal(expectedProjects, responseListTotal);
    }

    [Fact]
    public void GetProjectsByPage_LessThanZeroProjectsProject_AllProjectBack()
    {
        MockApiContext.Setup(x => x.GetProjects(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>(), out It.Ref<bool>.IsAny)).Returns((int pageIndex, int maxProjects, bool? isDeadLineOrderByAsc, out bool haveMoreProjects) =>
        {
            int numberPage = pageIndex + 1;
            haveMoreProjects = numberPage * maxProjects < _projectsDb.Count();
            return _projectsDb.Skip(pageIndex * maxProjects).Take(maxProjects).ToList();
        });

        bool isGetAllProjects = false;
        int expectedProjects = 4;
        int responseListTotal = 0;
        int index = 0;
        do
        {
            IActionResult getProjects = _projectsController.Get(index, -1);
            OkObjectResult? okResult = getProjects as OkObjectResult;
            index++;
            Assert.True(okResult != null);
            Assert.Equal(200, okResult.StatusCode);
            Assert.True(okResult.Value is ProjectResponse);

            var getResponse = okResult.Value as ProjectResponse;
            Assert.True(getResponse != null && getResponse.IsSucess);

            if (getResponse.Projects.Count == 0)
            {
                isGetAllProjects = true;
                break;
            }

            Assert.Single(getResponse.Projects);
            Assert.True(getResponse.Error == null);
            responseListTotal += getResponse.Projects.Count;
            foreach (var project in getResponse.Projects)
            {
                var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
                Assert.True(findProjectFromBd != null);
                Assert.Equal(findProjectFromBd.Name, project.Name);
                Assert.Equal(findProjectFromBd.Id, project.Id);
                Assert.Equal(findProjectFromBd.TimeSpend, project.TimeSpend);
                Assert.Equal(findProjectFromBd.DeadLine, project.DeadLine);
                Assert.Equal(findProjectFromBd.ProjectIsCompleted, project.ProjectIsCompleted);
            }
        } while (!isGetAllProjects);
        Assert.Equal(expectedProjects, responseListTotal);
    }

    [Fact]
    public void GetProjects_Project_OrderByDeadLineBack()
    {
        MockApiContext.Setup(x => x.GetProjects(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>(), out It.Ref<bool>.IsAny)).Returns((int pageIndex, int maxProjects, bool? isDeadLineOrderByAsc, out bool haveMoreProjects) =>
        {
            return ReturnIsDeadLineOrderByAsc(pageIndex, maxProjects, isDeadLineOrderByAsc, out haveMoreProjects);
        });


        IActionResult getProjects = _projectsController.Get(true, 0, 50);
        OkObjectResult? okResult = getProjects as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Equal(4, getResponse.Projects.Count);
        Assert.True(getResponse.Error == null);

        Assert.True(getResponse.Projects.First().Id == 4);
        Assert.True(getResponse.Projects.Last().Id == 3);

        foreach (var project in getResponse.Projects)
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

    private List<Project> ReturnIsDeadLineOrderByAsc(int pageIndex, int maxProjects, bool? isDeadLineOrderByAsc, out bool haveMoreProjects)
    {
        int numberPage = pageIndex + 1;
        haveMoreProjects = numberPage * maxProjects < _projectsDb.Count();

        if (isDeadLineOrderByAsc.HasValue)
        {
            if (isDeadLineOrderByAsc.Value)
            {
                return _projectsDb.OrderBy(x => x.DeadLine).Skip(pageIndex * maxProjects).Take(maxProjects).ToList();
            }
            else
            {
                return _projectsDb.OrderByDescending(x => x.DeadLine).Skip(pageIndex * maxProjects).Take(maxProjects).ToList();
            }
        }

        return _projectsDb.Skip(pageIndex * maxProjects).Take(maxProjects).ToList();
    }

    [Fact]
    public void GetProjectsByOrder_Project_OrderByDescDeadLineBack()
    {
        MockApiContext.Setup(x => x.GetProjects(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>(), out It.Ref<bool>.IsAny)).Returns((int pageIndex, int maxProjects, bool? isDeadLineOrderByAsc, out bool haveMoreProjects) =>
        {
            return ReturnIsDeadLineOrderByAsc(pageIndex, maxProjects, isDeadLineOrderByAsc, out haveMoreProjects);
        });

        IActionResult getProject = _projectsController.Get(false, 0, 50);
        OkObjectResult? okResult = getProject as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Equal(4, getResponse.Projects.Count);
        Assert.True(getResponse.Error == null);

        Assert.True(getResponse.Projects.First().Id == 3);
        Assert.True(getResponse.Projects.Last().Id == 4);

        foreach (var project in getResponse.Projects)
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
    public void GetProjectsByOrder_ExceptionProject_ExceptionExpectedBack()
    {
        MockApiContext.Setup(x => x.GetProjects(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>(), out It.Ref<bool>.IsAny)).Throws(new Exception("Something happen"));

        IActionResult getProject = _projectsController.Get(false, 0, 50);
        BadRequestObjectResult? badResult = getProject as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.Empty(getResponse.Projects);
        Assert.True(getResponse.Error != null);
        Assert.Equal("Something happen", getResponse.Error.Message);
    }

    [Fact]
    public void GetAllProjects_ExceptionProject_ExceptionExpectedBack()
    {
        MockApiContext.Setup(x => x.GetProjects(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool?>(), out It.Ref<bool>.IsAny)).Throws(new Exception("Something happen"));

        IActionResult getProjects = _projectsController.Get();
        BadRequestObjectResult? badResult = getProjects as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.Empty(getResponse.Projects);
        Assert.True(getResponse.Error != null);
        Assert.Equal("Something happen", getResponse.Error.Message);
    }

    [Fact]
    public void InsertNewProject_NewProject_NewCreatedProjectBack()
    {
        var projectRequest = new ProjectRequest
        {
            DeadLine = DateTime.UtcNow.AddDays(120),
            Name = "New project",
        };

        MockApiContext.Setup(x => x.InsertProject(It.IsAny<Entities.Project>()))
                                .Callback<Entities.Project>((project) =>
                                {
                                    var maxId = _projectsDb.Count > 0 ? _projectsDb.Max(p => p.Id) : 1;
                                    project.Id = maxId + 1;
                                    _projectsDb.Add(project);
                                })
                                .Returns((Entities.Project project) => project);

        IActionResult insertNewProject = _projectsController.Insert(projectRequest);
        OkObjectResult? okResult = insertNewProject as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Single(getResponse.Projects);
        Assert.True(getResponse.Error == null);
        Assert.Equal(5, _projectsDb.Count);

        foreach (var project in getResponse.Projects)
        {
            var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
            Assert.Equal(5, project.Id);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(findProjectFromBd.Name, project.Name);
            Assert.Equal("New project", project.Name);
            Assert.Equal(projectRequest.Name, project.Name);
            Assert.Equal(0, project.TimeSpend);
            Assert.Equal(findProjectFromBd.Id, project.Id);
            Assert.Equal(findProjectFromBd.TimeSpend, project.TimeSpend);
            Assert.Equal(projectRequest.DeadLine.Value.ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(findProjectFromBd.DeadLine, project.DeadLine);
            Assert.Equal(findProjectFromBd.ProjectIsCompleted, project.ProjectIsCompleted);
        }
    }

    [Fact]
    public void InsertNewProjectInvalidDeadline_NewProject_NewCreatedProjectWithDefaultDeadLineBack()
    {
        var projectRequest = new ProjectRequest
        {
            DeadLine = DateTime.UtcNow.AddDays(-120),
            Name = "New project",
        };

        MockApiContext.Setup(x => x.InsertProject(It.IsAny<Entities.Project>()))
                                .Callback<Entities.Project>((project) =>
                                {
                                    var maxId = _projectsDb.Count > 0 ? _projectsDb.Max(p => p.Id) : 1;
                                    project.Id = maxId + 1;
                                    _projectsDb.Add(project);
                                })
                                .Returns((Entities.Project project) => project);

        IActionResult insertNewProject = _projectsController.Insert(projectRequest);
        OkObjectResult? okResult = insertNewProject as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Single(getResponse.Projects);
        Assert.True(getResponse.Error == null);
        Assert.Equal(5, _projectsDb.Count);

        foreach (var project in getResponse.Projects)
        {
            var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
            Assert.Equal(5, project.Id);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(findProjectFromBd.Name, project.Name);
            Assert.Equal("New project", project.Name);
            Assert.Equal(projectRequest.Name, project.Name);
            Assert.Equal(0, project.TimeSpend);
            Assert.Equal(findProjectFromBd.Id, project.Id);
            Assert.Equal(findProjectFromBd.TimeSpend, project.TimeSpend);
            Assert.Equal(projectRequest.DeadLine.Value.ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(DateTime.UtcNow.AddMonths(12).ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(findProjectFromBd.DeadLine, project.DeadLine);
            Assert.Equal(findProjectFromBd.ProjectIsCompleted, project.ProjectIsCompleted);
        }
    }

    [Fact]
    public void InsertNewProjectDEfaultDateline_NewProject_NewCreatedProjectWithDefaultDeadLineBack()
    {
        var projectRequest = new ProjectRequest
        {
            DeadLine = default(DateTime),
            Name = "New project",
        };

        MockApiContext.Setup(x => x.InsertProject(It.IsAny<Entities.Project>()))
                                .Callback<Entities.Project>((project) =>
                                {
                                    var maxId = _projectsDb.Count > 0 ? _projectsDb.Max(p => p.Id) : 1;
                                    project.Id = maxId + 1;
                                    _projectsDb.Add(project);
                                })
                                .Returns((Entities.Project project) => project);

        IActionResult insertNewProject = _projectsController.Insert(projectRequest);
        OkObjectResult? okResult = insertNewProject as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Single(getResponse.Projects);
        Assert.True(getResponse.Error == null);
        Assert.Equal(5, _projectsDb.Count);

        foreach (var project in getResponse.Projects)
        {
            var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
            Assert.Equal(5, project.Id);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(findProjectFromBd.Name, project.Name);
            Assert.Equal("New project", project.Name);
            Assert.Equal(projectRequest.Name, project.Name);
            Assert.Equal(0, project.TimeSpend);
            Assert.Equal(findProjectFromBd.Id, project.Id);
            Assert.Equal(findProjectFromBd.TimeSpend, project.TimeSpend);
            Assert.Equal(projectRequest.DeadLine.Value.ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(DateTime.UtcNow.AddMonths(12).ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(findProjectFromBd.DeadLine, project.DeadLine);
            Assert.Equal(findProjectFromBd.ProjectIsCompleted, project.ProjectIsCompleted);
        }
    }

    [Fact]
    public void InsertNewProject_NewProject_ThrowNewExceptionProjectBack()
    {
        var projectRequest = new ProjectRequest
        {
            DeadLine = DateTime.UtcNow.AddDays(120),
            Name = "New project",
        };

        MockApiContext.Setup(x => x.InsertProject(It.IsAny<Entities.Project>())).Throws(new Exception("Something happen"));

        IActionResult insertNewProject = _projectsController.Insert(projectRequest);
        BadRequestObjectResult? badResult = insertNewProject as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.Empty(getResponse.Projects);
        Assert.True(getResponse.Error != null);
        Assert.Equal("Something happen", getResponse.Error.Message);
    }

    [Fact]
    public void InsertNewProjectNoDeadLine_NewProject_NewCreatedProjectBack()
    {
        var projectRequest = new ProjectRequest
        {
            Name = "New project"
        };

        MockApiContext.Setup(x => x.InsertProject(It.IsAny<Entities.Project>()))
                                .Callback<Entities.Project>((project) =>
                                {
                                    var maxId = _projectsDb.Count > 0 ? _projectsDb.Max(p => p.Id) : 1;
                                    project.Id = maxId + 1;
                                    _projectsDb.Add(project);
                                })
                                .Returns((Entities.Project project) => project);

        IActionResult insertNewProject = _projectsController.Insert(projectRequest);
        OkObjectResult? okResult = insertNewProject as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Single(getResponse.Projects);
        Assert.True(getResponse.Error == null);
        Assert.Equal(5, _projectsDb.Count);

        foreach (var project in getResponse.Projects)
        {
            var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
            Assert.Equal(5, project.Id);
            Assert.Equal("New project", project.Name);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(findProjectFromBd.Name, project.Name);
            Assert.Equal(projectRequest.Name, project.Name);
            Assert.Equal(0, project.TimeSpend);
            Assert.Equal(findProjectFromBd.Id, project.Id);
            Assert.Equal(findProjectFromBd.TimeSpend, project.TimeSpend);
            Assert.Equal(DateTime.UtcNow.AddMonths(12).ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(findProjectFromBd.DeadLine, project.DeadLine);
            Assert.Equal(findProjectFromBd.ProjectIsCompleted, project.ProjectIsCompleted);
        }
    }

    [Fact]
    public void ErrorOnInsertNewProject_NewProject_ErrorBack()
    {
        var projectRequest = new ProjectRequest
        {
            DeadLine = DateTime.UtcNow.AddDays(120),
            Name = "",
        };

        IActionResult insertNewProject = _projectsController.Insert(projectRequest);
        BadRequestObjectResult? badResult = insertNewProject as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.True(getResponse.Projects.Count == 0);
        Assert.True(getResponse.Error != null);
        Assert.Equal(4, _projectsDb.Count);
        Assert.Equal("Value cannot be null. (Parameter 'Name of project is null or empty.')", getResponse.Error.Message);
    }

    [Fact]
    public void ErrorOnInsertNewProject_NewProject_ErrorNullRequestBack()
    {
        IActionResult insertNewProject = _projectsController.Insert(null);
        BadRequestObjectResult? badResult = insertNewProject as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.True(getResponse.Projects.Count == 0);
        Assert.True(getResponse.Error != null);
        Assert.Equal(4, _projectsDb.Count);
        Assert.Equal("Value cannot be null. (Parameter 'projectRequest')", getResponse.Error.Message);
    }


    [Fact]
    public void UpdateProject_UpdateProject_UpdateProjectBack()
    {
        var projectRequest = new UpdateProjectRequest
        {
            Id = 3,
            Name = "New project33",
            ProjectIsCompleted = false,
            TimeSpend = 40,
            DeadLine = DateTime.UtcNow.AddDays(150)
        };

        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Returns((long id) =>
        {
            return _projectsDb.FirstOrDefault(x => x.Id == id);
        });

        MockApiContext.Setup(x => x.UpdateProject(It.IsAny<Entities.Project>()))
                                 .Callback<Entities.Project>((project) =>
                                 {
                                     Assert.Equal("New project33", project.Name);
                                     Assert.Equal(DateTime.UtcNow.AddDays(150).ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
                                     var projectFind = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
                                     if (projectFind != null)
                                     {
                                         projectFind.ProjectIsCompleted = project.ProjectIsCompleted;
                                         projectFind.DeadLine = project.DeadLine;
                                         projectFind.Name = project.Name;
                                         projectFind.TimeSpend = project.TimeSpend;
                                     }
                                   
                                 })
                                .Returns((Entities.Project project) => project);

        IActionResult updateProject = _projectsController.Update(projectRequest);
        OkObjectResult? okResult = updateProject as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Single(getResponse.Projects);
        Assert.True(getResponse.Error == null);
        Assert.Equal(4, _projectsDb.Count);

        foreach (var project in getResponse.Projects)
        {
            var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(projectRequest.Name, project.Name);
            Assert.Equal(180, project.TimeSpend);
            Assert.Equal(findProjectFromBd.Id, project.Id);
            Assert.Equal(projectRequest.DeadLine.Value.ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(projectRequest.ProjectIsCompleted, project.ProjectIsCompleted);
        }
    }

    [Fact]
    public void NullDeadLineUpdateProject_UpdateProject_UpdateProjectBack()
    {
        var projectRequest = new UpdateProjectRequest
        {
            Id = 3,
            Name = "New project45",
            ProjectIsCompleted = false,
            TimeSpend = 20,
            DeadLine = null
        };

        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Returns((long id) =>
        {
            return _projectsDb.FirstOrDefault(x => x.Id == id);
        });

        MockApiContext.Setup(x => x.UpdateProject(It.IsAny<Entities.Project>()))
                                 .Callback<Entities.Project>((project) =>
                                 {
                                     Assert.Equal("New project45", project.Name);
                                     Assert.Equal(180, project.TimeSpend);
                                     
                                     var projectFind = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
                                     if (projectFind != null)
                                     {
                                         Assert.Equal(DateTime.UtcNow.AddDays(90).ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
                                         projectFind.ProjectIsCompleted = project.ProjectIsCompleted;
                                         projectFind.DeadLine = project.DeadLine;
                                         projectFind.Name = project.Name;
                                         projectFind.TimeSpend = project.TimeSpend;
                                     }

                                 })
                                .Returns((Entities.Project project) => project);

        IActionResult updateProject = _projectsController.Update(projectRequest);
        OkObjectResult? okResult = updateProject as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Single(getResponse.Projects);
        Assert.True(getResponse.Error == null);
        Assert.Equal(4, _projectsDb.Count);

        foreach (var project in getResponse.Projects)
        {
            var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(projectRequest.Name, project.Name);
            Assert.Equal(180, project.TimeSpend);
            Assert.Equal(findProjectFromBd.Id, project.Id);
            Assert.Equal(projectRequest.DeadLine.Value.ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(projectRequest.ProjectIsCompleted, project.ProjectIsCompleted);
        }
    }

    [Fact]
    public void UpdateProject_UpdateProject_UpdateProjectNoTimeSpendUpdateBack()
    {
        var projectRequest = new UpdateProjectRequest
        {
            Id = 3,
            Name = "New project",
            ProjectIsCompleted = false,
            TimeSpend = 0,
            DeadLine = DateTime.UtcNow.AddDays(50)
        };

        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Returns((long id) =>
        {
            return _projectsDb.FirstOrDefault(x => x.Id == id);
        });

        MockApiContext.Setup(x => x.UpdateProject(It.IsAny<Entities.Project>()))
                                 .Callback<Entities.Project>((project) =>
                                 {
                                     Assert.Equal("New project", project.Name);
                                     var projectFind = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
                                     if (projectFind != null)
                                     {
                                         Assert.Equal(180, project.TimeSpend);
                                         projectFind.ProjectIsCompleted = project.ProjectIsCompleted;
                                         projectFind.DeadLine = project.DeadLine;
                                         projectFind.Name = project.Name;
                                         projectFind.TimeSpend = project.TimeSpend;
                                     }

                                 })
                                .Returns((Entities.Project project) => project);

        IActionResult updateProject = _projectsController.Update(projectRequest);
        OkObjectResult? okResult = updateProject as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Single(getResponse.Projects);
        Assert.True(getResponse.Error == null);
        Assert.Equal(4, _projectsDb.Count);

        foreach (var project in getResponse.Projects)
        {
            var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(projectRequest.Name, project.Name);
            Assert.Equal(180, project.TimeSpend);
            Assert.Equal(findProjectFromBd.Id, project.Id);
            Assert.Equal(projectRequest.DeadLine.Value.ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(projectRequest.ProjectIsCompleted, project.ProjectIsCompleted);
        }
    }

    [Fact]
    public void NotUpdateProject_UpdateProject_UpdateProjectDeadLineInPastBack()
    {
        var projectRequest = new UpdateProjectRequest
        {
            Id = 3,
            Name = "New project",
            ProjectIsCompleted = false,
            TimeSpend = 0,
            DeadLine = DateTime.UtcNow.AddDays(-50)
        };

        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Returns((long id) =>
        {
            return _projectsDb.FirstOrDefault(x => x.Id == id);
        });

        MockApiContext.Setup(x => x.UpdateProject(It.IsAny<Entities.Project>()))
                                 .Callback<Entities.Project>((project) =>
                                 {
                                     Assert.Equal("New project", project.Name);
                                     Assert.Equal(DateTime.UtcNow.AddDays(90).ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
                                     var projectFind = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
                                     if (projectFind != null)
                                     {
                                         Assert.Equal(180, project.TimeSpend);
                                         projectFind.ProjectIsCompleted = project.ProjectIsCompleted;
                                         projectFind.DeadLine = project.DeadLine;
                                         projectFind.Name = project.Name;
                                         projectFind.TimeSpend = project.TimeSpend;
                                     }

                                 })
                                .Returns((Entities.Project project) => project);

        IActionResult updateProject = _projectsController.Update(projectRequest);
        OkObjectResult? okResult = updateProject as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Single(getResponse.Projects);
        Assert.True(getResponse.Error == null);
        Assert.Equal(4, _projectsDb.Count);

        foreach (var project in getResponse.Projects)
        {
            var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(projectRequest.Name, project.Name);
            Assert.Equal(180, project.TimeSpend);
            Assert.Equal(findProjectFromBd.Id, project.Id);
            Assert.Equal(projectRequest.DeadLine.Value.ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.Equal(projectRequest.ProjectIsCompleted, project.ProjectIsCompleted);
        }
    }

    [Fact]
    public void NotFindProject_UpdateProject_GetErrorBack()
    {
        var projectRequest = new UpdateProjectRequest
        {
            Id = 3,
            Name = "New project",
            ProjectIsCompleted = false,
            TimeSpend = 60,
            DeadLine = DateTime.UtcNow.AddDays(50)
        };

        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Returns((long id) =>
        {
            return null;
        });

        IActionResult updateProject = _projectsController.Update(projectRequest);
        BadRequestObjectResult? badResult = updateProject as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.True(getResponse.Projects.Count == 0);
        Assert.True(getResponse.Error != null);
        Assert.Equal(4, _projectsDb.Count);
        Assert.Equal("Project don't found on Database.", getResponse.Error.Message);
    }

    [Fact]
    public void NotFindProject_UpdateProject_ThrowNewExceptionErrorBack()
    {
        var projectRequest = new UpdateProjectRequest
        {
            Id = 3,
            Name = "New project",
            ProjectIsCompleted = false,
            TimeSpend = 50,
            DeadLine = DateTime.UtcNow.AddDays(50)
        };

        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Throws(new Exception("Something happen"));

        IActionResult updateProject = _projectsController.Update(projectRequest);
        BadRequestObjectResult? badResult = updateProject as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.True(getResponse.Projects.Count == 0);
        Assert.True(getResponse.Error != null);
        Assert.Equal(4, _projectsDb.Count);
        Assert.Equal("Something happen", getResponse.Error.Message);
    }

    [Fact]
    public void NameOnRequestProject_UpdateProject_GetErrorBack()
    {
        var projectRequest = new UpdateProjectRequest
        {
            Id = 3,
            Name = "",
            ProjectIsCompleted = false,
            TimeSpend = 20,
            DeadLine = DateTime.UtcNow.AddDays(50)
        };

        IActionResult updateRequest = _projectsController.Update(projectRequest);
        BadRequestObjectResult? badResult = updateRequest as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.True(getResponse.Projects.Count == 0);
        Assert.True(getResponse.Error != null);
        Assert.Equal(4, _projectsDb.Count);
        Assert.Equal("Value cannot be null. (Parameter 'Name of project is null or empty.')", getResponse.Error.Message);
    }

    [Fact]
    public void RequestIsNullProject_UpdateProject_GetErrorBack()
    {
        IActionResult updateProject = _projectsController.Update(null);
        BadRequestObjectResult? badResult = updateProject as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.True(getResponse.Projects.Count == 0);
        Assert.True(getResponse.Error != null);
        Assert.Equal(4, _projectsDb.Count);
        Assert.Equal("Value cannot be null. (Parameter 'updateProjectRequest')", getResponse.Error.Message);
    }

    [Fact]
    public void NotFindProject_UpdateStatusProject_GetErrorBack()
    {
        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Returns((long id) =>
        {
            return _projectsDb.FirstOrDefault(x => x.Id == id);
        });

        IActionResult updateStatus = _projectsController.UpdateStatus(6, false);
        BadRequestObjectResult? badResult = updateStatus as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.True(getResponse.Projects.Count == 0);
        Assert.True(getResponse.Error != null);
        Assert.Equal(4, _projectsDb.Count);
        Assert.Equal("Project don't found on Database.", getResponse.Error.Message);
    }

    [Fact]
    public void UpdateProject_UpdateStatusProject_UpdateStatusBack()
    {
        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Returns((long id) =>
        {
            return _projectsDb.FirstOrDefault(x => x.Id == id);
        });

        MockApiContext.Setup(x => x.UpdateProject(It.IsAny<Entities.Project>()))
                         .Callback<Entities.Project>((project) =>
                         {
                             Assert.True(project.ProjectIsCompleted);
                             var projectFind = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
                             if (projectFind != null)
                             {
                                 projectFind.ProjectIsCompleted = project.ProjectIsCompleted;
                                 projectFind.DeadLine = project.DeadLine;
                                 projectFind.Name = project.Name;
                                 projectFind.TimeSpend = project.TimeSpend;
                             }
                         })
                        .Returns((Entities.Project project) => project);

        IActionResult updateStatusResponse = _projectsController.UpdateStatus(2, true);
        OkObjectResult? okResult = updateStatusResponse as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Single(getResponse.Projects);
        Assert.True(getResponse.Error == null);
        Assert.Equal(4, _projectsDb.Count);

        foreach (var project in getResponse.Projects)
        {
            var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(findProjectFromBd.Name, project.Name);
            Assert.Equal(findProjectFromBd.TimeSpend, project.TimeSpend);
            Assert.Equal(findProjectFromBd.Id, project.Id);
            Assert.Equal(findProjectFromBd.DeadLine.Value.ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.True(findProjectFromBd.ProjectIsCompleted);
        }
    }

    [Fact]
    public void UpdateProject_UpdateStatusProject_ThrownNewExceptionBack()
    {
        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Throws(new Exception("Something happen"));

        IActionResult updateStatusResponse = _projectsController.UpdateStatus(2, true);
        BadRequestObjectResult? badResult = updateStatusResponse as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.True(getResponse.Projects.Count == 0);
        Assert.True(getResponse.Error != null);
        Assert.Equal(4, _projectsDb.Count);
        Assert.Equal("Something happen", getResponse.Error.Message);
    }

    [Fact]
    public void UpdateTimeUsedProject_UpdateTimeUsedProject_UpdateTimeSuccessBack()
    {
        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Returns((long id) =>
        {
            return _projectsDb.FirstOrDefault(x => x.Id == id);
        });

        MockApiContext.Setup(x => x.UpdateProject(It.IsAny<Entities.Project>()))
                         .Callback<Entities.Project>((project) =>
                         {
                             Assert.Equal(230, project.TimeSpend);
                             var projectFind = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
                             if (projectFind != null)
                             {
                                 projectFind.ProjectIsCompleted = project.ProjectIsCompleted;
                                 projectFind.DeadLine = project.DeadLine;
                                 projectFind.Name = project.Name;
                                 projectFind.TimeSpend = project.TimeSpend;
                             }
                             
                         })
                        .Returns((Entities.Project project) => project);

        IActionResult updateTimeSpend = _projectsController.UpdateTimeSpend(2, 80);
        OkObjectResult? okResult = updateTimeSpend as OkObjectResult;

        Assert.True(okResult != null);
        Assert.Equal(200, okResult.StatusCode);
        Assert.True(okResult.Value is ProjectResponse);

        var getResponse = okResult.Value as ProjectResponse;
        Assert.True(getResponse != null && getResponse.IsSucess);
        Assert.Single(getResponse.Projects);
        Assert.True(getResponse.Error == null);
        Assert.Equal(4, _projectsDb.Count);

        foreach (var project in getResponse.Projects)
        {
            var findProjectFromBd = _projectsDb.FirstOrDefault(x => x.Id == project.Id);
            Assert.True(findProjectFromBd != null);
            Assert.Equal(findProjectFromBd.Name, project.Name);
            Assert.Equal(findProjectFromBd.TimeSpend, project.TimeSpend);
            Assert.Equal(findProjectFromBd.Id, project.Id);
            Assert.Equal(findProjectFromBd.DeadLine.Value.ToString("yyyy-MM-dd"), project.DeadLine.Value.ToString("yyyy-MM-dd"));
            Assert.False(findProjectFromBd.ProjectIsCompleted);
        }
    }

    [Fact]
    public void UpdateTimeUsedProject_UpdateTimeUsedProject_GetErrorBack()
    {
        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Returns((long id) =>
        {
            return _projectsDb.FirstOrDefault(x => x.Id == id);
        });

        IActionResult updateTimeSpend = _projectsController.UpdateTimeSpend(8, 120);
        BadRequestObjectResult? badResult = updateTimeSpend as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.True(getResponse.Projects.Count == 0);
        Assert.True(getResponse.Error != null);
        Assert.Equal(4, _projectsDb.Count);
        Assert.Equal("Project don't found on Database.", getResponse.Error.Message);
    }

    [Fact]
    public void UpdateTimeUsedProject_UpdateTimeUsedProject_ThrowNewExceptionErrorBack()
    {
        MockApiContext.Setup(x => x.GetProjectById(It.IsAny<long>())).Throws(new Exception("Something happen"));

        IActionResult updateTimeSpend = _projectsController.UpdateTimeSpend(8, 120);
        BadRequestObjectResult? badResult = updateTimeSpend as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.True(getResponse.Projects.Count == 0);
        Assert.True(getResponse.Error != null);
        Assert.Equal(4, _projectsDb.Count);
        Assert.Equal("Something happen", getResponse.Error.Message);
    }

    [Fact]
    public void NotValidTimeUsedProject_UpdateTimeUsedProject_GetErrorBack()
    {
        IActionResult updateTimeSpendResponse = _projectsController.UpdateTimeSpend(8, 29);
        BadRequestObjectResult? badResult = updateTimeSpendResponse as BadRequestObjectResult;

        Assert.True(badResult != null);
        Assert.Equal(400, badResult.StatusCode);
        Assert.True(badResult.Value is ProjectResponse);

        var getResponse = badResult.Value as ProjectResponse;
        Assert.True(getResponse != null && !getResponse.IsSucess);
        Assert.True(getResponse.Projects.Count == 0);
        Assert.True(getResponse.Error != null);
        Assert.Equal(4, _projectsDb.Count);
        Assert.Equal("The time spend on project is less than 30. Need to be more than 30 minutes.", getResponse.Error.Message);
    }
}