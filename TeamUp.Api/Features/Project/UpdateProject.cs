using Microsoft.AspNetCore.Mvc;

namespace TeamUp.Features.Project;

public partial class ProjectsController
{
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDetailsReadDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProjectAsync(int id, [FromBody] ProjectCreateDto projectDto)
    {
        await projectRepo.UpdateAsync(id, projectDto);
        var updatedProject = await projectRepo.GetDetailsAsync(id);
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
        var project = await db.Projects.FindAsync(id);

        if (project is null) return false;
        
        project.Name = projectDto.Name;
        project.Description = projectDto.Description;
        project.StartDate = projectDto.StartDate;

        await db.SaveChangesAsync();
        return true;
    }
}