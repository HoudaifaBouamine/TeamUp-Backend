using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TeamUp.Features.Notification;

partial class FireBaseNotificationEndpoints
{
    [HttpGet("{userId:guid}")]
    [Authorize]
    [ProducesResponseType<FireBaseSessionTokenDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserSession(Guid userId)
    {
        var session = await db.FireBaseNotificationSessions
            .Where(s => s.UserId == userId)
            .OrderBy(s=>s.Id)
            .LastOrDefaultAsync();

        if (session is null) return NotFound();

        return Ok(new FireBaseSessionTokenDto(session.SessionToken));
    }
    private record FireBaseSessionTokenDto(string SessionToken);
}
