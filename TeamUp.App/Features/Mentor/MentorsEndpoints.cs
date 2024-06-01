using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Features;


[Tags("Mentors Group")]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/mentors")]
[ApiController]
public class MentorsEndpoints : ControllerBase
{

    private readonly IMentorRepository repo;
    public MentorsEndpoints([FromServices] IMentorRepository mentorRepository) 
    {
        repo = mentorRepository;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMentorsListResponse))]
    public async Task<IResult> GetMentorsAsync(
        [FromQuery] string? SearchPattern,
        [FromQuery] int? PageSize,
        [FromQuery] int? PageNumber)
    {
        var mentors = await repo.GetMentorsAsync(SearchPattern!, PageSize ?? 10, PageNumber ?? 1);
        return Results.Ok(mentors);
    }
}

public class MentorRepository(AppDbContext db) : IMentorRepository
{
    public async Task<GetMentorsListResponse> GetMentorsAsync(string? SearchPattern, int? PageSize, int? PageNumber)
    {
        
        var mentors = db.Users
            .Include(u => u.UsersProjects)
            .Where(u => u.UsersProjects
                .Any(p => p.IsMentor == true));

        var wow = db.Users.Include(u=>u.UsersProjects).ToList();



        if(PageSize is null) PageNumber = null;

        if(SearchPattern is not null)
            mentors = mentors.Where(u=>
                u.DisplayName.ToLower().Contains(SearchPattern.ToLower()) ||
                u.Handler.ToLower().Contains(SearchPattern.ToLower()));

        int TotalCount = mentors.Count();

        if(PageSize is not null && PageNumber is not null)
            mentors = mentors
                .Skip(PageSize.Value * (PageNumber.Value -1))
                .Take(PageSize.Value);
        else if (PageSize is not null)
            mentors = mentors
                .Take(PageSize.Value);


        var usersResult = await mentors
            .Include(u=>u.Projects)
            .Select(u => new MentorReadDto(u.Id,u.DisplayName,u.Handler,u.Rate,u.ProfilePicture!))
            .ToListAsync();

        return new GetMentorsListResponse
            (
            TotalCount,
            PageNumber??=1,
            PageSize??=TotalCount,
            PageNumber > 1,
            PageNumber * PageSize < TotalCount, 
            usersResult);
    }

}


public interface IMentorRepository
{
    Task<GetMentorsListResponse> GetMentorsAsync(string SearchPattern, int? pageSize, int? pageNumber);
}

public record MentorReadDto
(
    Guid Id,
    string DisplayName,
    string Handler,
    float Rate,
    string? ProfilePicture
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