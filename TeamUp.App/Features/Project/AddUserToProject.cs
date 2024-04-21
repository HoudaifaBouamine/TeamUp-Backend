using Features.Projects.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Models;

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

        if (project is not null)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());

            if (user != null)
            {
                project.ProjectsUsers.Add(new UsersProject
                {
                    User = user,
                    IsMentor = isMentor
                });
                project.TeamSize++;

                await _context.SaveChangesAsync();
                return true;
            }
        }
        return false;
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
            .Where(u => userIds.Contains(Guid.Parse(u.Id)))
            .ToListAsync();

        var prevUsersCount = project.Users.Count();
        project.Users.AddRange(users);
        var newUsersCount = project.Users.Count();
        project.TeamSize = newUsersCount;

        await _context.SaveChangesAsync();
        
        return newUsersCount - prevUsersCount;
    }

}