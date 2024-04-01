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

        routeGroup.MapPost("/login", Login)
        .WithSummary("[C]")
        .WithOpenApi();

        routeGroup.MapPost("/refresh", Refresh)
        .WithSummary("[C]]")
        .WithOpenApi();

        routeGroup.MapPost("/confirmEmail",ConfirmEmail)
        .WithSummary("[C]")
        .WithOpenApi()
        .Add(endpointBuilder =>
        {
            var finalPattern = ((RouteEndpointBuilder)endpointBuilder).RoutePattern.RawText;
            confirmEmailEndpointName = $"{nameof(ConfirmEmail)}-{finalPattern}";
            endpointBuilder.Metadata.Add(new EndpointNameMetadata(confirmEmailEndpointName));
        });

        routeGroup.MapPost("/resendConfirmationEmail", ResendConfirmationEmail)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("[C]]")
        .WithOpenApi();

        routeGroup.MapPost("/forgotPassword", ForgetPassword)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("[C]")
        .WithOpenApi();

        routeGroup.MapPost("/resetPassword", ResetPassword)
        .WithSummary("[C]]")
        .WithOpenApi();


        routeGroup.MapGet("/currenUser", CurrentUser)
        .RequireAuthorization(p=>p.RequireAuthenticatedUser())
        .WithSummary("[C]")
        .WithOpenApi()
        .Produces(StatusCodes.Status500InternalServerError);

        routeGroup.MapPost("/exchangeResetCodeForToken", GetResetPasswordToken)
        .HasApiVersion(3)
        .WithSummary("D")
        .WithOpenApi();
        
        routeGroup.MapPost("/resetPassword", ResetPasswordByToken)
        .HasApiVersion(3)
        .WithSummary("D")
        .WithOpenApi();

    }


    // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();
    IEmailSenderCustome emailSender = emailSender;
    string? confirmEmailEndpointName = null;
}