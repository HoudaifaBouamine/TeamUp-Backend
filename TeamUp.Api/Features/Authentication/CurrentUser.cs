using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Models;

namespace Authentication.IdentityApi;

partial class AuthEndpoints
{
        public async Task<Results<Ok<UserInfoResponseDto>, ValidationProblem, NotFound>> CurrentUserNewAsync
                (ClaimsPrincipal claimsPrincipal,
                [FromServices] UserManager<User> userManager) 
        {
    
            if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
            {
                return TypedResults.NotFound();
            }
    
            var userInfo = new UserInfoResponseDto(
                Id : user.Id,
                Email : user.Email!,
                IsEmailConfirmed : user.EmailConfirmed,
                UserName : user.DisplayName
            );
    
            return TypedResults.Ok(userInfo);
        }
        
        public record UserInfoResponseNewDto
        (
            Guid Id, 
            string DisplayName,
            string Handler,
            string PictureUrl,
            string Email,
            bool IsEmailConfirmed
        );
    

    public async Task<Results<Ok<UserInfoResponseDto>, ValidationProblem, NotFound>> CurrentUserAsync
            (ClaimsPrincipal claimsPrincipal,
            [FromServices] UserManager<User> userManager) 
    {

        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        var userInfo = new UserInfoResponseDto(
            Id : user.Id,
            Email : user.Email!,
            IsEmailConfirmed : user.EmailConfirmed,
            UserName : user.DisplayName
        );

        return TypedResults.Ok(userInfo);
    }
}