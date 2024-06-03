using Asp.Versioning;
using Features.Projects.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Models;
using Utils;
using Project = Models.Project;

namespace Features.Projects;

partial class ProjectsController
{
    
    [HttpGet]
    [ApiVersion(4)]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(GetProjectsListResponse4))]
    public async Task<IActionResult> GetProjectsDetailsAsync(
        [FromQuery] string? searchPattern,
        [FromQuery] int? pageSize = 10,
        [FromQuery] int? pageNumber = 1)
    {
        var projects = await _projectRepository
            .GetListWithSearchAndPagination4Async(pageSize, pageNumber, searchPattern);

        return Ok(projects);
    }


    [HttpGet]
    [ApiVersion(2)]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(GetProjectsListResponse2))]
    public async Task<IActionResult> GetProjects3Async(
        [FromQuery] string? searchPattern,
        [FromQuery] int? pageSize = 10,
        [FromQuery] int? pageNumber = 1)
    {
        var projects = await _projectRepository
            .GetListWithSearchAndPagination2Async(pageSize, pageNumber, searchPattern);

        return Ok(projects);
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(GetProjectsListResponse))]
    public async Task<IActionResult> GetProjectsAsync(
        [FromQuery] int? pageSize,
        [FromQuery] int? pageNumber,
        [FromQuery] string? searchPattern)
    {
        var projects = await _projectRepository
            .GetListWithSearchAndPaginationAsync(pageSize, pageNumber, searchPattern);

        return Ok(projects);
    }

    [HttpPost]
    [ApiVersion(2)]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(GetProjectsListResponse))]
    private async Task<IActionResult> GetProjectsV2Async(
        [FromQuery] int? PageSize,
        [FromQuery] int? PageNumber,
        [FromQuery] string? SearchPattern,
        [FromQuery] string[]? TeamSizes)
    {
        var projects = await _projectRepository
            .GetListWithFiltersAsync(PageSize, PageNumber, SearchPattern, TeamSizes, null, null);

            return Ok(projects);
    }

}


partial class ProjectRepository
{
    public async Task<GetProjectsListResponse4> GetListWithSearchAndPagination4Async (int? pageSize,int? pageNumber, string? searchPattern)
    {
        if(pageSize is null) pageNumber = null;

        IQueryable<ProjectPost> projects = _context.ProjectPosts
            .Include(p=>p.Project)
            .Where(p=>p.Project != null);

        if (searchPattern is not null)
            projects = projects.Where(p =>
                p.Title.ToLower().Contains(searchPattern.ToLower()) ||
                p.Summary.ToLower().Contains(searchPattern.ToLower()) ||
                p.LearningGoals.ToLower().Contains(searchPattern.ToLower()) ||
                p.TeamAndRols.ToLower().Contains(searchPattern.ToLower()));

        int TotalCount = projects.Count();

        if(pageSize is not null && pageNumber is not null)
            projects = projects
                .Skip(pageSize.Value * (pageNumber.Value -1))
                .Take(pageSize.Value);
        else if (pageSize is not null)
            projects = projects
                .Take(pageSize.Value);
        
        var projectsDto = await projects
            .Include(pp=>pp.RequiredSkills)
            .Include(pp=>pp.Categories)
            .Include(pp=>pp.Project)
                .ThenInclude(p=>p!.Users)
            .Select(p => new ProjectsController.ProjectDetailsReadDto(p)).ToListAsync();

        return new GetProjectsListResponse4
        (
            TotalCount : TotalCount,
            PageNumber : pageNumber??=1,
            PageSize : pageSize??=TotalCount,
            IsPrevPageExist : pageNumber > 1,
            IsNextPageExist : pageNumber * pageSize < TotalCount, 
            Projects: projectsDto
        );

    }


    
    
    /// <summary>
    /// get the list of projects, 
    /// </summary>
    /// <param name="PageSize">Max number of projects to return</param>
    /// <param name="PageNumber">The index of the page (if null is passed, return first page)</param>
    /// <param name="SearchPattern">Search on this pattern in the project title and description (not-case sensitive)</param>
    /// <returns></returns>
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

        /// <summary>
    /// get the list of projects, 
    /// </summary>
    /// <param name="pageSize">Max number of projects to return</param>
    /// <param name="pageNumber">The index of the page (if null is passed, return first page)</param>
    /// <param name="SearchPattern">Search on this pattern in the project title and description (not-case sensitive)</param>
    /// <returns></returns>
    public async Task<GetProjectsListResponse2> GetListWithSearchAndPagination2Async (int? pageSize,int? pageNumber, string? SearchPattern)
    {
        if(pageSize is null) pageNumber = null;

        IQueryable<Project> projects = _context.Projects;

        if(SearchPattern is not null)
            projects = projects.Where(p=>
                p.Description.ToLower().Contains(SearchPattern.ToLower()) ||
                p.Name.ToLower().Contains(SearchPattern.ToLower()));

        int TotalCount = projects.Count();

        if(pageSize is not null && pageNumber is not null)
            projects = projects
                .Skip(pageSize.Value * (pageNumber.Value -1))
                .Take(pageSize.Value);
        else if (pageSize is not null)
            projects = projects
                .Take(pageSize.Value);

        List<string> skillsList = ["Not Implimented", "Not Implimented", "Not Implimented"];

        var projectsDto = await projects
            .Include(p=>p.Users)
            .Select(p => new ProjectReadDto2
            (
                p.Id,
                p.Name,
                p.Description,
                "Scenarios : Not Implimented",
                "Learning Goalds : Not Implimented",
                "Team And Rols : Not Implimented",
                skillsList,
                p.StartDate,
                p.EndDate,
                p.Users.Count(),
                p.Users.Take(3).Select(u => new ProjecUserShortDto
                (
                    u.Id,
                    u.ProfilePicture            
                )).ToList()
            )).ToListAsync();

        return new GetProjectsListResponse2
        (
            TotalCount : TotalCount,
            PageNumber : pageNumber??=1,
            PageSize : pageSize??=TotalCount,
            IsPrevPageExist : pageNumber > 1,
            IsNextPageExist : pageNumber * pageSize < TotalCount, 
            Projects: projectsDto
        );

    }


    /// <summary>
    /// get the list of projects, 
    /// </summary>
    /// <param name="PageSize">Max number of projects to return</param>
    /// <param name="PageNumber">The index of the page (if null is passed, return first page)</param>
    /// <param name="SearchPattern">Search on this pattern in the project title and description (not-case sensitive)</param>
    /// <param name="Durations"></param>
    /// <returns></returns>
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


}