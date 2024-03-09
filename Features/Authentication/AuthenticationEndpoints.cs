using Authentication.CustomIdentityApi.V1;
using Authentication.CustomIdentityApi.V2;
using Carter;
using Configuration;
using Microsoft.AspNetCore.Identity;

namespace Authentication
{
    public class AuthenticationEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var authGroup = app.MapGroup("/auth")
                    .WithTags("auth");

            authGroup.MapIdentityApiV1<IdentityUser>().HasApiVersion(1)
                .RequireRateLimiting(RateLimiterConfig.Policy.Fixed);

            authGroup.MapIdentityApiV2<IdentityUser>().HasApiVersion(2)
                .RequireRateLimiting(RateLimiterConfig.Policy.Fixed)
                .WithSummary("Not complited, do not use v2 yet")
                .WithOpenApi();
        }
    }
}