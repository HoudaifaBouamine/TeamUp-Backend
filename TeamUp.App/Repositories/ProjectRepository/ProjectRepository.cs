

using System.Linq.Expressions;
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

    public async Task<GetProjectsListResponse> GetListWithSearchAndPaginationAsync (int? PageSize,int? PageNumber, string? SearchPattern)
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
                p.Users.Count(),
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


public static class ExpressionGenerator
{
    public static Expression<Func<Project, bool>> GenerateTeamSizeFilterExpression(string[] filters)
    {
        var parameter = Expression.Parameter(typeof(Project), "p");
        var property = Expression.Property(parameter, "TeamSize");

        Expression filterExpression = null;

        foreach (var filter in filters)
        {
            var range = filter.Split('-');
            if (range.Length == 1)
            {
                int minValue = int.Parse(range[0].Trim('+'));
                var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, Expression.Constant(minValue));
                filterExpression = filterExpression == null ? greaterThanOrEqual : Expression.OrElse(filterExpression, greaterThanOrEqual);
            }
            else if (range.Length == 2)
            {
                int minValue = int.Parse(range[0]);
                int maxValue = int.Parse(range[1]);
                var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, Expression.Constant(minValue));
                var lessThanOrEqual = Expression.LessThanOrEqual(property, Expression.Constant(maxValue));
                var rangeExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
                filterExpression = filterExpression == null ? rangeExpression : Expression.OrElse(filterExpression, rangeExpression);
            }
            else
            {
                throw new ArgumentException("Invalid filter range format.");
            }
        }

        if (filterExpression == null)
        {
            throw new ArgumentException("At least one valid filter range is required.");
        }

        var lambda = Expression.Lambda<Func<Project, bool>>(filterExpression, parameter);
        return lambda;
    }

}




    public async Task<GetProjectsListResponse> GetListWithFiltersAsync(int? PageSize, int? PageNumber, string? SearchPattern, string[]? TeamSizes, string[]? Categories, string[]? Durations)
    {
        if(PageSize is null) PageNumber = null;

        IQueryable<Project> projects = _context.Projects;

        if(TeamSizes is not null)
        {        
            var filterExpression = ExpressionGenerator.GenerateTeamSizeFilterExpression(TeamSizes);
            projects = projects.Where(filterExpression);
        }

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
            .Include(p=>p.ProjectsUsers)
            .Include(p=>p.Users)    
            .Select(p => new ProjectReadDto
            (
                p.Id,
                p.Name,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.ProjectsUsers.Count(),
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
        project.TeamSize = 1;

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project.Id;
    }


    public async Task<bool> AddUserToProjectAsync(int projectId, Guid userId, bool isMentor)
    {
        var project = await _context.Projects
            .Include(p => p.ProjectsUsers)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project is not null)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());

            if (user != null)
            {
                project.ProjectsUsers.Add(new UsersProject
                {
                    User = user,
                    IsMentor = isMentor
                });
                project.TeamSize++;

                await _context.SaveChangesAsync();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Add list of users to a project, returning the number of users added successfuly
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="userIds"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="userIds"></param>
    /// <returns></returns>
    public async Task<int> AddUsersToProjectAsync(int projectId, List<Guid> userIds)
    {
        var project = await _context.Projects
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if(project is null) return -1;

        var users = await _context.Users
            .Where(u => userIds.Contains(Guid.Parse(u.Id)))
            .ToListAsync();

        var prevUsersCount = project.Users.Count();
        project.Users.AddRange(users);
        var newUsersCount = project.Users.Count();
        project.TeamSize = newUsersCount;

        await _context.SaveChangesAsync();
        
        return newUsersCount - prevUsersCount;
    }

    public async Task<bool> UpdateAsync(int id, ProjectCreateDto projectDto)
    {
        var project = await _context.Projects.FindAsync(id);
        
        if (project is not null)
        {
            project.Name = projectDto.Name;
            project.Description = projectDto.Description;
            project.StartDate = projectDto.StartDate;

            await _context.SaveChangesAsync();
            return true;
        }

        return false;
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



