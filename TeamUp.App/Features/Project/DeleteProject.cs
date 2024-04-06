using Microsoft.AspNetCore.Mvc;

namespace Features.Projects;
partial class ProjectsController
{
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var existingProject = await _projectRepository.GetByIdAsync(id);
        if (existingProject is null)
        {
            return NotFound();
        }
        await _projectRepository.DeleteAsync(id);
        return NoContent();
    }
}