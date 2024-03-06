using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Carter;
using Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication
{
    public class AuthenticationEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var authGroup = app.MapGroup("/auth")
                    .WithTags("auth");

            authGroup.MapIdentityApi<IdentityUser>().HasApiVersion(1)
                .RequireRateLimiting(RateLimiterConfig.Policy.Fixed);
        }
    }
}