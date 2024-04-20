using Authentication.UserManager;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Models;
using Utils;

namespace Authentication.IdentityApi;
partial class AuthEndpoints
{
    async Task<Results<Ok,NotFound<ErrorResponse>,StatusCodeHttpResult>> ResendConfirmationEmail
        ([FromBody] ResendConfirmationEmailRequest resendRequest,
        [FromServices] CustomUserManager userManager)

    {
        if (await userManager.FindByEmailAsync(resendRequest.Email) is not { } user)
        {
            return TypedResults.NotFound(new ErrorResponse($"User with email {resendRequest.Email} is not valied"));
        }

        var success = await SendEmailConfirmationCodeEmailAsync(userManager,resendRequest.Email,VerificationCode.VerificationCodeTypes.EmailVerification);
        
        if(success)
            return TypedResults.Ok();
        else
            return TypedResults.StatusCode(500);
    }
}