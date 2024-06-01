using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Features;


[Tags("Follow Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/mentors")]
[ApiController]
public class FollowEndpoints : ControllerBase
{

}