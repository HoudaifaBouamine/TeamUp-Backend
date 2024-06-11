using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace TeamUp.Features.Mentor;


[Tags("Mentors Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/mentors")]
[ApiController]

public class MentorsEndpoints(
    [FromServices] IMentorRepository mentorRepository,
    [FromServices] UserManager<User> userManager) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMentorsDetailsListResponse))]
    public async Task<IResult> GetMentorsAsync(
        [FromQuery] string? searchPattern,
        [FromQuery] int? pageSize = 10,
        [FromQuery] int? pageNumber = 1)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Results.Unauthorized();
        var mentors = await mentorRepository.GetMentorsAsync(user,searchPattern!, pageSize!.Value, pageNumber!.Value);
        return Results.Ok(mentors);
    }
}

public class MentorRepository(AppDbContext db) : IMentorRepository
{
    public async Task<GetMentorsDetailsListResponse> GetMentorsAsync(User currentUser, string? searchPattern, int pageSize, int pageNumber)
    {

        var mentors = db.Users.Where(u => u.IsMentor);
        
        if(searchPattern is not null)
            mentors = mentors.Where(u=>
                u.DisplayName.ToLower().Contains(searchPattern.ToLower()) ||
                u.Handler.ToLower().Contains(searchPattern.ToLower()));

        var totalCount = mentors.Count();

        mentors = mentors
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);
        
        var usersResult = await mentors
            .Include(u=>u.Projects)
            .Select(u => new MentorDetailsReadDto
                (
                    u.Id,
                    u.DisplayName,
                    u.Handler,
                    u.Rate,
                    404,
                    db.Follows.Include(f => f.Followee).Count(f => f.Followee.Id == u.Id),
                    db.UsersProjects.Where(up=>up.IsMentor)
                        .Include(up=>up.Project)
                            .ThenInclude(p=>p.ProjectsUsers)
                        .Select(up=>up.Project.ProjectsUsers
                            .Count(p => p.IsMentor == false))
                        .Count(),
                    db.Follows.Include(f=>f.Followee).Include(f=>f.Follower).Any(f=>f.Followee.Id == u.Id && f.Follower.Id == currentUser.Id),
                    u.ProfilePicture)).ToListAsync();

        return new GetMentorsDetailsListResponse
            (
                totalCount,
                pageNumber,
                pageSize,
                pageNumber > 1,
                pageNumber * pageSize < totalCount, 
                usersResult
            );
    }

}


public interface IMentorRepository
{
    Task<GetMentorsDetailsListResponse> GetMentorsAsync(User currentUser,string searchPattern, int pageSize, int pageNumber);
}

public record MentorReadDto
(
    Guid Id,
    string DisplayName,
    string Handler,
    float Rate,
    string ProfilePicture
);

public record MentorDetailsReadDto
(
    Guid Id,
    string DisplayName,
    string Handler,
    float Rate,
    int ReviewsCount,
    int FollowersCount,
    int MenteesCount,
    bool IsFollowed,
    string ProfilePicture
);

public record GetMentorsListResponse
(
    int TotalCount,
    int PageNumber,
    int PageSize,
    bool IsPrevPageExist,
    bool IsNextPageExist,
    IEnumerable<MentorReadDto> Mentors
);

public record GetMentorsDetailsListResponse
(
    int TotalCount,
    int PageNumber,
    int PageSize,
    bool IsPrevPageExist,
    bool IsNextPageExist,
    IEnumerable<MentorDetailsReadDto> Mentors
);