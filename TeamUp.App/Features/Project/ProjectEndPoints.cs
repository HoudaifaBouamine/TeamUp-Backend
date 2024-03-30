using Microsoft.AspNetCore.Mvc;
using Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Authentication.UserManager;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Features.Projects;

[Tags("Projects Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/projects")]
[ApiController]
public class ProjectsController : ControllerBase
{

    private readonly AppDbContext  _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<GetProjectsListResponse>> GetProjects(
        [FromQuery] string? SearchPattern,
        [FromQuery] int? PageNumber,
        [FromQuery] int? PageSize)    
    {

        if(PageSize is null) PageNumber = null;

        IQueryable<Project> projects = _context.Projects;

        if(SearchPattern is not null)
            projects = projects.Where(p=>
                p.Description.ToLower().Contains(SearchPattern.ToLower()) ||
                p.Name.ToLower().Contains(SearchPattern.ToLower()));

        int TotalCount = projects.Count();

        if(PageSize is not null && PageNumber is not null)
            projects = projects
                .Skip(PageSize.Value * (PageNumber.Value -1))
                .Take(PageSize.Value);
        else if (PageSize is not null)
            projects = projects
                .Take(PageSize.Value);

        var projectsDto = await projects
            .Include(p=>p.Users)
            .Select(p => new ProjectReadDto
            (
                p.Id,
                p.Name,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.Users.Count,
                p.Users.Take(3).Select(u => new ProjecUserShortDto
                (
                    u.Id,
                    u.ProfilePicture            
                )).ToList()
            )).ToListAsync();

            return Ok(new GetProjectsListResponse
            (
                TotalCount : TotalCount,
                PageNumber : PageNumber??=1,
                PageSize : PageSize??=TotalCount,
                IsPrevPageExist : PageNumber > 1,
                IsNextPageExist : PageNumber * PageSize < TotalCount, 
                Projects: projectsDto
            ));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDetailsReadDto>> GetProject(int id)
    {
        var project = await _context.Projects
            .Include(p=>p.Users)
            .Include(p=>p.ProjectsUsers)
            .Select(p=>new ProjectDetailsReadDto
            (
                p.Id,
                p.Name,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.Users.Count(),
                p.Users.Select(u=>new ProjecUserLongDto
                (
                    u.Id,
                    u.DisplayName,
                    u.Handler,
                    u.ProfilePicture,
                    p.ProjectsUsers.First(up=>up.UserId == u.Id).IsMentor
                )).ToList()               
            ))
            .FirstOrDefaultAsync(u=>u.Id == id);

        if (project is null)
        {
            return NotFound();
        }


        return Ok(project);
    }

    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProjectCreateDto>> CreateProject(
        [FromBody] ProjectCreateDto projectDto,
        [FromServices] CustomUserManager userManager)
    {
        User? user = await userManager.GetUserAsync(User); // the variable User declared in the controller base class
        
        if(user is null) return NotFound(new ErrorResponse("User not found, this should not happen (Contact the backend developer to fix the bag)"));

        if(! user.EmailConfirmed) return BadRequest(new ErrorResponse("To create a project, your email must be verified"));

        var project = new Project
        {
            Name = projectDto.Name,
            Description = projectDto.Description,
            StartDate = projectDto.StartDate,
            EndDate = null,
            ChatRoom = new ChatRoom(),
            Users = [user]
        };

        project.ProjectsUsers.Add(new UsersProject()
        {
            User = user,
            IsMentor = true
        });

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, projectDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectDetailsReadDto>> UpdateProject(int id, ProjectCreateDto projectDto)
    {

        var project = await _context.Projects.FindAsync(id);

        if (project is null) return NotFound();

        project.Name = projectDto.Name;
        project.Description = projectDto.Description;
        project.StartDate = projectDto.StartDate;

        await _context.SaveChangesAsync();

        return Ok(project);
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project is null) return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}



