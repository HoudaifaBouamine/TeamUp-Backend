using Microsoft.AspNetCore.Mvc;

namespace Features.Projects;
partial class ProjectsController
{
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDetailsReadDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectCreateDto projectDto)
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