using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utils;

namespace Features;

[Tags("Project Posts Group")]
[ApiVersion(4)]
[Route("api/v{v:apiVersion}/projects-posts")]
[ApiController]
public class ProjectPostEndpoints(AppDbContext db, UserManager<User> userManager) : ControllerBase
{
    private readonly AppDbContext db = db;
    private readonly UserManager<User> userManager = userManager;

    /// <summary>
    /// answer a join request
    /// </summary>
    /// <param name="requestId">request'id to be answered</param>
    /// <param name="isAccepted">send true, if request is accepted, otherwise send false</param>
    /// <returns></returns>
    [HttpPost("join-requests/answer")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectJoinRequestReadDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> AnswerProjectJoinRequestAsync(
        [FromQuery][Required] int requestId,
        [FromQuery][Required] bool isAccepted = true)
    {

        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is null) return Unauthorized(new ErrorResponse("User account does not exist any more"));

        var request = await db.ProjectJoinRequests
            .Where(r => r.Id == requestId)
            .Include(r => r.ProjectPost)
            .Include(r => r.ProjectPost.Creator)
            .Where(r => r.ProjectPost.Creator.Id == currentUser.Id)
            .FirstOrDefaultAsync();

        if (request is null)
            return NotFound(new ErrorResponse("Request not found or can not be answered by the current user"));

        if (isAccepted)
            request.Accept();

        request.Refuse();
        
        await db.SaveChangesAsync();
        
        return Ok(new ProjectJoinRequestReadDto(request));
    }
    
    /// <summary>
    /// Get the statuses of all join requests related to a specific project
    /// </summary>
    /// <param name="projectPostId"></param>
    /// <returns></returns>
    [HttpGet("join-requests")]
    [ProducesResponseType< List<ProjectJoinRequestReadDto> >(StatusCodes.Status200OK)]
    [ProducesResponseType( StatusCodes.Status401Unauthorized,Type = typeof(ErrorResponse))]
    [ProducesResponseType( StatusCodes.Status404NotFound,Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetProjectJoinRequestsAsync([FromQuery][Required] int projectPostId)
    {
        Guid id;

        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser is null) return Unauthorized(new ErrorResponse("User account does not exist any more"));
            id = currentUser.Id;
        }

        var post = await db.ProjectPosts
            .Where(p => p.Id == projectPostId)
            .Include(p => p.Creator)
            .Where(p => p.Creator.Id == id)
            .Include(p => p.ProjectJoinRequests)
            .FirstOrDefaultAsync();

        if (post is null) return NotFound(new ErrorResponse("Project not found"));

        return Ok(post.ProjectJoinRequests.Select(r => new ProjectJoinRequestReadDto(r)));
    }

    //// No Need For Pagination Now
    // // create pagination class named GetProjectJoinRequestListReadDto
    // public class GetProjectJoinRequestListReadDto
    // {
    //     public int TotalCount { get; set; }
    //     public int PageNumber { get; set; }
    //     public int PageSize { get; set; }
    //     public bool IsPrevPageExist { get; set; }
    //     public bool IsNextPageExist { get; set; }

    //     public List<ProjectJoinRequestReadDto> JoinRequests { get; set; } = [];

    // }















    /// <summary>
    /// Creating join request to a project
    /// </summary>
    /// <param name="joinRequestDto"></param>
    /// <returns></returns>
    [HttpPost("join-requests")]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(ProjectJoinRequestReadDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> CreateProjectJoinRequestAsync(
        [FromBody] ProjectJoinRequestCreateDto joinRequestDto)
    {
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is null) return Unauthorized(new ErrorResponse("User account does not exist any more"));

        var post = await db.ProjectPosts
            .Include(p => p.Creator)
            .Include(p => p.Project)
            .FirstOrDefaultAsync(p => p.Id == joinRequestDto.ProjectPostId);

        if (post is null) return NotFound(new ErrorResponse("Project not found"));

        var joinRequest = ProjectJoinRequest.Create(currentUser, post);

        db.ProjectJoinRequests.Add(joinRequest);
        await db.SaveChangesAsync();

        return Ok(new ProjectJoinRequestReadDto(joinRequest));
    }

    /// <summary>
    /// Get single project join request by id
    /// </summary>
    /// <param name="requestId">join request's id</param>
    /// <returns></returns>
    [HttpGet("join-requests/{requestId}")]
    [ProducesResponseType<ProjectJoinRequestReadDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectJoinRequestAsync([FromRoute] int requestId)
    {
        var request = await db.ProjectJoinRequests
            .Where(r => r.Id == requestId)
            .Include(r => r.ProjectPost)
            .Include(r => r.ProjectPost.Creator)
            .FirstOrDefaultAsync();

        if (request is null) return NotFound(new ErrorResponse("Request not found"));
        return Ok(new ProjectJoinRequestReadDto(request));
    }

    public record ProjectJoinRequestCreateDto(int ProjectPostId, string Message);

    public class ProjectJoinRequestReadDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsClosed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondAt { get; set; }

        public ProjectJoinRequestReadDto(ProjectJoinRequest joinRequest)
        {
            Id = joinRequest.Id;
            ProjectId = joinRequest.ProjectPost.Id;
            IsAccepted = joinRequest.IsAccepted;
            IsClosed = joinRequest.IsClosed;
            CreatedAt = joinRequest.CreatedAt;
            RespondAt = joinRequest.RespondAt;
        }
    }





















    /// <summary>
    /// Create project post
    /// </summary>
    /// <param name="postDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectPostReadDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> CreateProjectPostAsync([FromBody] ProjectPostCreateDto postDto)
    {
        var currentUser = await userManager.GetUserAsync(User);

        if (currentUser is null) return Unauthorized(new ErrorResponse("User account does not exist any more"));

        var skills = await db.Skills.Where(s => postDto.RequiredSkills.Contains(s.Name)).ToListAsync();

        var newSkills = postDto.RequiredSkills
            .Where(rs => skills.All(s => s.Name != rs))
            .Select(s=>new Skill {Name = s});
        
        await db.Skills.AddRangeAsync(newSkills);
        await db.SaveChangesAsync();
        skills = await db.Skills.Where(s => postDto.RequiredSkills.Contains(s.Name)).ToListAsync();
        
        var categories = await db.Categories.Where(s => postDto.Categories.Contains(s.Name)).ToListAsync();

        var post = new ProjectPost
        (
            creator: currentUser,
            title: postDto.Title,
            summary: postDto.Summary,
            expextedDuration: postDto.ExpectedDuration,
            expectedTeamSize: postDto.ExpectedTeamSize,
            scenario: postDto.Scenario,
            learningGoals: postDto.LearningGoals,
            teamAndRoles: postDto.TeamAndRoles,
            skills: skills,
            categories: categories
        );

        await db.ProjectPosts.AddAsync(post);
        await db.SaveChangesAsync();
        return Ok(new ProjectPostReadDto(post));
    }

    public record ProjectPostCreateDto(
        string Title,
        string Summary,
        string Scenario,
        string LearningGoals,
        string TeamAndRoles,
        int ExpectedTeamSize,
        [AllowedValues("1 Week", "2-3 Weeks", "1 Month", "2-3 Months", "+3 Months")]
        string ExpectedDuration,
        List<string> RequiredSkills,
        List<string> Categories);









    /// <summary>
    /// Get single project post by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectPostDetailsReadDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetProjectPostsByIdAsync(int id)
    {
        var project = await db.ProjectPosts.Include(p=>p.RequiredSkills).Include(p=>p.Categories).Include(p=>p.Creator).FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return NotFound(new ErrorResponse("Project not found"));

        return Ok(new ProjectPostDetailsReadDto(project));
    }



    /// <summary>
    /// get the list of available project posts for specific user
    /// </summary>
    /// <param name="id">id of project's creator</param>
    /// <param name="searchPattern">pattern to search for, not case sensitive</param>
    /// <param name="pageSize">number of project posts to be returned in the response</param>
    /// <param name="pageNumber">index of the page</param>
    /// <returns></returns>
    [HttpGet("for-user/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectPostListResponse) )]
    public async Task<IActionResult> GetProjectPostsAsync(Guid id, string? searchPattern, int pageSize = 10, int pageNumber = 1)
    {
        IQueryable<ProjectPost> posts = db.ProjectPosts.Where(p=>p.CreatorId == id);

        if(searchPattern is not null)
            posts = posts.Where(p=>
                p.Summary.ToLower().Contains(searchPattern.ToLower()) ||
                p.LearningGoals.ToLower().Contains(searchPattern.ToLower()));

        int TotalCount = posts.Count();

        posts = posts
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);
    

        var projectsDto = await posts
            .Include(p=>p.Creator)
            .Include(p=>p.RequiredSkills)
            .OrderByDescending(p=>p.PostingTime)
            .Select(p => new ProjectPostReadDto(p)).ToListAsync();

        return Ok(new GetProjectPostListResponse
        {
            TotalCount = TotalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            IsPrevPageExist = pageNumber > 1,
            IsNextPageExist = pageNumber * pageSize < TotalCount, 
            ProjectsPosts = projectsDto
        });

    }


    /// <summary>
    /// get the list of all available project posts
    /// </summary>
    /// <param name="searchPattern">pattern to search for, not case sensitive</param>
    /// <param name="pageSize">number of project posts to be returned in the response</param>
    /// <param name="pageNumber">index of the page</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof(GetProjectPostListResponse) )]
    public async Task<IActionResult> GetProjectPostsAsync(string? searchPattern, int pageSize = 10, int pageNumber = 1)
        {
        IQueryable<ProjectPost> posts = db.ProjectPosts;

        if(searchPattern is not null)
            posts = posts.Where(p=>
                p.Summary.ToLower().Contains(searchPattern.ToLower()) ||
                p.LearningGoals.ToLower().Contains(searchPattern.ToLower()));

        int TotalCount = posts.Count();

        posts = posts
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);
    

        var projectsDto = await posts
            .Include(p=>p.Creator)
            .Include(p=>p.RequiredSkills)
            .Include(p=>p.Categories)
            .OrderByDescending(p=>p.PostingTime)
            .Select(p => new ProjectPostReadDto(p)).ToListAsync();

        return Ok(new GetProjectPostListResponse
        {
            TotalCount = TotalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            IsPrevPageExist = pageNumber > 1,
            IsNextPageExist = pageNumber * pageSize < TotalCount, 
            ProjectsPosts = projectsDto
        });

    }


    public class GetProjectPostListResponse
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool IsPrevPageExist { get; set; }
        public bool IsNextPageExist { get; set; }
        public List<ProjectPostReadDto> ProjectsPosts { get; set; } = [];
    }
    
    public class ProjectPostReadDto
    {
        public int Id { get; init; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public DateTime PostingTime { get; set; }

        public string ProjectLevel { get; set; } = "Beginner";
        public string ExpectedDuration { get; set; }
        public int ExpectedTeamSize { get; set; }
        public List<string> Categories { get; set; } = [];
        public List<string> Skills { get; set; } = [];
        public MentorReadDto Mentor { get; set; } = null!;

        public ProjectPostReadDto(ProjectPost projectPost)
        {
            this.Id = projectPost.Id;
            this.Title = projectPost.Title;
            this.Summary = projectPost.Summary;
            this.Categories = projectPost.Categories.Select(s => s.Name).ToList();
            this.Skills = projectPost.RequiredSkills.Select(s => s.Name).ToList();
            this.PostingTime = projectPost.PostingTime;
            this.ExpectedDuration = projectPost.ExpextedDuration;
            this.ExpectedTeamSize = projectPost.ExpectedTeamSize;
            this.Mentor = new MentorReadDto(projectPost.Creator.Id,
                projectPost.Creator.DisplayName,
                projectPost.Creator.Handler,
                projectPost.Creator.Rate,
                projectPost.Creator.ProfilePicture!);
        }
    }

    class ProjectPostDetailsReadDto
    {
        public int Id { get; init; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Scenario { get; set; } = string.Empty;
        public string LearningGoals { get; set; } = string.Empty;
        public string TeamAndRols { get; set; } = string.Empty;
        public string ProjectLevel { get; set; } = "Beginner";
        public string ExpectedDuration { get; set; }
        public int ExpectedTeamSize { get; set; }
        public DateTime PostingTime { get; set; }
        public List<string> Categories { get; set; } = [];
        public List<string> RequiredSkills { get; set; } = [];
        public MentorReadDto Mentor { get; set; } = null!;
        public ProjectPostDetailsReadDto(ProjectPost projectPost)
        {
            this.Id = projectPost.Id;
            this.Title = projectPost.Title;
            this.Summary = projectPost.Summary;
            this.Scenario = projectPost.Scenario;
            this.LearningGoals = projectPost.LearningGoals;
            this.TeamAndRols = projectPost.TeamAndRols;
            this.RequiredSkills = projectPost.RequiredSkills.Select(s=>s.Name).ToList();
            this.Categories = projectPost.Categories.Select(s => s.Name).ToList();
            this.ExpectedDuration = projectPost.ExpextedDuration;
            this.ExpectedTeamSize = projectPost.ExpectedTeamSize;
            this.PostingTime = projectPost.PostingTime;

            this.Mentor = new MentorReadDto(projectPost.Creator.Id,
                                            projectPost.Creator.DisplayName,
                                            projectPost.Creator.Handler,
                                            projectPost.Creator.Rate,
                                            projectPost.Creator.ProfilePicture!);
        }
    }
}
