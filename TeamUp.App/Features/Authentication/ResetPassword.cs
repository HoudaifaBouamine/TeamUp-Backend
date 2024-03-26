using Authentication.UserManager;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Authentication.IdentityApi;

partial class AuthEndpoints
{
    
    async Task<Results<Ok, ValidationProblem,NotFound>> ResetPassword
        ([FromBody] ResetPasswordRequest resetRequest, 
        [FromServices] IServiceProvider sp, 
        [FromServices] AppDbContext db)
    {
        var userManager = sp.GetRequiredService<CustomUserManager>();

        var user = await db.Users.Include(u=>u.PasswordRestCode).FirstOrDefaultAsync(u=>u.Email == resetRequest.Email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
            // returned a 400 for an invalid code given a valid user email.
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