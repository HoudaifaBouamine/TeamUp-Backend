using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Users;

partial class UserEndpoints
{

    async Task<Results<Ok<GetUsersListResponse>,StatusCodeHttpResult>> 
        GetUsersAsync(
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
        else if (PageSize is not null)
            users = users
                .Take(PageSize.Value);


        var usersResult = await users
            .Select(u => new UserReadDto(u.Id,u.Email!,u.DisplayName,u.Handler,u.Rate,u.ProfilePicture!))
            .ToListAsync();

        return TypedResults.Ok(new GetUsersListResponse
            (
            TotalCount,
            PageNumber??=1,
            PageSize??=TotalCount,
            PageNumber > 1,
            PageNumber * PageSize < TotalCount, 
            usersResult));
    }
}

