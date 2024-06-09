using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;

namespace TeamUp.Features.Project;

[Tags("Projects Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/projects")]
[ApiController]
public partial class ProjectsController(IProjectRepository projectRepo) : ControllerBase;

public partial class ProjectRepository(AppDbContext db) : IProjectRepository
{
    #region Utils
    private static ProjectReadDto MapProjectToProjectReadDto(Models.Project? project)
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
        return await db.UsersProjects
            .Where(pu => pu.ProjectId == projectId)
            .CountAsync();
    }





    public async Task<IEnumerable<ProjecUserShortDto>> GetUsersSampleAsync(int projectId)
    {
        var users = await db.UsersProjects
            .Include(pu=>pu.User)
            .Where(pu => pu.ProjectId == projectId)
            .Take(3)
            .Select(pu => new ProjecUserShortDto(
                pu.UserId,
                pu.User.ProfilePicture))
            .ToListAsync();

        return users;
    }






    public async Task<ProjectDetailsReadDto?> GetDetailsAsync(int projectId)
    {
        var project = await db.Projects
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