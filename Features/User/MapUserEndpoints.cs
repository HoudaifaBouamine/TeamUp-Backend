using Carter;
using Configuration;

namespace Users;

public partial class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var usersGroup = app
            .MapGroup("/users")
            .WithTags("Users")
            .HasApiVersion(1)
            .RequireRateLimiting(RateLimiterConfig.Policy.Fixed)
            .WithSummary("In development")
            .WithOpenApi();

        usersGroup.MapGet("/",GetUsers)
            .Produces(StatusCodes.Status200OK,typeof(GetUsersResponse))
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
