using Microsoft.AspNetCore.Mvc;
using Features.Projects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;
using Asp.Versioning;
using Repositories;
using System.Security.Claims;
using Authentication.UserManager;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace Features.Projects;

[Tags("Projects Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/projects")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;

    public ProjectsController(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(GetProjectsListResponse))]
    public async Task<IActionResult> 
        GetProjectsAsync(
        [FromQuery] int? PageSize,
        [FromQuery] int? PageNumber,
        [FromQuery] string? SearchPattern)
    {
        var projects = await _projectRepository.GetAllAsync(
            PageSize,
            PageNumber, 
            SearchPattern);

        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDetailsReadDto>> GetProject(int id)
    {
        var project = await _projectRepository.GetDetailsAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        return Ok(project);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest,Type = typeof(ErrorResponse))]
    public async Task<IActionResult> CreateProjectAsync(
        [FromBody] ProjectCreateDto projectDto,
        [FromServices] CustomUserManager userManager
    )
    {   
        User? user = await userManager.GetUserAsync(User);
        
        if(user is null) 
            return BadRequest(new ErrorResponse("User account does not exist any more"));

        if (user.EmailConfirmed is false)
            return BadRequest(new ErrorResponse("User email is not confirmed"));
        var projectId = await _projectRepository.CreateAsync(projectDto,user);
        var projectReadDto = new ProjectReadDto
        (
            Id : projectId,
            Name: projectDto.Name,
            Description: projectDto.Description,
            StartDate: projectDto.StartDate,
            null,
            1,
            [new ProjecUserShortDto(user.Id, user.ProfilePicture)]
        );
        return CreatedAtAction(nameof(GetProject), new { id = projectId }, projectReadDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectDetailsReadDto>> UpdateProject(int id, [FromBody] ProjectCreateDto projectDto)
    {
        // Validate the projectDto if necessary
        await _projectRepository.UpdateAsync(id, projectDto);
        var updatedProject = await _projectRepository.GetDetailsAsync(id);
        if (updatedProject == null)
        {
            return NotFound();
        }
        return Ok(updatedProject);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var existingProject = await _projectRepository.GetByIdAsync(id);
        if (existingProject == null)
        {
            return NotFound();
        }
        await _projectRepository.DeleteAsync(id);
        return NoContent();
    }
}

