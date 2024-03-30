using TeamUp.Repositories;
using Models ; 
using Features.Projects ;
using Microsoft.EntityFrameworkCore;
using Features.Projects;
namespace TeamUp.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectReadDto>> GetAllProjectsAsync()
    {
        var projects = await _context.Projects
            .Include(p=>p.Users)
            .Select(p => new ProjectReadDto
            {
                Id = p.Id,
                Name = p.ProjectName,
                Description = p.ProjectDescription,
                StartDate = p.StartDateTime,
                EndDate = p.EndDateTime,
                UsersCount = p.Users.Count(),
                UsersSample = p.Users.Take(3).Select(u => new ProjecUserShortDto
                (
                    u.Id,
                    u.ProfilePicture            
                )).ToList(),
            }).ToListAsync();

        return Ok(projects);
    }

    public async Task<ProjectDetailsReadDto> GetProjectByIdAsync(int id)
    {
         var project = await _context.Projects
            .Include(p=>p.Users)
            .Include(p=>p.ProjectsUsers)
            .Select(p=>new ProjectDetailsReadDto()
            {
                Id = p.Id,
                Name = p.ProjectName,
                Description = p.ProjectDescription,
                StartDate = p.StartDateTime,
                EndDate = p.EndDateTime,
                UsersCount = p.Users.Count(),
                Users = p.Users.Select(u=>new ProjecUserLongDto
                (
                    u.Id,
                    u.DisplayName,
                    u.Handler,
                    u.ProfilePicture,
                    p.ProjectsUsers.First(up=>up.UserId == u.Id).IsMentor
                )).ToList()               
            })
            .FirstOrDefaultAsync(u=>u.Id == id);

        if (project is null)
        {
            return NotFound();
        }


        return Ok(project);
    }

    public async Task<int> CreateProjectAsync(ProjectCreateDto projectDto, string userId)
    {

         User? user = await userManager.GetUserAsync(User); // the variable User declared in the controller base class
        
        if(user is null) return NotFound(new ErrorResponse("User not found, this should not happen (Contact the backend developer to fix the bag)"));

        if(! user.EmailConfirmed) return BadRequest(new ErrorResponse("To create a project, your email must be verified"));

        var project = new Project
        {
            ProjectName = projectDto.Name,
            ProjectDescription = projectDto.Description,
            StartDateTime = projectDto.StartDate,
            EndDateTime = null,
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

    public async Task<bool> UpdateProjectAsync(int id, ProjectCreateDto projectDto)
    {
         var project = await _context.Projects.FindAsync(id);

        if (project is null) return NotFound();

        project.ProjectName = projectDto.Name;
        project.ProjectDescription = projectDto.Description;
        project.StartDateTime = projectDto.StartDate;

        await _context.SaveChangesAsync();

        return Ok(project);
    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
         var project = await _context.Projects.FindAsync(id);

        if (project is null) return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

