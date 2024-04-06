using Bogus.DataSets;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Models;
using Utils;

namespace Authentication.IdentityApi;

partial class AuthEndpoints
{
    async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult,UnauthorizedHttpResult,BadRequest<ErrorResponse>>> Login
        ([FromBody] UserLoginRequestDto login,
        [FromQuery] bool? useCookies,
        [FromQuery] bool? useSessionCookies,
        [FromServices] IServiceProvider sp)
    {
        var signInManager = sp.GetRequiredService<SignInManager<User>>();
        var userManager = sp.GetRequiredService<UserManager<User>>();

        var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
        var isPersistent = (useCookies == true) && (useSessionCookies != true);
        signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

        if (await userManager.FindByEmailAsync(login.Email) is not { } user)
        {
            // We could respond with a 404 instead of a 401 like Identity UI, but that feels like unnecessary information.
            return TypedResults.Unauthorized();
        }

        string userName = (await userManager.GetUserNameAsync(user))!; 

        var result = await signInManager.PasswordSignInAsync(userName,login.Password, isPersistent, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        // The signInManager already produced the needed response in the form of a cookie or bearer token.
        return TypedResults.Empty;
    }



}