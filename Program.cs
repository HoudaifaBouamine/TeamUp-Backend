using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;
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
    // Default Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
    
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowAll");


var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();

var versionedGroup = app
    .MapGroup("api/v{apiVersion:apiVersion}")
    .WithApiVersionSet(apiVersionSet);


var authGroup = versionedGroup.MapGroup("/auth")
                    .WithTags("auth");

authGroup.MapIdentityApi<IdentityUser>().HasApiVersion(1);

app.MapGet("/",  ()=>
{
    return Results.Redirect("/swagger/index.html");
});

app.MapGet("/test-auth",(ClaimsPrincipal user)=>
{
    return  $"wow user = {user.Identity.Name} is here";
})
.RequireAuthorization();

app.MapGet("/test-no-auth",(ClaimsPrincipal user)=>
{
    return  $"wow user = {user.Identity.Name} is here";
});


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

app.Run();
