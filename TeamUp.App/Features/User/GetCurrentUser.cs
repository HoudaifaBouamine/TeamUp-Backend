﻿using System.Security.Claims;
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

    async Task<Results<Ok<MentorSelfReadDetailsDto>,Ok<UserSelfReadDetailsDto>, UnauthorizedHttpResult>>
        GetCurrentUserAsync(
            [FromServices] AppDbContext db,
            [FromServices] UserManager<User> userManager,
            ClaimsPrincipal currentUser)
    {

        var userId = Guid.Parse(userManager.GetUserId(currentUser) ?? string.Empty);

        var user = await db.Users
            .Include(u=>u.Skills)
            .Include(u=>u.Categories)
            .Include(u=>u.UsersProjects)
            .Include(u=>u.Projects)
            .FirstOrDefaultAsync(u=>u.Id == userId);

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

        var followersCount = await db.Follows.CountAsync(f => f.Followee == user);
        
        var userReadResult = GetSelfReadDto(user,menteesCount, followersCount);
        
        return TypedResults.Ok(userReadResult);
    }

    dynamic GetSelfReadDto(User user,int menteesCount,int followersCount)
    {
        dynamic userReadResult;
        
        if (menteesCount == 0)
        {
            userReadResult = new UserSelfReadDetailsDto
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
                IsMentor:false
            );
        }
        else
        {
            userReadResult = new MentorSelfReadDetailsDto
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
                MenteesCount: menteesCount,
                FollowersCount: followersCount,
                IsMentor:true
            );
        }

        return Task.FromResult(userReadResult);
    }
    
    public record MentorSelfReadDetailsDto(
        Guid Id,
        string Email,
        string DisplayName,
        string Handler,
        float Rate,
        string ProfilePicture,
        IEnumerable<string> Skills,
        IEnumerable<string> Categories,
        int ProjectCount,
        int MenteesCount,
        int FollowersCount,
        bool IsMentor = true
    );

    public record UserSelfReadDetailsDto(
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
        bool IsMentor = true
    );

}

