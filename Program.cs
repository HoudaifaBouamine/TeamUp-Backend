using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Authentication.Oauth.Google;
using Authentication.UserManager;
using Carter;
using Configuration;
using EmailServices;
using EndpointsManager;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Swashbuckle.AspNetCore.Filters;
using static Google.Apis.Auth.GoogleJsonWebSignature;

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



app.MapGet("/users",(AppDbContext db)=>
{
    return db.Users.Select(u=>new {Id = u.Id,DisplayName = u.DisplayName, Email = u.Email});
});

// app.MapPost("/login-google", async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult,UnauthorizedHttpResult>>
//             ([FromBody] UserLoginDto login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp) =>
//         {
//             var signInManager = sp.GetRequiredService<SignInManager<TUser>>();
//             var userManager = sp.GetRequiredService<UserManager<TUser>>();

//             var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
//             var isPersistent = (useCookies == true) && (useSessionCookies != true);
//             signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

//             if (await userManager.FindByEmailAsync(login.Email) is not { } user)
//             {
//                 // We could respond with a 404 instead of a 401 like Identity UI, but that feels like unnecessary information.
//                 return TypedResults.Unauthorized();
//             }

//             string userName = (await userManager.GetUserNameAsync(user))!; 

//             var result = await signInManager.PasswordSignInAsync(userName,login.Password, isPersistent, lockoutOnFailure: true);

//             if (!result.Succeeded)
//             {
//                 return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
//             }

//             // The signInManager already produced the needed response in the form of a cookie or bearer token.
//             return TypedResults.Empty;
//         });

app.Run();
record RequestLog(string Path,string? User,int? StatusCode,double LatencyMilliseconds);
public class GoogleSignInDto
    {
        /// <summary>
        /// This token being passed here is generated from the client side when a request is made  to 
        /// i.e. react, angular, flutter etc. It is being returned as A jwt from google oauth server. 
        /// </summary>
        [Required]
        public string IdToken { get; set; } 
    }
        public enum LoginProvider
    {
        Google = 1,
        Facebook
    }