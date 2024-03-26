﻿namespace TeamUp_Backend.Features.Project
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
    using Microsoft.EntityFrameworkCore;
    using Users;

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
                var projects = _context.Projects
                    .Include(p=>p.Users)
                    .Select(p => new ProjectReadDto
                    {
                        Id = p.Id,
                        Name = p.ProjectName,
                        Description = p.ProjectDescription,
                        StartDate = p.StartDateTime,
                        EndDate = p.EndDateTime,
                        UsersCount = p.Users.Count(),
                        UsersSample = p.Users.Take(3).Select(u => new ProjecUserShortDto
                        (
                            u.Id,
                            u.ProfilePicture            
                        )).ToList(),
                    }).ToList();

                return Ok(projects);
            }

            
            [HttpGet("{id}")]
            public async Task<ActionResult<ProjectDetailsReadDto>> GetProject(int id)
            {
                var project = await _context.Projects
                    .Include(p=>p.Users)
                    .Select(p=>new ProjectDetailsReadDto()
                    {
                        Id = p.Id,
                        Name = p.ProjectName,
                        Description = p.ProjectDescription,
                        StartDate = p.StartDateTime,
                        EndDate = p.EndDateTime,
                        UsersCount = p.Users.Count(),
                        Users = p.Users.Select(u=>new ProjecUserLongDto
                        (
                           u.Id,
                           u.DisplayName,
                           u.Handler,
                           u.ProfilePicture
                        )).ToList()               
                    })
                    .FirstOrDefaultAsync(u=>u.Id == id);

                if (project is null)
                {
                    return NotFound();
                }


                return Ok(project);
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
                project.ProjectsUsers.Add(new UsersProject()
                {
                    User = user,
                    IsMentor = true
                });
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


