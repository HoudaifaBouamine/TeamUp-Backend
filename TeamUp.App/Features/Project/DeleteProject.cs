using Microsoft.AspNetCore.Mvc;

namespace TeamUp.Features.Project;

public partial class ProjectsController
{
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProjectAsync(int id)
    {
        var success = await projectRepo.DeleteAsync(id);
        
        if(success) return NoContent();

        return NotFound();
    }
}

partial class ProjectRepository
{
    public async Task<bool> DeleteAsync(int id)
    {
        var project = await db.Projects.FindAsync(id);
        
        if (project is null)
            return false;

        db.Projects.Remove(project);
        await db.SaveChangesAsync();

        return true;
    }
}