namespace TeamUp_Backend.Features.Project
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using TeamUp_Backend.Features.Project;
    using Models;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using System.Security.Claims;
    using Authentication.UserManager;

    [Tags("Projects Group")]
        [ApiVersion(1)]
        [Route("api/v{v:apiVersion}/[controller]")]
        [ApiController]
        public class ProjectsController : ControllerBase
        {
            private readonly AppDbContext  _context;

            public ProjectsController(AppDbContext context)
            {
                _context = context;
            }
            
            [HttpGet]
            public ActionResult<IEnumerable<ProjectReadDto>> GetProjects()    
            {
                var projects = _context.Projects.Select(p => new ProjectReadDto
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    StartDateTime = p.StartDateTime,
                    EndDateTime = p.EndDateTime,
                }).ToList();

                return Ok(projects);
            }

            
            [HttpGet("{id}")]
            public ActionResult<ProjectReadDto> GetProject(int id)
            {
                var project = _context.Projects.Find(id);

                if (project == null)
                {
                    return NotFound();
                }

                var projectDto = new ProjectReadDto
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    ProjectDescription = project.ProjectDescription,
                    StartDateTime = project.StartDateTime,
                    EndDateTime = project.EndDateTime,
                };

                return Ok(projectDto);
            }

            
            [HttpPost]
            [Authorize]
            public async Task<ActionResult<ProjectCreateDto>> CreateProject(
                [FromBody] ProjectCreateDto projectDto,
                [FromServices] CustomUserManager userManager)
            {
                User? user = await userManager.GetUserAsync(User); // the variable User declared in the controller base class
                
                if(user is null) return NotFound(new ErrorResponse("User not found, this should not happen (Contact the backend developer to fix the bag)"));

                if(! user.EmailConfirmed) return BadRequest(new ErrorResponse("To create a project, your email must be verified"));

                var project = new Project
                {
                    ProjectName = projectDto.ProjectName,
                    ProjectDescription = projectDto.ProjectDescription,
                    StartDateTime = projectDto.StartDateTime,
                    EndDateTime = null,
                    ChatRoom = new ChatRoom(),
                    Users = [user]
                };

                _context.Projects.Add(project);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, projectDto);
            }

            [HttpPut("{id}")]
            public IActionResult UpdateProject(int id, ProjectCreateDto projectDto)
            {

                var project = _context.Projects.Find(id);

                if (project is null) return NotFound();

                project.ProjectName = projectDto.ProjectName;
                project.ProjectDescription = projectDto.ProjectDescription;
                project.StartDateTime = projectDto.StartDateTime;

                _context.SaveChanges();

                return NoContent();
            }

            
            [HttpDelete("{id}")]
            public IActionResult DeleteProject(int id)
            {
                var project = _context.Projects.Find(id);

                if (project is null) return NotFound();

                _context.Projects.Remove(project);
                _context.SaveChanges();

                return NoContent();
            }
        }
    }


