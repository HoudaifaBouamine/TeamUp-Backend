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
    // if(builder.Environment.IsDevelopment())
    // options.UseInMemoryDatabase("TeamUpDb");
    // else if(builder.Environment.IsProduction())
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));

    //options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
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
// (options=>
// {
//     options.DefaultScheme = "Cookie",
// });
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

// if(builder.Environment.IsDevelopment())
    // builder.Services.AddTransient<IEmailSenderCustome,EmailSenderMock>();
// else
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
        await ctx.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message));
    } 
});

app.Use(async (ctx, next) =>
{

    using (var scope = app.Services.CreateScope())
    {

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        if (!await db.Users.AnyAsync())
        {

            await AddTestingAccountsAsync(db, scope.ServiceProvider);

            await DataSeeder.SeedCaterogyData(db);
            await DataSeeder.SeedSkillsData(db);
            await DataSeeder.SeedUsersData(db);
            await DataSeeder.SeedProjectPostData(db);
        }
    }

    async Task  AddTestingAccountsAsync(AppDbContext db, IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<CustomUserManager>();

        var authEndpoints = new AuthEndpoints(serviceProvider.GetRequiredService<IEmailSenderCustome>());

        var dto = new AuthEndpoints.UserRegisterRequestDto
            ("string", "string@gmail.com", "stringstring");

        await authEndpoints.RegisterAsync(dto, userManager);

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == "string@gmail.com");
        user.SetAsMentor();

        await db.SaveChangesAsync();
        
        Log.Debug("User : " + JsonSerializer.Serialize(user));
    
    }
    
    await next();
});

app.MapGet("sendNotification",async (string token,string title,string body) =>
{
    var data = new JoinRequestNotificationData
    {
        message = "Please let me join your project !!!",
        projectId = "1",
        senderId = Guid.NewGuid(),
        projectTitle = "TeamUp",
        senderName = "Houdaifa Bouamine",
        senderPicture = "https://ipfs.io/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/363.jpg",
        requestId = "1"
    };
    await FireBaseNotification.SendMessageAsync(token,title,body,data);
});

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseRateLimiter();


app.MapAppEndpoints();     
app.MapHelpersEndpoints();
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.UseSwaggerDocs();

app.Run();
record RequestLog(string Path,string? User,int? StatusCode,double LatencyMilliseconds);

public partial class Program;