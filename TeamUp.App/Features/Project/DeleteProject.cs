using Microsoft.AspNetCore.Mvc;

namespace Features.Projects;
partial class ProjectsController
{
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProjectAsync(int id)
    {

        var success = await _projectRepository.DeleteAsync(id);
        
        if(success) return NoContent();

        return NotFound();
    }
}

partial class ProjectRepository
{
    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        
        if (project is null)
            return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return true;
    }
}