using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Users;

partial class UserEndpoints
{

    async Task<Results<Ok<GetUsersResponse>,StatusCodeHttpResult>> 
        GetUsers(
        [FromQuery] string? SearchPattern,
        [FromQuery] int? PageNumber,
        [FromQuery] int? PageSize,
        [FromServices] AppDbContext db)
    {
        if(PageSize is null) PageNumber = null;

        IQueryable<User> users = db.Users;

        if(SearchPattern is not null)
            users = users.Where(u=>
                u.DisplayName.ToLower().Contains(SearchPattern.ToLower()) ||
                u.Handler.ToLower().Contains(SearchPattern.ToLower()));

        int TotalCount = users.Count();

        if(PageSize is not null && PageNumber is not null)
            users = users
                .Skip(PageSize.Value * (PageNumber.Value -1))
                .Take(PageSize.Value);

        var usersResult = await users
            .Select(u => new UserReadDto(u.Id,u.Email,u.DisplayName,u.Handler,u.Rate,u.ProfilePicture!))
            .ToListAsync();

        return TypedResults.Ok(new GetUsersResponse
            (
            TotalCount,
            PageNumber??=1,
            PageSize??=TotalCount,
            PageNumber > 1,
            PageNumber * PageSize < TotalCount, 
            usersResult));
    }

    record GetUsersResponse(int TotalCount,int PageNumber,int PageSize,bool IsPrevPageExist,bool IsNextPageExist,IEnumerable<UserReadDto> Users);
    record UserReadDto(string Id,string Email,string DisplayName,string Handler,float Rate,string ProfilePicture);
}

