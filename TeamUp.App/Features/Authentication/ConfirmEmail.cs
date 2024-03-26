using Authentication.UserManager;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Authentication.IdentityApi;

partial class AuthEndpoints
{
    async Task<Results<Ok<object>,BadRequest<ErrorResponse>,NotFound>> ConfirmEmail (
        [FromBody] EmailConfirmationDto emailConfirmation,
        [FromServices] IServiceProvider sp,
        [FromServices] AppDbContext db,
        [FromServices] CustomUserManager userManager)
    {

        if (await userManager.FindByEmailAsync(emailConfirmation.Email) is not { } userIdentity)
        {
            return TypedResults.NotFound();
        }
        
        var userModel = await db.Users.Include(u=>u.EmailVerificationCode).FirstOrDefaultAsync(u=>u.Id == userIdentity.Id);

        if(userModel is null) return TypedResults.NotFound();

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