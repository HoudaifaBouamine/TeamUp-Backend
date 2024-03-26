using Authentication.Oauth.Google;
using Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using Models;
using Serilog;
using Utils;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Authentication.IdentityApi;

partial class AuthEndpoints
{
    
    async Task<Results<Ok,BadRequest<ErrorResponse>>> GoogleAsync(
        [FromBody] GoogleLoginDto model,
        [FromServices] IServiceProvider sp,
        [FromServices] AppDbContext db,
        [FromServices] IOptions<GoogleAuthConfig> authConfigOptions)
    {
        var userManager = sp.GetRequiredService<UserManager<User>>();
        var signInManager = sp.GetRequiredService<SignInManager<User>>();
        var authConfig = authConfigOptions.Value;
        Payload payload;

        try
        {
            payload = await ValidateAsync(model.IdToken, new ValidationSettings
            {
                Audience =[authConfig.AndroidClientId, authConfig.WebClientId,"407408718192.apps.googleusercontent.com" ]
            });

        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return TypedResults.BadRequest(new ErrorResponse(ex.Message));
        }

        var userToBeCreated = new CreateUserFromSocialLogin
        {
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
            Email = payload.Email,
            ProfilePicture = payload.Picture,
            LoginProviderSubject = payload.Subject,
        };
            
        var user = await userManager.CreateUserFromSocialLogin(db, userToBeCreated, LoginProvider.Google);

        if (user is not null)
        {
            await signInManager.ExternalLoginSignInAsync(LoginProvider.Google.GetDisplayName(),payload.Subject,true);
            return TypedResults.Ok();
        }
        else
        {
            return TypedResults.BadRequest(new ErrorResponse("Unable to link a Local User to a Provider"));
        }
    }

}
