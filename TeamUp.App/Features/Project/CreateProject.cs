using System.Text.Json;
using Asp.Versioning;
using Authentication.UserManager;
using Features.Projects.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Org.BouncyCastle.Tls;
using Utils;

namespace Features.Projects;
partial class ProjectsController
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest,Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateProjectAsync(
        [FromBody] ProjectCreateDto projectDto,
        [FromServices] CustomUserManager userManager
    )
    {   
        var user = await userManager.GetUserAsync(User);
        
        if(user is null) 
            return BadRequest(new ErrorResponse("User account does not exist any more"));

        if (user.EmailConfirmed is false)
            return BadRequest(new ErrorResponse("User email is not confirmed"));

        var projectId = await _projectRepository.CreateAsync(projectDto,user);

        var projectReadDto = new ProjectReadDto
        (
            Id : projectId,
            Name: projectDto.Name,
            Description: projectDto.Description,
            StartDate: projectDto.StartDate,
            null,
            1,
            [new ProjecUserShortDto(user.Id, user.ProfilePicture)]
        );
        return CreatedAtAction(nameof(GetProject), new { id = projectId }, projectReadDto);
    }
    
    /// <summary>
    /// Start a project based on project post
    /// </summary>
    /// <param name="projectPostId"></param>
    /// <param name="userManager"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    [HttpPost("{projectPostId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest,Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiVersion(4)]
    public async Task<IActionResult> StartProjectAsync(
        [FromRoute] int projectPostId,
        [FromServices] CustomUserManager userManager,
        [FromServices] AppDbContext db
    )
    {
        Project obj = null;
        try
        {
            var user = await userManager.GetUserAsync(User);
        
            if(user is null) 
                return BadRequest(new ErrorResponse("User account does not exist any more"));
            //
            // if (user.EmailConfirmed is false)
            //     return BadRequest(new ErrorResponse("User email is not confirmed"));

            var projectPost = await db.ProjectPosts.
                Include(p=>p.Creator)
                .Include(p=>p.ProjectJoinRequests)
                .ThenInclude(pjr=>pjr.User)
                .Include(p => p.Project)
                .FirstOrDefaultAsync(p => p.Id == projectPostId);
        
            if (projectPost is null)
                return NotFound(new ErrorResponse($"Project post with id = {projectPostId} is not found"));

            projectPost.Start();
            obj = projectPost.Project;
            
            db.Projects.Add(projectPost.Project);
            db.ProjectPosts.Update(projectPost);

            await db.SaveChangesAsync();
        
            var projectReadDto = new ProjectDetailsReadDto(projectPost);
        
            return CreatedAtAction(nameof(GetProject), new { id = projectPost.Project.Id }, projectReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new ErrorResponse("Error is : " + e.Message + "\n" + JsonSerializer.Serialize(obj.Id)));
        }
        
    }
    
    
    public class ProjectDetailsReadDto
    {
        public int Id { get; init; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Scenario { get; set; }
        public string LearningGoals { get; set; }
        public string TeamAndRols { get; set; }
        public DateTime PostingTime { get; set; }
        public DateOnly StartingDate { get; set; }

        public string ProjectLevel { get; set; } = "Beginner";
        public string ExpectedDuration { get; set; }
        public int ExpectedTeamSize { get; set; }
        public List<string> Categories { get; set; } = [];
        public List<string> Skills { get; set; } = [];
        public MentorReadDto Mentor { get; set; } = null!;

        public ProjectDetailsReadDto(ProjectPost projectPost)
        {
            this.Id = projectPost.Id;
            this.Title = projectPost.Title;
            this.Summary = projectPost.Summary;
            this.Scenario = projectPost.Scenario;
            this.LearningGoals = projectPost.LearningGoals;
            this.TeamAndRols = projectPost.TeamAndRols;
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
            this.StartingDate = projectPost.Project.StartDate;
        }
    }

}

partial class ProjectRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="projectDto"></param>
    /// <param name="user"></param>
    /// <returns>Project Id</returns>
    public async Task<int> CreateAsync(ProjectCreateDto projectDto,User user)
    {
        var project = Project.Create
        (
            name : projectDto.Name,
            description : projectDto.Description,
            startDate : projectDto.StartDate,
            creator : user
        );

        _context.Projects.Add(project);

        await _context.SaveChangesAsync();
        return project.Id;
    }
    
    // public async Task<int> CreateAsync(ProjectPost projectPost,User user)
    // {
    //     var project = Project.Create
    //     (
    //         name : projectDto.Name,
    //         description : projectDto.Description,
    //         startDate : projectDto.StartDate,
    //         creator : user
    //     );
    //
    //     _context.Projects.Add(project);
    //
    //     await _context.SaveChangesAsync();
    //     return project.Id;
    // }
    
}