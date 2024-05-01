using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Mentor;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utils;
namespace Features;


///// Planning
///////////////
// Get Project's posts list, with pagination and search
// Add new post, required authenticated user, verified email
// send join request, will be notified to the publicher to accept or reject the request
// get project join requests list, with pagination and search
// get project join request by id,
// response to project join request

[Tags("Project Posts Group")]
[ApiVersion(4)]
[Route("api/v{v:apiVersion}/projects-posts")]
[ApiController]
public class ProjectPostEndpoints(AppDbContext db, UserManager<User> userManager) : ControllerBase
{
    private readonly AppDbContext db = db;
    private readonly UserManager<User> userManager = userManager;

    [HttpPost]
    public async Task<IActionResult> CreateProjectPostAsync([FromBody] ProjectPostCreateDto postDto)
    {
        var currentUser = await userManager.GetUserAsync(User);

        if(currentUser is null) return Unauthorized(new ErrorResponse("User account does not exist any more"));

        var skills = await db.Skills.Where(s=>postDto.RequiredSkills.Contains(s.Name)).ToListAsync();

            var post = new ProjectPost
            (
                creator : currentUser,
                title : postDto.Title,
                summary : postDto.Summary,
                expextedDuration : new TimeSpan(postDto.ExpectedDurationInDays,0,0,0),
                expectedTeamSize : postDto.ExpectedTeamSize,
                scenario : postDto.Scenario,
                learningGoals : postDto.LearningGoals,
                teamAndRols : postDto.TeamAndRols,
                skills: skills
            );

            await db.ProjectPosts.AddAsync(post);
            await db.SaveChangesAsync();
            return Ok(new ProjectPostReadDto(post));
        }   
        
        public class ProjectPostCreateDto
        {
            [Required]
            public string Title { get; set; } = string.Empty;

            [Required]
            public string Summary { get; set; } = string.Empty;

            [Required]
            public string Scenario { get; set; } = string.Empty;

            [Required]
            public string LearningGoals { get; set; } = string.Empty;

        [Required]
        public string TeamAndRols { get; set; } = string.Empty;

        [Required]
        public int ExpectedTeamSize { get; set; }

        [Required]
        public int ExpectedDurationInDays { get; set; }

        [Required]
        public List<string> RequiredSkills { get; set;}  = [];
    }

















    [HttpGet]
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
        public string Scenario { get; set; } = string.Empty;
        public string LearningGoals { get; set; } = string.Empty;
        public string TeamAndRols { get; set; } = string.Empty;
        public List<string> RequiredSkills { get; set; } = [];
        public TimeSpan ExpextedDuration { get; set; } 
        public int ExpectedTeamSize { get; set; }
        public MentorReadDto Mentor { get; set; } = null!;    

        public ProjectPostReadDto(ProjectPost projectPost)
        {
            this.Id = projectPost.Id;
            this.Title = projectPost.Title;
            this.Summary = projectPost.Summary;
            this.Scenario = projectPost.Scenario;
            this.LearningGoals = projectPost.LearningGoals;
            this.TeamAndRols = projectPost.TeamAndRols;
            this.RequiredSkills = projectPost.RequiredSkills.Select(s=>s.Name).ToList();
            this.ExpextedDuration = projectPost.ExpextedDuration;
            this.ExpectedTeamSize = projectPost.ExpectedTeamSize;
            this.Mentor = new MentorReadDto(projectPost.Creator.Id,
                                            projectPost.Creator.Email!,
                                            projectPost.Creator.DisplayName,
                                            projectPost.Creator.Handler,
                                            projectPost.Creator.Rate,
                                            projectPost.Creator.ProfilePicture!);
        }
    }
}
