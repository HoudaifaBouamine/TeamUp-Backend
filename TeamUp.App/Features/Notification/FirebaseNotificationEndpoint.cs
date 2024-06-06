using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utils;

namespace Features;

[Tags("Notification Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/notifications/firebase")]
[ApiController]
public class FireBaseNotificationEndpoints(AppDbContext db, UserManager<User> userManager) : ControllerBase
{
    
    
    
    /// <summary>
    /// Store session token
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> StoreSession([FromQuery] string sessionToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var session = new FireBaseNotificationSession(user.Id, sessionToken);
        db.FireBaseNotificationSessions.Add(session);
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("{userId}")]
    [Authorize]
    [ProducesResponseType<FireBaseSessionTokenDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserSession(Guid userId)
    {
        var session = await db.FireBaseNotificationSessions
            .Where(s => s.UserId == userId)
            .OrderBy(s=>s.Id)
            .LastOrDefaultAsync();

        if (session is null) return NotFound();

        return Ok(new FireBaseSessionTokenDto(session.SessionToken));
    }

    record FireBaseSessionTokenDto(string SessionToken);
}
