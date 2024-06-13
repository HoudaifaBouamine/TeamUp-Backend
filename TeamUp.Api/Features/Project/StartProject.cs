using System.Text.Json;
using Asp.Versioning;
using Authentication.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils;
using Project = Models.Project;

namespace TeamUp.Features.Project;

public partial class ProjectsController
{
    
    /// <summary>
    /// Start a project based on project post
    /// </summary>
    /// <param name="projectPostId"></param>
    /// <param name="userManager"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    [HttpPost("start/{projectPostId:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest,Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ApiVersion(4)]
    public async Task<IActionResult> StartProjectAsync(
        [FromRoute] int projectPostId,
        [FromServices] CustomUserManager userManager,
        [FromServices] AppDbContext db
    )
    {
        Models.Project obj = null;
        try
        {
            var user = await userManager.GetUserAsync(User);
        
            if(user is null) 
                return NotFound(new ErrorResponse("User account does not exist any more"));
            //
            // if (user.EmailConfirmed is false)
            //     return BadRequest(new ErrorResponse("User email is not confirmed"));

            var projectPostToVerify = (await db.ProjectPosts
                .Include(p => p.Creator)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == projectPostId));

            if(projectPostToVerify is null) 
                return NotFound(new ErrorResponse($"Project post is not found"));
            
            if (projectPostToVerify.Creator.Id != user.Id) 
                return Forbid();

            if (projectPostToVerify.IsStarted)
                return BadRequest(new ErrorResponse("Project already started"));
            
            var projectPost = await db.ProjectPosts.
                Include(p=>p.Creator)
                .Include(p=>p.ProjectJoinRequests)
                .ThenInclude(pjr=>pjr.User)
                .Include(p => p.Project)
                .FirstOrDefaultAsync(p => p.Id == projectPostId);
        
            if(projectPost is null) 
                return NotFound(new ErrorResponse($"Project post is not found"));
            
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
}