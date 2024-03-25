namespace TeamUp_Backend.Features.Project
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using TeamUp_Backend.Features.Project;
    using Models;
  
    
        [Route("api/[controller]")]
        [ApiController]
        public class ProjectsController : ControllerBase
        {
            private readonly AppDbContext  _context;

            public ProjectsController(AppDbContext context)
            {
                _context = context;
            }
            
            [HttpGet]
            public ActionResult<IEnumerable<ProjectDto>> GetProjects()    
            {
                var projects = _context.Projects.Select(p => new ProjectDto
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    ProjectDescription = p.ProjectDescription,
                    StartDateTime = p.StartDateTime,
                    EndDateTime = p.EndDateTime,
                    ChatRoomId = p.ChatRoomId
                }).ToList();

                return Ok(projects);
            }

            
            [HttpGet("{id}")]
            public ActionResult<ProjectDto> GetProject(int id)
            {
                var project = _context.Projects.Find(id);

                if (project == null)
                {
                    return NotFound();
                }

                var projectDto = new ProjectDto
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    ProjectDescription = project.ProjectDescription,
                    StartDateTime = project.StartDateTime,
                    EndDateTime = project.EndDateTime,
                    ChatRoomId = project.ChatRoomId
                };

                return Ok(projectDto);
            }

            
            [HttpPost]
            public ActionResult<ProjectDto> CreateProject(ProjectDto projectDto)
            {
                var project = new Project
                {
                    ProjectName = projectDto.ProjectName,
                    ProjectDescription = projectDto.ProjectDescription,
                    StartDateTime = projectDto.StartDateTime,
                    EndDateTime = projectDto.EndDateTime,
                    ChatRoomId = projectDto.ChatRoomId
                };

                _context.Projects.Add(project);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, projectDto);
            }

            [HttpPut("{id}")]
            public IActionResult UpdateProject(int id, ProjectDto projectDto)
            {
                if (id != projectDto.Id)
                {
                    return BadRequest();
                }

                var project = _context.Projects.Find(id);
                if (project == null)
                {
                    return NotFound();
                }

                project.ProjectName = projectDto.ProjectName;
                project.ProjectDescription = projectDto.ProjectDescription;
                project.StartDateTime = projectDto.StartDateTime;
                project.EndDateTime = projectDto.EndDateTime;
                project.ChatRoomId = projectDto.ChatRoomId;

                _context.SaveChanges();

                return NoContent();
            }

            
            [HttpDelete("{id}")]
            public IActionResult DeleteProject(int id)
            {
                var project = _context.Projects.Find(id);
                if (project == null)
                {
                    return NotFound();
                }

                _context.Projects.Remove(project);
                _context.SaveChanges();

                return NoContent();
            }
        }
    }


