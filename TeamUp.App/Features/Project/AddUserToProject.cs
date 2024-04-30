using Features.Projects.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Features.Projects;
partial class ProjectsController
{
    [HttpGet("{id}/AddUser/{user_id}")]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(ProjectDetailsReadDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddUserToProjectAsync(int id,Guid user_id,[FromQuery] bool isMentor = false)
    {
        var isAddedSuccessfuly = await _projectRepository.AddUserToProjectAsync(id, user_id,isMentor);
        if(isAddedSuccessfuly)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
}


partial class ProjectRepository
{
    public async Task<bool> AddUserToProjectAsync(int projectId, Guid userId, bool isMentor)
    {
        var project = await _context.Projects
            .Include(p => p.ProjectsUsers)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if(project is null) return false;

       
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return false;

        project.AddUser(user, isMentor);

        await _context.SaveChangesAsync();
    
        return true;    
    }

    /// <summary>
    /// Add list of users to a project, returning the number of users added successfuly
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="userIds"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="userIds"></param>
    /// <returns></returns>
    public async Task<int> AddUsersToProjectAsync(int projectId, List<Guid> userIds)
    {
        var project = await _context.Projects
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if(project is null) return -1;

        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();

        var prevUsersCount = project.Users.DistinctBy(u=>u.Id).Count();
        project.AddUsers(users);

        await _context.SaveChangesAsync();
        
        return project.TeamSize - prevUsersCount;
    }

}