using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utils;

namespace Users;

partial class UserEndpoints
{
    
    [Authorize]
    async Task<Results<Ok<UserReadDetails4Dto>, NotFound<ErrorResponse>, UnauthorizedHttpResult>> 
        GetCurrentUserAsync(
        [FromServices] AppDbContext db,
        [FromServices] UserManager<User> userManager,
            ClaimsPrincipal currentUser)
    {

        var user = await userManager.GetUserAsync(currentUser);

        if(user is null)
            return TypedResults.NotFound(new ErrorResponse("User is not found"));

        var userReadResult = new UserReadDetails4Dto(
            Id    : user.Id,
            Email : user.Email!,
            DisplayName : user.DisplayName,
            Handler: user.Handler,
            Rate: user.Rate,
            ProfilePicture: user.ProfilePicture!,
            user.Skills.Select(s=>s.Name),
            user.Categories.Select(c=>c.Name));

        return TypedResults.Ok(userReadResult);
    }

    public record UserReadDetails5Dto(
        Guid Id,
        string Email,
        string DisplayName,
        string Handler,
        float Rate,
        string ProfilePicture,
        IEnumerable<string> Skills,
        IEnumerable<string> Categories,
        int ProjectCount,
        int MenteeCount,
        int FollowersCount
    );
}

