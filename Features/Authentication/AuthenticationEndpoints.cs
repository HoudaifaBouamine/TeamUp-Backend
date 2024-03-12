using Authentication.CustomIdentityApi.V2;
using Carter;
using Configuration;

namespace Authentication
{
    public class AuthenticationEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var authGroup = app.MapGroup("/auth")
                    .WithTags("auth");

            authGroup.MapIdentityApiV2<User>().HasApiVersion(2)
                .RequireRateLimiting(RateLimiterConfig.Policy.Fixed)
                .WithSummary("Not complited, do not use v2 yet")
                .WithOpenApi();
        }
    }
}