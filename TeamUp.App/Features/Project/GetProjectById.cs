using Features.Projects.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Features.Projects;

partial class ProjectsController
{
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(ProjectDetailsReadDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(int id)
    {
        var project = await _projectRepository.GetDetailsAsync(id);
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
        var project = await _context.Projects.FindAsync(id);
        return MapProjectToProjectReadDto(project!);
    }
}

    

