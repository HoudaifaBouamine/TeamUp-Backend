

using Features.Projects;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories;
using Project = Models.Project;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectReadDto> GetByIdAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        return MapProjectToProjectReadDto(project!);
    }




public async Task<GetProjectsListResponse> GetAllAsync (int? PageSize,int? PageNumber, string? SearchPattern)
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

    return new GetProjectsListResponse
    (
        TotalCount : TotalCount,
        PageNumber : PageNumber??=1,
        PageSize : PageSize??=TotalCount,
        IsPrevPageExist : PageNumber > 1,
        IsNextPageExist : PageNumber * PageSize < TotalCount, 
        Projects: projectsDto
    );

}



    public async Task<int> CreateAsync(ProjectCreateDto projectDto,User user)
    {
        var project = new Project
        {
            Name = projectDto.Name,
            Description = projectDto.Description,
            StartDate = projectDto.StartDate,
            ChatRoom = new (),
            Users = [user]
        };

        project.ProjectsUsers.Add(new UsersProject
        {
            IsMentor = true,
            User = user
        });

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project.Id;
    }




    public async Task UpdateAsync(int id, ProjectCreateDto projectDto)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            project.Name = projectDto.Name;
            project.Description = projectDto.Description;
            project.StartDate = projectDto.StartDate;

            await _context.SaveChangesAsync();
        }
    }





    public async Task DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
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
}



