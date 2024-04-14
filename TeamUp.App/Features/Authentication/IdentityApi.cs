using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.Extensions.Options;
using EmailServices;
using Carter;

namespace Authentication.IdentityApi;

public partial class AuthEndpoints(
    IEmailSenderCustome emailSender
    ) : ICarterModule 
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {

        var routeGroup = app.MapGroup("auth").WithTags("Auth Group");

        routeGroup.MapPost("/google", GoogleAsync);

        routeGroup.MapPost("/register", RegisterAsync)
        .WithSummary("[C] an email will be send to the user to confirm it his email address")
        .WithOpenApi();

        routeGroup.MapPost("/login", LoginAsync)
        .WithSummary("[C]")
        .WithOpenApi();

        routeGroup.MapPost("/refresh", RefreshAsync)
        .WithSummary("[C]]")
        .WithOpenApi();

        routeGroup.MapPost("/confirmEmail",ConfirmEmailAsync)
        .WithSummary("[C]")
        .WithOpenApi()
        .Add(endpointBuilder =>
        {
            var finalPattern = ((RouteEndpointBuilder)endpointBuilder).RoutePattern.RawText;
            confirmEmailEndpointName = $"{nameof(ConfirmEmailAsync)}-{finalPattern}";
            endpointBuilder.Metadata.Add(new EndpointNameMetadata(confirmEmailEndpointName));
        });

        routeGroup.MapPost("/resendConfirmationEmail", ResendConfirmationEmailAsync)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("[C]]")
        .WithOpenApi();

        routeGroup.MapPost("/forgotPassword", ForgetPasswordAsync)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("[C]")
        .WithOpenApi();

        routeGroup.MapPost("/resetPassword", ResetPasswordAsync)
        .WithSummary("[C]]")
        .WithOpenApi();


        routeGroup.MapGet("/currenUser", CurrentUserAsync)
        .RequireAuthorization(p=>p.RequireAuthenticatedUser())
        .WithSummary("[C]")
        .WithOpenApi()
        .Produces(StatusCodes.Status500InternalServerError);

        routeGroup.MapPost("/exchangeResetCodeForToken", GetResetPasswordTokenAsync)
        .HasApiVersion(3)
        .WithSummary("D")
        .WithOpenApi();
        
        routeGroup.MapPost("/resetPassword", ResetPasswordByTokenAsync)
        .HasApiVersion(3)
        .WithSummary("D")
        .WithOpenApi();

    }


    // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();
    IEmailSenderCustome emailSender = emailSender;
    string? confirmEmailEndpointName = null;
}