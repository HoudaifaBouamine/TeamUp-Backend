using Features.Projects.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Features.Projects;
partial class ProjectsController
{
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDetailsReadDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProjectAsync(int id, [FromBody] ProjectCreateDto projectDto)
    {
        await _projectRepository.UpdateAsync(id, projectDto);
        var updatedProject = await _projectRepository.GetDetailsAsync(id);
        if (updatedProject == null)
        {
            return NotFound();
        }
        return Ok(updatedProject);
    }
}

partial class ProjectRepository
{
    public async Task<bool> UpdateAsync(int id, ProjectCreateDto projectDto)
    {
        var project = await _context.Projects.FindAsync(id);
        
        if (project is not null)
        {
            project.Name = projectDto.Name;
            project.Description = projectDto.Description;
            project.StartDate = projectDto.StartDate;

            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}