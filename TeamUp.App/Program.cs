using System.Threading.RateLimiting;
using Asp.Versioning;
using Authentication.Oauth.Google;
using Authentication.UserManager;
using Carter;
using Configuration;
using EmailServices;
using EndpointsManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using Models;
using Utils;

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
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
    options.User.RequireUniqueEmail = true;
});


// builder.Services.AddAuthentication().AddGoogle(googleOptions =>
//     {
//         googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//         googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//     });

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

builder.Services.AddTransient<IEmailSenderCustome,EmailSender>();
builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<CustomUserManager>();
builder.Services.AddScoped<GoogleAuthService>();
builder.Services.Configure<GoogleAuthConfig>(builder.Configuration.GetSection("Authentication:Google"));


///////////////////////////////////////////////////



var app = builder.Build();

// Handmade Global logging
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

// Handmade Global Error Handling
app.Use(async (ctx,next)=>
{
    try{
        await next();
    }
    catch(Exception ex)
    {
        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message));
    } 
});



app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseRateLimiter();

app.MapAppEndpoints();     
app.MapHelpersEndpoints();
app.MapControllers();
app.UseSwaggerDocs();

app.Run();
record RequestLog(string Path,string? User,int? StatusCode,double LatencyMilliseconds);
