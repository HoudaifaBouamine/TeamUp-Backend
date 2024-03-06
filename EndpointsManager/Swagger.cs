using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndpointsManager
{
    public static class SwaggerEndpoint
    {
        /// <summary>
        /// Configure swagger
        /// </summary>
        /// <param name="app"></param>
        public static void UseSwaggerDocs(this WebApplication app)
        {
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options=>
                {
                    var descriptions = app.DescribeApiVersions();

                    foreach (var description in descriptions)
                    {
                        string url = $"/swagger/{description.GroupName}/swagger.json";
                        string name = description.GroupName.ToUpperInvariant();

                        options.SwaggerEndpoint(url,name);
                    }
                });
            }
        }
    }
}