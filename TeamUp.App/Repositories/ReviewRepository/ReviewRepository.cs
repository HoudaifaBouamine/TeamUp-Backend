
using AutoMapper;
using DTos;
using Microsoft.EntityFrameworkCore;

using Models ; 

using Repositories ; 
 public class UserReviewRepository : IUserReviewRepository
{
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserReviewRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserReviewDto> GetUserReviewByIdAsync(int reviewId)
        {
            var review = await _context.UserReviews.FindAsync(reviewId);
            return _mapper.Map<UserReviewDto>(review);
        }

        public async Task CreateAsync(UserReviewDto reviewDto)
        {
            var review = _mapper.Map<UserReview>(reviewDto);
            _context.UserReviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserReviewDto reviewDto)
        {
            var review = _mapper.Map<UserReview>(reviewDto);
            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int reviewId)
        {
            var review = await _context.UserReviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.UserReviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

}

   
   