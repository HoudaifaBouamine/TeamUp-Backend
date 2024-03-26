using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;
using EmailServices;
using Authentication.UserManager;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Models;
using Utils;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.IdentityApi;

/// <summary>
/// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add identity endpoints.
/// </summary>
public partial class AuthEndpoints : ICarterModule
{
    // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    private object MapAuth()
    {
        throw new NotImplementedException();
    }

    TimeProvider timeProvider = null!;
    IOptionsMonitor<BearerTokenOptions> bearerTokenOptions = null!;
    IEmailSenderCustome emailSender = null!;
    LinkGenerator linkGenerator = null!;

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        timeProvider = app.ServiceProvider.GetRequiredService<TimeProvider>();
        bearerTokenOptions = app.ServiceProvider.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
        emailSender = app.ServiceProvider.GetRequiredService<IEmailSenderCustome>();
        linkGenerator = app.ServiceProvider.GetRequiredService<LinkGenerator>();

        string? confirmEmailEndpointName = null;

        var routeGroup = app.MapGroup("auth").WithTags("Auth Group");

        routeGroup.MapPost("/register", Register)
        .WithSummary("[C] an email will be send to the user to confirm it his email address")
        .WithOpenApi();

        routeGroup.MapPost("/login", async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult,UnauthorizedHttpResult,BadRequest<ErrorResponse>>>
            ([FromBody] UserLoginDto login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp) =>
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

   
        })
        .WithSummary("[C]")
        .WithOpenApi();

        routeGroup.MapPost("/refresh", async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>>
            ([FromBody] RefreshRequest refreshRequest, [FromServices] IServiceProvider sp) =>
        {
            var signInManager = sp.GetRequiredService<SignInManager<User>>();
            var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
            var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

            // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
            if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
                timeProvider.GetUtcNow() >= expiresUtc ||
                await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not User user)

            {
                return TypedResults.Challenge();
            }

            var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
            return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
        })
        .WithSummary("[C]]")
        .WithOpenApi();

        routeGroup.MapPost("/confirmEmail", async Task<Results<Ok,BadRequest<object>, UnauthorizedHttpResult,NotFound>>
            ([FromBody] EmailConfirmationDto emailConfirmation, [FromServices] IServiceProvider sp,[FromServices] AppDbContext db) =>
        {
            

            var userManager = sp.GetRequiredService<CustomUserManager>();



            if (await userManager.FindByEmailAsync(emailConfirmation.Email) is not { } userIdentity)
            {
                // We could respond with a 404 instead of a 401 like Identity UI, but that feels like unnecessary information.
                return TypedResults.Unauthorized();
            }

            
            var userModel = await db.Users.Include(u=>u.EmailVerificationCode).FirstOrDefaultAsync(u=>u.Id == userIdentity.Id);

            if(userModel is null) return TypedResults.NotFound();

            var result = await userManager.ConfirmEmailAsync(userModel,emailConfirmation.Code);

            if(result.Succeeded)
            {
                return TypedResults.Ok();
            }
            else
            {
                object errorResponse = new 
                    {
                        Error = result.Errors.First(),
                        StatusCode = 400,
                    };
                return TypedResults.BadRequest(errorResponse);
            }

        })
        .WithSummary("[C]")
        .WithOpenApi()
        .Add(endpointBuilder =>
        {
            var finalPattern = ((RouteEndpointBuilder)endpointBuilder).RoutePattern.RawText;
            confirmEmailEndpointName = $"{nameof(MapAuth)}-{finalPattern}";
            endpointBuilder.Metadata.Add(new EndpointNameMetadata(confirmEmailEndpointName));
        });

        routeGroup.MapPost("/resendConfirmationEmail", async Task<Results<Ok,StatusCodeHttpResult>>
            ([FromBody] ResendConfirmationEmailRequest resendRequest, HttpContext context, [FromServices] IServiceProvider sp,[FromServices] AppDbContext db) =>
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
        })
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("[C]]")
        .WithOpenApi();

        routeGroup.MapPost("/forgotPassword", async Task<Results<Ok,NotFound, ValidationProblem,StatusCodeHttpResult>>
            ([FromBody] ForgotPasswordRequest resetRequest, [FromServices] AppDbContext db,[FromServices] IServiceProvider sp) =>
        {


            var userManager = sp.GetRequiredService<CustomUserManager>();

            var user = await db.Users.Include(u=>u.PasswordRestCode).FirstOrDefaultAsync(u=>u.Email == resetRequest.Email);

            if(user is null) return TypedResults.NotFound();
            
            if(user.PasswordRestCode is null) user.PasswordRestCode = VerificationCode.CreatePasswordResetCode();
            var code = user.PasswordRestCode.Code;
            
            await db.SaveChangesAsync();

            var success = await SendPasswordResetCodeEmailAsync(db,resetRequest.Email,VerificationCode.VerificationCodeTypes.PasswordRest);

            if(success)
                return TypedResults.Ok();
            else
                return TypedResults.StatusCode(500);

        })
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("[C]")
        .WithOpenApi();

        routeGroup.MapPost("/resetPassword", async Task<Results<Ok, ValidationProblem,NotFound>>
            ([FromBody] ResetPasswordRequest resetRequest, [FromServices] IServiceProvider sp, [FromServices] AppDbContext db) =>
        {
            var userManager = sp.GetRequiredService<CustomUserManager>();

            var user = await db.Users.Include(u=>u.PasswordRestCode).FirstOrDefaultAsync(u=>u.Email == resetRequest.Email);

            if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
                // returned a 400 for an invalid code given a valid user email.
                return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
            }

            IdentityResult result;
            try
            {
                result = await userManager.ResetPasswordAsync(user, resetRequest.ResetCode, resetRequest.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
            }

            if (!result.Succeeded)
            {
                return CreateValidationProblem(result);
            }

            return TypedResults.Ok();
        })
        .WithSummary("[C]]")
        .WithOpenApi();


        routeGroup.MapGet("/currenUser", async Task<Results<Ok<UserInfoDto>, ValidationProblem, NotFound>>
            (ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider sp) =>
        {
            var userManager = sp.GetRequiredService<UserManager<User>>();
            if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
            {
                return TypedResults.NotFound();
            }

            var userInfo = new UserInfoDto(
                Id : user.Id,
                Email : user.Email!,
                IsEmailConfirmed : user.EmailConfirmed,
                UserName : user.DisplayName
            );

            return TypedResults.Ok(userInfo);
//            return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
        })
        .RequireAuthorization(p=>p.RequireAuthenticatedUser())
        .WithSummary("[C]")
        .WithOpenApi()
        .Produces(StatusCodes.Status500InternalServerError);

    }







}