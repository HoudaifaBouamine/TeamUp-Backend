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
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>();

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

versionedGroup.MapPost("/wow/{id}",async Task <Results<Ok<string>,NotFound>>(int id,[FromBody] string wow)=>{

    return TypedResults.Ok("worked");
}).HasApiVersion(2)
.Produces(StatusCodes.Status500InternalServerError)
.WithOpenApi(op =>
{
    op.Summary = "wow is amazing";
    op.Description = "this is the tetailed explaination for wow";
    op.Parameters[0].Description = "the wow id is greate";
    op.RequestBody.Description = "wow is wow";
    return op;
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
