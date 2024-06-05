using Asp.Versioning;
using Features.Projects.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Models;
using Utils;
using Project = Models.Project;

namespace Features.Projects;

partial class ProjectsController
{
    
    [HttpGet("for-user")]
    [ApiVersion(4)]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(GetProjectsListResponse4))]
    public async Task<IActionResult> GetUserProjectsDetailsAsync(
        [FromQuery] Guid userId,
        [FromQuery] string? searchPattern,
        [FromQuery] int? pageSize = 10,
        [FromQuery] int? pageNumber = 1)
    {
        GetProjectsListResponse4 projects = await _projectRepository
            .GetUsersProjectsListWithSearchAndPaginationAsync(userId,pageSize, pageNumber, searchPattern);

        return Ok(projects);
    }
    
    [Authorize]
    [HttpGet("for-current-user")]
    [ApiVersion(4)]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(GetProjectsListResponse4))]
    public async Task<IActionResult> GetCurrentUserProjectsDetailsAsync(
        UserManager<User> userManager,
        [FromQuery] string? searchPattern,
        [FromQuery] int? pageSize = 10,
        [FromQuery] int? pageNumber = 1)
    {
        Guid userId = Guid.Parse(userManager.GetUserId(User)!);
        
        GetProjectsListResponse4 projects = await _projectRepository
            .GetUsersProjectsListWithSearchAndPaginationAsync(userId,pageSize, pageNumber, searchPattern);

        return Ok(projects);
    }
}

public partial class ProjectRepository
{
    public async Task<GetProjectsListResponse4> GetUsersProjectsListWithSearchAndPaginationAsync
        (Guid userId, int? pageSize, int? pageNumber, string? searchPattern)
    {
        if (pageSize is null) pageNumber = null;

        IQueryable<ProjectPost> projects = _context.ProjectPosts
            .Include(pp => pp.Project)
                .ThenInclude(p=>p!.ProjectsUsers)
            .Where(pp => pp.Project != null)
            .Where(pp=> pp.Project!.ProjectsUsers.Any(pu=>pu.UserId == userId));

        if (searchPattern is not null)
            projects = projects.Where(p =>
                p.Title.ToLower().Contains(searchPattern.ToLower()) ||
                p.Summary.ToLower().Contains(searchPattern.ToLower()) ||
                p.LearningGoals.ToLower().Contains(searchPattern.ToLower()) ||
                p.TeamAndRols.ToLower().Contains(searchPattern.ToLower()));

        int TotalCount = projects.Count();

        if (pageSize is not null && pageNumber is not null)
            projects = projects
                .Skip(pageSize.Value * (pageNumber.Value - 1))
                .Take(pageSize.Value);
        else if (pageSize is not null)
            projects = projects
                .Take(pageSize.Value);

        var projectsDto = await projects
            .Include(pp => pp.RequiredSkills)
            .Include(pp => pp.Categories)
            .Include(pp => pp.Project)
            .ThenInclude(p => p!.Users)
            .Select(p => new ProjectsController.ProjectDetailsReadDto(p)).ToListAsync();

        return new GetProjectsListResponse4
        (
            TotalCount: TotalCount,
            PageNumber: pageNumber ??= 1,
            PageSize: pageSize ??= TotalCount,
            IsPrevPageExist: pageNumber > 1,
            IsNextPageExist: pageNumber * pageSize < TotalCount,
            Projects: projectsDto
        );

    }
}