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
    async Task<Results<Ok<UserReadDetails5Dto>, NotFound<ErrorResponse>, UnauthorizedHttpResult>>
        GetCurrentUserAsync(
            [FromServices] AppDbContext db,
            [FromServices] UserManager<User> userManager,
            ClaimsPrincipal currentUser)
    {

        var user = await userManager.GetUserAsync(currentUser);
            
        if (user is null)
            return TypedResults.Unauthorized();

        var menteesCount = await db.UsersProjects
            .Include(up => up.User)
            .Where(up => up.User.Id == user.Id)
            .Where(up => up.IsMentor)
            .Include(up => up.Project)
            .Select(up => up.Project)
            .Select(p => p.ProjectsUsers
                .Count(pu => !pu.IsMentor))
            .SumAsync();

        var userReadResult = new UserReadDetails5Dto(
            Id: user.Id,
            Email: user.Email!,
            DisplayName: user.DisplayName,
            Handler: user.Handler,
            Rate: user.Rate,
            ProfilePicture: user.ProfilePicture!,
            user.Skills.Select(s => s.Name),
            user.Categories.Select(c => c.Name),
            user.Projects.Count,
            menteesCount,
            0
        );

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

