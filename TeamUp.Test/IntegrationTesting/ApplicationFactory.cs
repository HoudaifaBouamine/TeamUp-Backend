using Castle.Core.Smtp;
using EmailServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static TeamUp.Test.IntegrationTesting.Authentication.FullAuthTests;

namespace TeamUp.Test;

internal class ApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services=>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            services.AddDbContext<AppDbContext>(op=>{
                op.UseInMemoryDatabase("TeamUpDb-Testing");
            });

            services.RemoveAll(typeof(IEmailSenderCustome));
            services.AddTransient<IEmailSenderCustome,EmailSenderMock>();

            var db = CreateDbContext(services);
            db.Database.EnsureDeleted();

        });
    }

    private static AppDbContext CreateDbContext(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}
