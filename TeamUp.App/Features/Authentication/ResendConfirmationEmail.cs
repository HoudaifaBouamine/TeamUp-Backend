using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Models;

namespace Authentication.IdentityApi;
partial class AuthEndpoints
{
    async Task<Results<Ok,StatusCodeHttpResult>> ResendConfirmationEmail
        ([FromBody] ResendConfirmationEmailRequest resendRequest,
        HttpContext context,
        [FromServices] IServiceProvider sp,
        [FromServices] AppDbContext db)
    {
        var userManager = sp.GetRequiredService<UserManager<User>>();
        if (await userManager.FindByEmailAsync(resendRequest.Email) is not { } user)
        {
            return TypedResults.Ok();
        }

        var success = await SendEmailConfirmationCodeEmailAsync(db,resendRequest.Email,VerificationCode.VerificationCodeTypes.EmailVerification);
        
        if(success)
            return TypedResults.Ok();
        else
            return TypedResults.StatusCode(500);
    }
}