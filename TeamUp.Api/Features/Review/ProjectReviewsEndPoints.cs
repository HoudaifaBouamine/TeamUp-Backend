using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DTos;
using Repositories;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectReviewController : ControllerBase
    {
        private readonly IProjectReviewRepository _projectReviewRepository;

        public ProjectReviewController(IProjectReviewRepository projectReviewRepository)
        {
            _projectReviewRepository = projectReviewRepository;
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<ProjectReviewDto>>> GetProjectReviews(int projectId)
        {
            var reviews = await _projectReviewRepository.GetProjectReviewsAsync(projectId);
            return Ok(reviews);
        }
    }
}
