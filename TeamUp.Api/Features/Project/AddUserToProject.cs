using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Utils;

namespace TeamUp.Features.Project;

public partial class ProjectsController
{
    [Authorize]
    [HttpGet("{projectId:int}/AddUser/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(ProjectDetailsReadDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddUserToProjectAsync(int projectId,Guid userId,[FromQuery] bool isMentor = false)
    {
        if (!await projectRepo.CheckMentor(userId, projectId)) 
            return Forbid();
        
        if (await projectRepo.AddUserToProjectAsync(projectId, userId, isMentor))
            return Ok();

        Log.Error("This should not happen");
        return BadRequest(new ErrorResponse("This should not happen"));
    }
}


partial class ProjectRepository
{
    /// <summary>
    /// Check if the user is mentor in a specified project
    /// </summary>
    /// <param name="userId">user id to check</param>
    /// <param name="projectId"></param>
    /// <returns>true if user is mentor, otherwise return false</returns>
    public async Task<bool> CheckMentor(Guid userId, int projectId)
    {
        return await db.UsersProjects
            .Where(p =>
                p.UserId == userId
                &&
                p.ProjectId == projectId
                &&
                p.IsMentor == true)
            .AnyAsync();
    }

    public async Task<bool> AddUserToProjectAsync(int projectId, Guid userId, bool isMentor)
    {
        var project = await db.Projects
            .Include(p => p.ProjectsUsers)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project is null) return false;
        
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return false;
        
        project.AddUser(user, isMentor);

        await db.SaveChangesAsync();

        return true;
    }
}