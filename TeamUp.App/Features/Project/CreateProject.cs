using Authentication.UserManager;
using Features.Projects.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
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
        User? user = await userManager.GetUserAsync(User);
        
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
    
}