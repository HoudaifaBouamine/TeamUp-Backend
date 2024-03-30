using Microsoft.AspNetCore.Mvc;
using Features.Projects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace Features.Projects
{
    [Tags("Projects Group")]
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectsController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectReadDto>>> GetProjects()
        {
            var projects = await _projectRepository.GetAllAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDetailsReadDto>> GetProject(int id)
        {
            var project = await _projectRepository.GetDetailsAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectCreateDto>> CreateProject([FromBody] ProjectCreateDto projectDto)
        {
            // Validate the projectDto if necessary
            var projectId = await _projectRepository.CreateAsync(projectDto);
            return CreatedAtAction(nameof(GetProject), new { id = projectId }, projectDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectDetailsReadDto>> UpdateProject(int id, [FromBody] ProjectCreateDto projectDto)
        {
            // Validate the projectDto if necessary
            await _projectRepository.UpdateAsync(id, projectDto);
            var updatedProject = await _projectRepository.GetDetailsAsync(id);
            if (updatedProject == null)
            {
                return NotFound();
            }
            return Ok(updatedProject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var existingProject = await _projectRepository.GetByIdAsync(id);
            if (existingProject == null)
            {
                return NotFound();
            }
            await _projectRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
