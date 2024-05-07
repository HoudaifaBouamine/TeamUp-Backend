using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Mentor;
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


    [HttpPost("join-requests/{requestId}")]
    public async Task<IActionResult> AnswerProjectJoinRequestAsync([FromRoute] int requestId,
        [FromQuery] [AllowedValues("accept", "reject")] string answer)
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

        if (answer == "accept") request.Accept();
        if (answer == "reject") request.Refuse();

        return Ok(new ProjectJoinRequestReadDto(request));
    }



    [HttpGet("join-requests")]
    public async Task<IActionResult> GetProjectJoinRequestsAsync([FromQuery] int ProjectPostId)
    {
        Guid id;

        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser is null) return Unauthorized(new ErrorResponse("User account does not exist any more"));
            id = currentUser.Id;
        }

        var post = await db.ProjectPosts
            .Where(p => p.Id == ProjectPostId)
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
















    [HttpPost("join-requests")]
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

    [HttpGet("join-requests/{requestId}")]
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





















    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectPostReadDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> CreateProjectPostAsync([FromBody] ProjectPostCreateDto postDto)
    {
        var currentUser = await userManager.GetUserAsync(User);

        if (currentUser is null) return Unauthorized(new ErrorResponse("User account does not exist any more"));

        var skills = await db.Skills.Where(s => postDto.RequiredSkills.Contains(s.Name)).ToListAsync();
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









    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectPostDetailsReadDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetProjectPostsByIdAsync(int id)
    {
        var project = await db.ProjectPosts.Include(p=>p.Creator).FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return NotFound(new ErrorResponse("Project not found"));

        return Ok(new ProjectPostDetailsReadDto(project));
    }




    [HttpGet]
    [ProducesResponseType( StatusCodes.Status200OK, Type = typeof(GetProjectPostListResponse) )]
    public async Task<IActionResult> GetProjectPostsAsync(string? SearchPattern, int PageSize = 10, int PageNumber = 1)
        {
        IQueryable<ProjectPost> posts = db.ProjectPosts;

        if(SearchPattern is not null)
            posts = posts.Where(p=>
                p.Summary.ToLower().Contains(SearchPattern.ToLower()) ||
                p.LearningGoals.ToLower().Contains(SearchPattern.ToLower()));

        int TotalCount = posts.Count();

        posts = posts
            .Skip(PageSize * (PageNumber - 1))
            .Take(PageSize);
    

        var projectsDto = await posts
            .Include(p=>p.Creator)
            .Include(p=>p.RequiredSkills)
            .Select(p => new ProjectPostReadDto(p)).ToListAsync();

        return Ok(new GetProjectPostListResponse
        {
            TotalCount = TotalCount,
            PageNumber = PageNumber,
            PageSize = PageSize,
            IsPrevPageExist = PageNumber > 1,
            IsNextPageExist = PageNumber * PageSize < TotalCount, 
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
        public List<string> Categories { get; set; } = [];
        public MentorReadDto Mentor { get; set; } = null!;
        
        public ProjectPostReadDto()
        {
            
        }
        public ProjectPostReadDto(ProjectPost projectPost)
        {
            this.Id = projectPost.Id;
            this.Title = projectPost.Title;
            this.Summary = projectPost.Summary;
            this.Categories = projectPost.Categories.Select(s => s.Name).ToList();
            
            this.Mentor = new MentorReadDto(projectPost.Creator.Id,
                projectPost.Creator.DisplayName,
                projectPost.Creator.Handler,
                projectPost.Creator.Rate,
                projectPost.Creator.ProfilePicture!);
        }
    }

    public class ProjectPostDetailsReadDto
    {
        public int Id { get; init; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Scenario { get; set; } = string.Empty;
        public string LearningGoals { get; set; } = string.Empty;
        public string TeamAndRols { get; set; } = string.Empty;
        public List<string> RequiredSkills { get; set; } = [];
        public List<string> Categories { get; set; } = [];
        public string ExpextedDuration { get; set; } 
        public int ExpectedTeamSize { get; set; }
        public MentorReadDto Mentor { get; set; } = null!;

        public ProjectPostDetailsReadDto()
        {
            
        }
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
            this.ExpextedDuration = projectPost.ExpextedDuration;
            this.ExpectedTeamSize = projectPost.ExpectedTeamSize;
            this.Mentor = new MentorReadDto(projectPost.Creator.Id,
                                            projectPost.Creator.DisplayName,
                                            projectPost.Creator.Handler,
                                            projectPost.Creator.Rate,
                                            projectPost.Creator.ProfilePicture!);
        }
    }
}
