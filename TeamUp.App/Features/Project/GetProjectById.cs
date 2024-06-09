using Microsoft.AspNetCore.Mvc;

namespace TeamUp.Features.Project;

public partial class ProjectsController
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(ProjectDetailsReadDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(int id)
    {
        var project = await projectRepo.GetDetailsAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        return Ok(project);
    }
}

partial class ProjectRepository
{
    public async Task<ProjectReadDto> GetByIdAsync(int id)
    {
        var project = await db.Projects.FindAsync(id);
        return MapProjectToProjectReadDto(project);
    }
}

    

