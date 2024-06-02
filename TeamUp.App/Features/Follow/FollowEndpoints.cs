using Asp.Versioning;
using Humanizer.DateTimeHumanizeStrategy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Features;


[Tags("Follow Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/mentors")]
[ApiController]
public class FollowEndpoints : ControllerBase
{
    private readonly AppDbContext db;
    private readonly UserManager<User> userManager;
    public FollowEndpoints(AppDbContext db, UserManager<User> userManager)
    {
        this.db = db;
        this.userManager = userManager;
    }
    
    /// <summary>
    /// Follow
    /// </summary>
    /// <param name="userToFollowId"></param>
    /// <returns></returns>
    [HttpPost("{userToFollowId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Follow(Guid userToFollowId)
    {
        var currentUser = await userManager.GetUserAsync(User);
        var userToFollow = await db.Users.FirstOrDefaultAsync(u => u.Id == userToFollowId);
        
        if (currentUser is null) return Unauthorized();
        if (userToFollow is null) return NotFound();
        
        var follow = new Follow()
        {
            Followee = userToFollow,
            Follower = currentUser
        };
        
        db.Follows.Add(follow);
        return NoContent();
    }
    
    /// <summary>
    /// UnFollow
    /// </summary>
    /// <param name="userToFollowId"></param>
    /// <returns></returns>
    [HttpDelete("{userToFollowId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnFollow(Guid userToFollowId)
    {
        var currentUser = await userManager.GetUserAsync(User);
        var userToFollow = await db.Users.FirstOrDefaultAsync(u => u.Id == userToFollowId);
        
        if (currentUser is null) return Unauthorized();
        if (userToFollow is null) return NotFound();

        await db.Follows
            .Where(f=>f.Follower == currentUser 
                      && f.Followee == userToFollow)
            .ExecuteDeleteAsync();
        
        return NoContent();
    }
}