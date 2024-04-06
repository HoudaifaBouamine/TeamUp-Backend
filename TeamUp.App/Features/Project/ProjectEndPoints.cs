using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Repositories;
namespace Features.Projects;

[Tags("Projects Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/projects")]
[ApiController]
public partial class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;

    public ProjectsController(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }
}

