using Microsoft.AspNetCore.Mvc;
using DTos ; 
using Repositories ; 
namespace Controllers
{[ApiController]
    [Route("api/[controller]")]
    public class UserReviewController : ControllerBase
    {
        private readonly IUserReviewRepository _userReviewRepository;

        public UserReviewController(IUserReviewRepository userReviewRepository)
        {
            _userReviewRepository = userReviewRepository;
        }

        [HttpGet("{reviewId}")]
        public async Task<ActionResult<UserReviewDto>> GetUserReviewById(int reviewId)
        {
            var review = await _userReviewRepository.GetUserReviewByIdAsync(reviewId);
            if (review == null)
            {
                return NotFound();
            }
            return review;
        }

        [HttpPost]
        public async Task<ActionResult> CreateUserReview(UserReviewDto reviewDto)
        {
            await _userReviewRepository.CreateAsync(reviewDto);
            return Ok();
        }

        [HttpPut("{reviewId}")]
        public async Task<ActionResult> UpdateUserReview(int reviewId, UserReviewDto reviewDto)
        {
            if (reviewId != reviewDto.Id)
            {
                return BadRequest();
            }
            await _userReviewRepository.UpdateAsync(reviewDto);
            return Ok();
        }

        [HttpDelete("{reviewId}")]
        public async Task<ActionResult> DeleteUserReview(int reviewId)
        {
            await _userReviewRepository.DeleteAsync(reviewId);
            return NoContent();
        }
    }
}
   

