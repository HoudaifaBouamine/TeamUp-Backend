using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Models;
using Serilog;
using Utils;

namespace Authentication.IdentityApi;
partial class AuthEndpoints
{
    public async Task<Results<Ok, ValidationProblem,BadRequest<ErrorResponse>>> Register
        ([FromBody] UserRegisterDto registration,
        HttpContext context,
        [FromServices] IServiceProvider sp,
        [FromServices] AppDbContext db)
    {

        var userManager = sp.GetRequiredService<UserManager<User>>();

        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException($"{nameof(Register)} requires a user store with email support.");
        }

        var userStore = sp.GetRequiredService<IUserStore<User>>();
        var emailStore = (IUserEmailStore<User>)userStore;
        var email = registration.Email;
        var userName = registration.Email;
        var displayName = registration.DisplayName;

        if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
        {
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
        }

        var user = new User();
        await userStore.SetUserNameAsync(user, userName, CancellationToken.None);
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);
        user.DisplayName = displayName;
        var result = await userManager.CreateAsync(user, registration.Password);

        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        var success = await SendEmailConfirmationCodeEmailAsync(db,email,VerificationCode.VerificationCodeTypes.EmailVerification);

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