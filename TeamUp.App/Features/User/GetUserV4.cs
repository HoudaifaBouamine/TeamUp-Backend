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

    /// <summary>
    /// Get user/mentor data by Id (with if the current user follow him or not...)
    /// </summary>
    async Task<Results<Ok<MentorOtherReadDetailsDto>, Ok<UserOtherReadDetails5Dto>, UnauthorizedHttpResult>>
        GetUserV4Async(
            [FromServices] AppDbContext db,
            [FromServices] UserManager<User> userManager,
            ClaimsPrincipal currentUser,
            [FromRoute] Guid userId)
    {

        var currentUserId = Guid.Parse(userManager.GetUserId(currentUser) ?? string.Empty);
        
        var user = await db.Users
            .Include(u => u.Skills)
            .Include(u => u.Categories)
            .Include(u => u.UsersProjects)
            .Include(u => u.Projects)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return TypedResults.Unauthorized();

        var menteesCount = await db.UsersProjects
            .Include(up => up.User)
            .Where(up => up.User.Id == userId)
            .Where(up => up.IsMentor)
            .Include(up => up.Project)
            .Select(up => up.Project)
            .Select(p => p.ProjectsUsers
                .Count(pu => !pu.IsMentor))
            .SumAsync();

        var isFollowed = await db.Follows
            .Include(f => f.Followee)
            .Include(f => f.Follower)
            .Where(f => f.Followee.Id == user.Id && f.Follower.Id == currentUserId)
            .AnyAsync();

        var followersCount = await db.Follows.CountAsync(f => f.Followee == user);
        
        var userReadResult = GetOtherReadDto(user,isFollowed, menteesCount,followersCount);

        return TypedResults.Ok(userReadResult);
    }

    private dynamic GetOtherReadDto(User user,bool isFollowed,int menteesCount,int followersCount)
    {
        dynamic userReadResult;
        
        if (menteesCount == 0)
        {
            userReadResult = new UserOtherReadDetails5Dto
            (
                Id: user.Id,
                Email: user.Email!,
                DisplayName: user.DisplayName,
                Handler: user.Handler,
                Rate: user.Rate,
                ProfilePicture: user.ProfilePicture!,
                Skills: user.Skills.Select(s => s.Name),
                Categories: user.Categories.Select(c => c.Name),
                ProjectCount: user.Projects.Count,
                FollowersCount: followersCount,
                IsFollowed: isFollowed,
                IsMentor: false
            );
        }
        else
        {
            userReadResult = new MentorOtherReadDetailsDto
            (
                Id: user.Id,
                Email: user.Email!,
                DisplayName: user.DisplayName,
                Handler: user.Handler,
                Rate: user.Rate,
                ProfilePicture: user.ProfilePicture!,
                Skills: user.Skills.Select(s => s.Name),
                MenteeCount: menteesCount,
                Categories: user.Categories.Select(c => c.Name),
                ProjectCount: user.Projects.Count,
                FollowersCount: followersCount,
                IsFollowed: isFollowed,
                IsMentor:true
            );
        }

        return userReadResult;
    }

    
    public record UserOtherReadDetails5Dto(
        Guid Id,
        string Email,
        string DisplayName,
        string Handler,
        float Rate,
        string ProfilePicture,
        IEnumerable<string> Skills,
        IEnumerable<string> Categories,
        int ProjectCount,
        int FollowersCount,
        bool IsFollowed,
        bool IsMentor = false
    );
    
    public record MentorOtherReadDetailsDto(
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
        int FollowersCount,
        bool IsFollowed,
        bool IsMentor = true
    );
}

