using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Models;

namespace Authentication.IdentityApi;

partial class AuthEndpoints
{

    async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>> Refresh
        ([FromBody] RefreshRequest refreshRequest,
        [FromServices] IServiceProvider sp)
    {
        var signInManager = sp.GetRequiredService<SignInManager<User>>();
        var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not User user)

        {
            return TypedResults.Challenge();
        }

        var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
        return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
    }

}