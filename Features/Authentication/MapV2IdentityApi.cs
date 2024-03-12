// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using EmailServices;
using Authentication.UserManager;
using Microsoft.EntityFrameworkCore;
using Humanizer;

namespace Authentication.CustomIdentityApi.V2;

/// <summary>
/// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add identity endpoints.
/// </summary>
public static class IdentityApiEndpointRouteBuilderExtensions
{
    // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    /// <summary>
    /// Add endpoints for registering, logging in, and logging out using ASP.NET Core Identity.
    /// </summary>
    /// <typeparam name="TUser">The type describing the user. This should match the generic parameter in <see cref="UserManager{TUser}"/>.</typeparam>
    /// <param name="endpoints">
    /// The <see cref="IEndpointRouteBuilder"/> to add the identity endpoints to.
    /// Call <see cref="EndpointRouteBuilderExtensions.MapGroup(IEndpointRouteBuilder, string)"/> to add a prefix to all the endpoints.
    /// </param>
    /// <returns>An <see cref="IEndpointConventionBuilder"/> to further customize the added endpoints.</returns>
    public static IEndpointConventionBuilder MapIdentityApiV2<TUser>(this IEndpointRouteBuilder endpoints)
        where TUser : class, new()
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var timeProvider = endpoints.ServiceProvider.GetRequiredService<TimeProvider>();
        var bearerTokenOptions = endpoints.ServiceProvider.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
        var emailSender = endpoints.ServiceProvider.GetRequiredService<IEmailSenderCustome<TUser>>();
        var linkGenerator = endpoints.ServiceProvider.GetRequiredService<LinkGenerator>();

        // We'll figure out a unique endpoint name based on the final route pattern during endpoint generation.
        string? confirmEmailEndpointName = null;

        var routeGroup = endpoints.MapGroup("");

        // NOTE: We cannot inject UserManager<TUser> directly because the TUser generic parameter is currently unsupported by RDG.
        // https://github.com/dotnet/aspnetcore/issues/47338
        routeGroup.MapPost("/register", async Task<Results<Ok, ValidationProblem>>
            ([FromBody] UserRegisterDto registration, HttpContext context, [FromServices] IServiceProvider sp, [FromServices] AppDbContext db) =>
        {
            var userManager = sp.GetRequiredService<UserManager<TUser>>();

            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException($"{nameof(MapIdentityApiV2)} requires a user store with email support.");
            }

            var userStore = sp.GetRequiredService<IUserStore<TUser>>();
            var emailStore = (IUserEmailStore<TUser>)userStore;
            var email = registration.Email;
            var userName = registration.DisplayName;

            if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
            {
                return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
            }

            var user = new TUser();
            await userStore.SetUserNameAsync(user, userName, CancellationToken.None);
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);
            var result = await userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                return CreateValidationProblem(result);
            }

            await SendEmailConfirmationCodeEmailAsync(db,user,email,VerificationCode.VerificationCodeTypes.EmailVerification);
            return TypedResults.Ok();
        })
        .WithSummary("[C] an email will be send to the user to confirm it his email address")
        .WithOpenApi();

        routeGroup.MapPost("/login", async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult,UnauthorizedHttpResult>>
            ([FromBody] UserLoginDto login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp) =>
        {
            var signInManager = sp.GetRequiredService<SignInManager<TUser>>();
            var userManager = sp.GetRequiredService<UserManager<TUser>>();

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
            var signInManager = sp.GetRequiredService<SignInManager<TUser>>();
            var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
            var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

            // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
            if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
                timeProvider.GetUtcNow() >= expiresUtc ||
                await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not TUser user)

            {
                return TypedResults.Challenge();
            }

            var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
            return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
        })
        .WithSummary("[C]]")
        .WithOpenApi();

        routeGroup.MapPost("/confirmEmail", async Task<Results<Ok,BadRequest<object>, UnauthorizedHttpResult>>
            ([FromBody] EmailConfirmationDto emailConfirmation, [FromServices] IServiceProvider sp,[FromServices] AppDbContext db) =>
        {
            

            var userManager = sp.GetRequiredService<CustomUserManager>();



            if (await userManager.FindByEmailAsync(emailConfirmation.Email) is not { } userIdentity)
            {
                // We could respond with a 404 instead of a 401 like Identity UI, but that feels like unnecessary information.
                return TypedResults.Unauthorized();
            }

            
            var userModel = await db.Users.Include(u=>u.EmailVerificationCode).FirstOrDefaultAsync(u=>u.Id == userIdentity.Id);
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
            confirmEmailEndpointName = $"{nameof(MapIdentityApiV2)}-{finalPattern}";
            endpointBuilder.Metadata.Add(new EndpointNameMetadata(confirmEmailEndpointName));
        });

        routeGroup.MapPost("/resendConfirmationEmail", async Task<Ok>
            ([FromBody] ResendConfirmationEmailRequest resendRequest, HttpContext context, [FromServices] IServiceProvider sp,[FromServices] AppDbContext db) =>
        {
            var userManager = sp.GetRequiredService<UserManager<TUser>>();
            if (await userManager.FindByEmailAsync(resendRequest.Email) is not { } user)
            {
                return TypedResults.Ok();
            }

            await SendEmailConfirmationCodeEmailAsync(db,user,resendRequest.Email,VerificationCode.VerificationCodeTypes.EmailVerification);
            return TypedResults.Ok();
        })
        .WithSummary("[C]]")
        .WithOpenApi();

        routeGroup.MapPost("/forgotPassword", async Task<Results<Ok,NotFound, ValidationProblem>>
            ([FromBody] ForgotPasswordRequest resetRequest, [FromServices] AppDbContext db,[FromServices] IServiceProvider sp) =>
        {


            var userManager = sp.GetRequiredService<CustomUserManager>();

            var user = await db.Users.Include(u=>u.PasswordRestCode).FirstOrDefaultAsync(u=>u.Email == resetRequest.Email);

            if(user is null) return TypedResults.NotFound();
            
            var code = user.PasswordRestCode.GeneratePasswordRestCode(user.Id);
            
            await db.SaveChangesAsync();

            await SendPasswordResetCodeEmailAsync(db,null,resetRequest.Email,VerificationCode.VerificationCodeTypes.PasswordRest);


            return TypedResults.Ok();
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


        routeGroup.MapGet("/currentUser", async Task<Results<Ok<UserInfoDto>, ValidationProblem, NotFound>>
            (ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider sp) =>
        {
            var userManager = sp.GetRequiredService<UserManager<TUser>>();
            if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
            {
                return TypedResults.NotFound();
            }

            var userInfo = new UserInfoDto(
                Id : await userManager.GetUserIdAsync(user),
                Email : await userManager.GetEmailAsync(user),
                IsEmailConfirmed : await userManager.IsEmailConfirmedAsync(user),
                UserName : await userManager.GetUserNameAsync(user)
            );

            return TypedResults.Ok(userInfo);
//            return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
        })
        .RequireAuthorization(p=>p.RequireAuthenticatedUser())
        .WithSummary("[C]")
        .WithOpenApi()
        .Produces(StatusCodes.Status500InternalServerError);

        
        async Task SendEmailConfirmationCodeEmailAsync(AppDbContext db,TUser user, string email, VerificationCode.VerificationCodeTypes verificationCodeType)
        {
            User userModel = db.Users.First(u=>u.Email == email);
        
            userModel.EmailVerificationCode = new VerificationCode();
            var code = userModel.EmailVerificationCode.GenerateEmailVerificationCode(userModel.Id);
        
            await db.SaveChangesAsync();
            await emailSender.SendConfirmationCodeAsync(user, email, code);
        }
        
        async Task SendPasswordResetCodeEmailAsync(AppDbContext db,TUser user, string email, VerificationCode.VerificationCodeTypes verificationCodeType)
        {
            User userModel = db.Users.First(u=>u.Email == email);
   
            userModel.PasswordRestCode = new VerificationCode();
            var code = userModel.PasswordRestCode.GeneratePasswordRestCode(userModel.Id);
        
            await db.SaveChangesAsync();
            await emailSender.SendPasswordResetCodeAsync(user, email, code);
        }
        
        return new IdentityEndpointsConventionBuilder(routeGroup);
        
        
    }





    private static ValidationProblem CreateValidationProblem(string errorCode, string errorDescription) =>
        TypedResults.ValidationProblem(new Dictionary<string, string[]> {
            { errorCode, [errorDescription] }
        });

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }

    private static async Task<InfoResponse> CreateInfoResponseAsync<TUser>(TUser user, UserManager<TUser> userManager)
        where TUser : class
    {
        return new()
        {
            Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
        };
    }

    // Wrap RouteGroupBuilder with a non-public type to avoid a potential future behavioral breaking change.
    private sealed class IdentityEndpointsConventionBuilder(RouteGroupBuilder inner) : IEndpointConventionBuilder
    {
        private IEndpointConventionBuilder InnerAsConventionBuilder => inner;

        public void Add(Action<EndpointBuilder> convention) => InnerAsConventionBuilder.Add(convention);
        public void Finally(Action<EndpointBuilder> finallyConvention) => InnerAsConventionBuilder.Finally(finallyConvention);
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromBodyAttribute : Attribute, IFromBodyMetadata
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromServicesAttribute : Attribute, IFromServiceMetadata
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    private sealed class FromQueryAttribute : Attribute, IFromQueryMetadata
    {
        public string? Name => null;
    }

    record UserInfoDto(string Id, string UserName,string Email,bool IsEmailConfirmed);
    record UserLoginDto(string Email,string Password);
    record UserRegisterDto(string DisplayName,string Email,string Password);
    record EmailConfirmationDto(string Email,string Code);
}