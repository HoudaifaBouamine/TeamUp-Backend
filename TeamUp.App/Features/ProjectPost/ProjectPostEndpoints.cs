using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
namespace TeamUp;


[Tags("Project Posts Group")]
[ApiVersion(4)]
[Route("api/v{v:apiVersion}/projects-posts")]
[ApiController]
public class ProjectPostEndpoints(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext db = db;

    [HttpGet]
    public async Task<IActionResult> GetProjectPosts()
    {
        return Ok(db.ProjectPosts.ToList());
    }
}
