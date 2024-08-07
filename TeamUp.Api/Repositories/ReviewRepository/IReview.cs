using DTos ; 
namespace Repositories ; 
public interface IUserReviewRepository
{
        Task<UserReviewDto> GetUserReviewByIdAsync(int reviewId);
        Task CreateAsync(UserReviewDto reviewDto);
        Task UpdateAsync(UserReviewDto reviewDto);
        Task DeleteAsync(int reviewId);
}

public interface IProjectReviewRepository
{
        Task<IEnumerable<ProjectReviewDto>> GetProjectReviewsAsync(int projectId);
}
