using Authentication.IdentityApi;
using Authentication.Oauth.Google;
using Carter;
using Configuration;
using Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using Models;
using Serilog;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Authentication
{
public class AuthenticationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/auth")
                .WithTags("Auth Group");

    }
}
}