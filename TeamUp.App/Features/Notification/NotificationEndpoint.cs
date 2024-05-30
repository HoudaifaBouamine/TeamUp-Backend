using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Mentor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utils;

namespace Features;

[Tags("Notification Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/notifications")]
[ApiController]
public class NotificationEndpoints(AppDbContext db, UserManager<User> userManager) : ControllerBase
{
    
}
