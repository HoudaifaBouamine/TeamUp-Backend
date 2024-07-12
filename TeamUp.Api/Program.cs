using System.Reflection;
using System.Text.Json;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Authentication.IdentityApi;
using Authentication.Oauth.Google;
using Authentication.UserManager;
using Carter;
using Configuration;
using EmailServices;
using EndpointsManager;
using Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using Models;
using Utils;
using Repositories;
using TeamUp.Features.Mentor;
using TeamUp.Features.Notification;
using TeamUp.Features.Project;
using TeamUp.Features.Chat;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Azure.Identity;
using Microsoft.Extensions.Options;

const enEnv env = enEnv.LocalDevelopment;

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
    
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddApiVersioning(options=>
{
    options.DefaultApiVersion = new ApiVersion(4);
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
    if(env == enEnv.LocalDevelopment)
        options.UseSqlite("Data Source=TeamUp.db");

    if (env == enEnv.RemoteDevelopment)
        options.UseNpgsql(builder.Configuration.GetConnectionString("DevelopmentConnection"));

    if (env == enEnv.Production)
        options.UseNpgsql(builder.Configuration.GetConnectionString("ProductionConnection"));
});


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


builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

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

builder.Services.AddSignalR();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddTransient<IEmailSenderCustome,EmailSender>();

builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<CustomUserManager>();
builder.Services.AddScoped<CustomUserManagerV2>();
builder.Services.AddScoped<GoogleAuthService>();
builder.Services.AddScoped<IProjectRepository,ProjectRepository>();
builder.Services.Configure<GoogleAuthConfig>(builder.Configuration.GetSection("Authentication:Google"));
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<ISkillRepository,SkillRepository>();
builder.Services.AddScoped<IMentorRepository,MentorRepository>();
builder.Services.AddScoped<INotificationService,FirebaseNotificationService>();

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

    // Log.Information("{log}",log);

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
        if (env != enEnv.Production)
            await ctx.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message));
        else
            Log.Error(JsonSerializer.Serialize(new ErrorResponse(ex.Message)));
    } 
});

if (env != enEnv.Production)
{
    // seeding testing data
    app.Use(async (ctx, next) =>
    {

        using (var scope = app.Services.CreateScope())
        {

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!await db.Users.AnyAsync())
            {

                //await AddTestingAccountsAsync(db, scope.ServiceProvider);

                await DataSeeder.SeedCaterogyData(db);
                await DataSeeder.SeedSkillsData(db);
                await DataSeeder.SeedUsersData(db);
                await DataSeeder.SeedProjectPostData(db);
            }
        }

        await next();

        async Task AddTestingAccountsAsync(AppDbContext db, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<CustomUserManager>();

            var authEndpoints = new AuthEndpoints(serviceProvider.GetRequiredService<IEmailSenderCustome>());

            var dto = new AuthEndpoints.UserRegisterRequestDto
                ("string", "string@gmail.com", "stringstring");

            var admindto = new AuthEndpoints.UserRegisterRequestDto
                ("admin", "admin@teamup.com", "adminadmin");

            await authEndpoints.RegisterAsync(dto, userManager);

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == "string@gmail.com");
            user?.SetAsMentor();

            await authEndpoints.RegisterAsync(admindto, userManager);

            var admin = await db.Users.FirstOrDefaultAsync(u => u.Email == "admin@teamup.com");
            admin?.SetAsMentor();

            await db.SaveChangesAsync();

            Log.Debug("User  : " + JsonSerializer.Serialize(user));
            Log.Debug("Admin : " + JsonSerializer.Serialize(user));
        }

    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseRateLimiter();

app.MapHub<ChatHub>("/chat");
app.MapAppEndpoints();     
app.MapHelpersEndpoints();
app.MapControllers();
app.UseSwaggerDocs();

app.Run();
record RequestLog(string Path,string? User,int? StatusCode,double LatencyMilliseconds);
enum enEnv { Production, LocalDevelopment , RemoteDevelopment }

public partial class Program;