using Authentication.UserManager;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;

namespace Authentication.IdentityApi;

partial class AuthEndpoints
{
    
    async Task<Results< Ok <GetResetPasswordTokenResponseDto>, ValidationProblem, NotFound>> 
        GetResetPasswordTokenAsync(
        [FromBody] GetResetPasswordTokenRequestDto resetRequest, 
        [FromServices] AppDbContext db,
        [FromServices] CustomUserManagerV2 userManager)
    {
        var user = await db.Users.Include(u=>u.PasswordRestCode).FirstOrDefaultAsync(u=>u.Email == resetRequest.Email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
        
        var token = CreateResetPasswordToken();
        user.SetPasswordRestToken(token);
        await userManager.UpdateAsync(user);
        return TypedResults.Ok(new GetResetPasswordTokenResponseDto(token));
    }


    async Task<Results< Ok, ValidationProblem, NotFound>> ResetPasswordByTokenAsync(
    [FromBody] ResetPasswordByTokenRequestDto resetRequest, 
    [FromServices] AppDbContext db,
    [FromServices] CustomUserManagerV2 userManager
    )
    {
        var user = await db.Users.FirstOrDefaultAsync(u=>u.Email == resetRequest.Email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
        }

        IdentityResult result;
        try
        {
            result = await userManager.ResetPasswordAsync(user, resetRequest.ResetToken, resetRequest.NewPassword);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to reset password, invaliedToken");
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







    async Task<Results<Ok, ValidationProblem,NotFound>> ResetPasswordAsync(
        [FromBody] ResetPasswordRequest resetRequest, 
        [FromServices] CustomUserManager userManager, 
        [FromServices] AppDbContext db)
    {

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

}