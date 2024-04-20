using Authentication.UserManager;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Models;
using Serilog;
using Utils;

namespace Authentication.IdentityApi;
partial class AuthEndpoints
{
    public async Task<Results<Ok, ValidationProblem,BadRequest<ErrorResponse>>> 
    RegisterAsync(
        [FromBody] UserRegisterRequestDto registration,
        [FromServices] CustomUserManager userManager
    )
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException($"{nameof(RegisterAsync)} requires a user store with email support.");
        }
        
        var email = registration.Email;
        var userName = registration.Email;
        var displayName = registration.DisplayName;

        if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
        {
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
        }

        var user = new User
        {
            DisplayName = displayName,
            Email = email,
            UserName = userName
        };
        IdentityResult result = await userManager.CreateAsync(user, registration.Password);

        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        var success = await SendEmailConfirmationCodeEmailAsync(userManager,email,VerificationCode.VerificationCodeTypes.EmailVerification);

        if(success)
        {
            return TypedResults.Ok();
        }
        else
        {
            Log.Debug($" --> : Cannot send confirmation email to {user.Email}");
            return TypedResults.BadRequest(new ErrorResponse($"Cannot send confirmation email to {user.Email}"));
        }
    }

}