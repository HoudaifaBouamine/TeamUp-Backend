using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EndpointsManager
{
    public static class HelpersEndpoints
    {
        /// <summary>
        /// Containes endpoints for testing and documentations 
        /// </summary>
        /// <param name="app"></param>
        public static void MapHelpersEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/",  ()=>
            {
                return Results.Redirect("/swagger/index.html");
            });

            app.MapGet("/test-auth",(ClaimsPrincipal user)=>
            {
                return  $"wow user = {user?.Identity?.Name} is here";
            })
            .RequireAuthorization();

            app.MapGet("/test-no-auth",(ClaimsPrincipal user)=>
            {
                return  $"wow user = {user?.Identity?.Name} is here";
            });

        }
    }
}