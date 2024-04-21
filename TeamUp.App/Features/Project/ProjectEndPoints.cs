using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Features.Projects.Contracts;
using Models;
using Microsoft.EntityFrameworkCore;
namespace Features.Projects;

[Tags("Projects Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/projects")]
[ApiController]
public partial class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;

    public ProjectsController(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }
}

public partial class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    #region Utils
    private ProjectReadDto MapProjectToProjectReadDto(Project project)
    {
        if (project == null)
            return null!;

        return new ProjectReadDto
        (
            Id : project.Id,
            Name : project.Name,
            Description : project.Description,
            StartDate : project.StartDate,
            EndDate : project.EndDate,
            UsersCount : project.Users.Count(),
            UsersSample : project.Users
                .Take(3)
                .Select(u => new ProjecUserShortDto
                (
                    Id : u.Id,
                    ProfilePicture : u.ProfilePicture
                ))
                .ToList()
        );
    }


    public async Task<int> GetUsersCountAsync(int projectId)
    {
        return await _context.UsersProjects
            .Where(pu => pu.ProjectId == projectId)
            .CountAsync();
    }





    public async Task<IEnumerable<ProjecUserShortDto>> GetUsersSampleAsync(int projectId)
    {
        var users = await _context.UsersProjects
            .Include(pu=>pu.User)
            .Where(pu => pu.ProjectId == projectId)
            .Take(3)
            .Select(pu => new ProjecUserShortDto(
                pu.UserId,
                pu.User.ProfilePicture))
            .ToListAsync();

        return users;
    }






    public async Task<ProjectDetailsReadDto> GetDetailsAsync(int projectId)
    {
        var project = await _context.Projects
            .Include(p => p.Users)
            .Include(p => p.ProjectsUsers)
            .Where(p => p.Id == projectId)
            .Select(p => new ProjectDetailsReadDto
            (
                p.Id,
                p.Name,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.Users.Count(),
                p.Users.Select(u => new ProjecUserLongDto
                (
                    u.Id,
                    u.DisplayName,
                    u.Handler,
                    u.ProfilePicture,
                    p.ProjectsUsers.First(up => up.UserId == u.Id).IsMentor 
                )).ToList()
            ))
            .FirstOrDefaultAsync();

        return project!;
    }


    #endregion
 
}