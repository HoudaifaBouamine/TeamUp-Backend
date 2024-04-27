using Carter;
using Configuration;

namespace Users;

public partial class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var usersGroup = app
            .MapGroup("/users")
            .WithTags("Users Group")
            .HasApiVersion(1)
            .RequireRateLimiting(RateLimiterConfig.Policy.Fixed)
            .WithSummary("In development")
            .WithOpenApi();

        usersGroup.MapGet("/",GetUsersAsync)
            .Produces(StatusCodes.Status500InternalServerError);

        usersGroup.MapGet("/{Id}",GetUserAsync);

        usersGroup.MapPut("/",UpdateUserAsync)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(p=>p.RequireAuthenticatedUser());

        usersGroup.MapDelete("/{Id}",DeleteUserAsync)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

    }
}
