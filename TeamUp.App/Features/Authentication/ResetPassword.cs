using Authentication.UserManager;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Authentication.IdentityApi;

partial class AuthEndpoints
{
    
    async Task<Results<Ok, ValidationProblem,NotFound>> ResetPassword(
        [FromBody] ResetPasswordRequest resetRequest, 
        [FromServices] IServiceProvider sp, 
        [FromServices] AppDbContext db)
    {
        var userManager = sp.GetRequiredService<CustomUserManager>();

        var user = await db.Users.Include(u=>u.PasswordRestCode).FirstOrDefaultAsync(u=>u.Email == resetRequest.Email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
        }

        IdentityResult result;
        try
        {
            result = await userManager.ResetPasswordAsync(user, resetRequest.ResetCode, resetRequest.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
        }

        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        return TypedResults.Ok();
    }

    async Task<Results< Ok <GetResetPasswordTokenResponseDto>, ValidationProblem, NotFound>> GetResetPasswordToken(
        [FromBody] GetResetPasswordTokenRequestDto resetRequest, 
        [FromServices] IServiceProvider sp, 
        [FromServices] AppDbContext db,
        [FromServices] CustomUserManagerV2 userManager)
    {
        var user = await db.Users.Include(u=>u.PasswordRestCode).FirstOrDefaultAsync(u=>u.Email == resetRequest.Email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
        
        var token = CreateResetPasswordToken();
        user.PasswordResetToken = token;

        return TypedResults.Ok(new GetResetPasswordTokenResponseDto(token));
    }


    async Task<Results< Ok, ValidationProblem, NotFound>> ResetPasswordByToken(
    [FromBody] ResetPasswordByTokenRequestDto resetRequest, 
    [FromServices] AppDbContext db,
    [FromServices] CustomUserManagerV2 userManager
    )
    {
        var user = await db.Users.Include(u=>u.PasswordRestCode).FirstOrDefaultAsync(u=>u.Email == resetRequest.Email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
        }

        IdentityResult result;
        try
        {
            result = await userManager.ResetPasswordAsync(user, resetRequest.ResetToken, resetRequest.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
        }

        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        return TypedResults.Ok();
    }


    string CreateResetPasswordToken()
    {
        return Guid.NewGuid().ToString();
    }
}