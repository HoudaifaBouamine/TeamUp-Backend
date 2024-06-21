using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace TeamUp.Features.Notification;

[Tags("Notification Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/notifications/firebase")]
[ApiController]
public partial class FireBaseNotificationEndpoints(
    [FromServices] AppDbContext db,
    [FromServices] UserManager<User> userManager) : ControllerBase;
