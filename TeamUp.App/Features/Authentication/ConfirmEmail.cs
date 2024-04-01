using Authentication.UserManager;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Authentication.IdentityApi;

partial class AuthEndpoints
{
    public async Task<Results<Ok<object>,BadRequest<ErrorResponse>,NotFound>> ConfirmEmailAsync (
        [FromBody] EmailConfirmationRequestDto emailConfirmation,
        [FromServices] CustomUserManager userManager)
    {
        
        var userModel = await userManager.Users
            .Include(u=>u.EmailVerificationCode)
            .FirstOrDefaultAsync(u=>u.Email == emailConfirmation.Email);

        if(userModel is null) 
        {
            System.Console.WriteLine("--> not found by id");
            return TypedResults.NotFound();
        }

        var result = await userManager.ConfirmEmailAsync(userModel,emailConfirmation.Code);

        if(result.Succeeded)
        {
            return TypedResults.Ok((object) new {Message = "Email Confirmed Seccuessfuly"});
        }
        else
        {
            return TypedResults.BadRequest(new ErrorResponse(result.Errors.First().Description));
        }
    }

}