using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Carter;
using TeamUp.Features.Chat;

namespace EndpointsManager
{
    public static class AppEndpoints
    {
        /// <summary>
        /// Manage all application's endpoints
        /// </summary>
        public static void MapAppEndpoints(this IEndpointRouteBuilder app)
        {
            
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(2))
                .ReportApiVersions()
                .Build();

            var versionedApp = app
                .MapGroup("api/v{apiVersion:apiVersion}")
                .WithApiVersionSet(apiVersionSet);

            versionedApp.MapCarter();
        }

    }
}