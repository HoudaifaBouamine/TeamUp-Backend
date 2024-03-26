using Authentication.CustomIdentityApi.V2;
using Authentication.Oauth.Google;
using Carter;
using Configuration;
using Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using Models;
using Serilog;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Authentication
{
public class AuthenticationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/auth")
                .WithTags("Auth Group");

        authGroup.MapIdentityApiV2<User>()
            .HasApiVersion(2)
            .RequireRateLimiting(RateLimiterConfig.Policy.Fixed)
            .WithOpenApi();

        authGroup.MapPost("/google",async ([FromBody] GoogleLoginDto model ,[FromServices] IServiceProvider sp,[FromServices] AppDbContext db,[FromServices] IOptions<GoogleAuthConfig> authConfigOptions)=>
        {

            var userManager = sp.GetRequiredService<UserManager<User>>();
            var signInManager = sp.GetRequiredService<SignInManager<User>>();
            var authConfig = authConfigOptions.Value;
            Payload payload = new();

                try
                {
                    payload = await ValidateAsync(model.IdToken, new ValidationSettings
                    {
                        Audience =[authConfig.AndroidClientId,"407408718192.apps.googleusercontent.com" ]
                    });

                }
                catch (Exception ex)
                {
                    Log.Error(" --> Error : " + ex.Message);

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
                    return Results.Ok();

                }
                else
                    return Results.BadRequest(new {Error = "Unable to link a Local User to a Provider"});
            
            
        });

    }
}
}