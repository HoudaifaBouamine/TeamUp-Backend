using System.Security.Claims;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Carter;
using Configuration;
using EndpointsManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options=>
{
    options.AddSecurityDefinition("oauth2",new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Description = "write : 'Bearer {accessTocken}' then click 'Authorize'"
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddApiVersioning(options=>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options=>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.ConfigureOptions<ConfugureSwaggerOptions>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });
});

builder.Services.AddDbContext<AppDbContext>(options=>
{
    options.UseNpgsql("Server=ep-purple-shadow-a2sl2l68.eu-central-1.aws.neon.tech;Database=TeamUp;Username=HoudaifaBouamine;Password=dipwbSB3Pj6l");
        // builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
});

builder.Services.AddCarter();

builder.Services.AddRateLimiter(options=>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy(RateLimiterConfig.Policy.Fixed, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey:httpContext?.Connection?.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(5)
            }
        ));

});


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
    
///////////////////////////////////////////////////

var app = builder.Build();

app.Use((ctx,next)=>
{

    var start = DateTime.UtcNow;
    var task = next();
    var end = DateTime.UtcNow;

    var log = new RequestLog(
        ctx.Request.Path,
        ctx?.User?.Identity?.Name,
        ctx?.Response.StatusCode,
        (end-start).TotalMilliseconds);

    Log.Information("{log}",log);

   return task;

});

app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseCors("AllowAll");

app.MapAppEndpoints();     
app.MapHelpersEndpoints();
app.UseSwaggerDocs();

app.Run();
record RequestLog(string Path,string? User,int? StatusCode,double LatencyMilliseconds);

class EmailSender : IEmailSender<IdentityUser>
{
    public Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
    {
        // System.Console.WriteLine(" --> Send confirm link");
        
        // var message = new SendGridMessage();
        // message.AddTo(email);
        // message.SetSubject("Confirm your email");
        // message.AddContent(MimeType.Html, $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.");
        // var client = new SendGridClient("YourSendGridApiKey");
        // return client.SendEmailAsync(message);

        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
    {
        //EmailService.SendEmail(email, "Reset your password", $"Your reset code: {resetCode}");
        System.Console.WriteLine(" --> Send password reset code");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
    {
        System.Console.WriteLine(" --> Send password reset link");

        return Task.CompletedTask;
    }
}