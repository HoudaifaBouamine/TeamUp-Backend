using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace TeamUp.Features.Notification;

public partial class FireBaseNotificationEndpoints
{
    /// <summary>
    /// Store session token
    /// </summary>
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
}
