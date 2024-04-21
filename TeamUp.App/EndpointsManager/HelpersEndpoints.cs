using System.Security.Claims;
using Bogus;
using Configuration;
using Features.Projects;
using Features.Projects.Contracts;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EndpointsManager
{
    public static class HelpersEndpoints
    {
        /// <summary>
        /// Contains endpoints for testing and documentations 
        /// </summary>
        /// <param name="app"></param>
        public static void MapHelpersEndpoints(this IEndpointRouteBuilder app)
        {
            var testingGroup = app.MapGroup("/").WithTags("Testing");

            testingGroup.MapGet("/",  ()=>
            {
                return Results.Redirect("/swagger/index.html");
            })
            .RequireRateLimiting(RateLimiterConfig.Policy.Fixed);;

            testingGroup.MapGet("/test-auth",(ClaimsPrincipal user)=>
            {
                return  $"wow user = {user?.Identity?.Name} is here";
            })
            .RequireAuthorization()
            .RequireRateLimiting(RateLimiterConfig.Policy.Fixed);;

            testingGroup.MapGet("/test-no-auth",(ClaimsPrincipal user)=>
            {
                return  $"wow user = {user?.Identity?.Name} is here";
            }).RequireRateLimiting(RateLimiterConfig.Policy.Fixed);;

            testingGroup.MapGet("/reset-database", async (AppDbContext db) =>
            {
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();   
            });

            testingGroup.MapGet("/generate-fake-data",async ([FromServices] AppDbContext db,[FromServices] IProjectRepository pr)=>
            {

        var usersFaker = new Faker<User>("en")
        .RuleFor(u=>u.FirstName,
            (f,u)=>f.Name.FirstName())
        .RuleFor(u=>u.LastName,
            (f,u)=>f.Name.LastName())
        .RuleFor(u=>u.DisplayName,
            (f,u)=>f.Internet.UserName(u.FirstName,u.LastName))
        .RuleFor(u=>u.Email, 
            (f,u)=>f.Internet.Email(u.FirstName,u.LastName))
        .RuleFor(u=>u.EmailConfirmed,(f,u)=>f.Random.Bool())
        .RuleFor(u=>u.FullAddress,(f,u)=>f.Address.FullAddress())
        .RuleFor(u=>u.PasswordHash,f=>f.Internet.Password())
        .RuleFor(u=>u.Handler,(f,u)=>$"{f.Name.JobTitle()} @{f.Company.CompanyName()}")
        .RuleFor(u=>u.ProfilePicture, f=>f.Internet.Avatar());

        var users = usersFaker.Generate(100);
        
        await db.Users.AddRangeAsync(users);
        await db.SaveChangesAsync();

        users = db.Users.ToList();

        var projectFaker = new Faker<Project>()
        .RuleFor(p=>p.Name, (f,p)=>f.Company.CompanyName())
        .RuleFor(p=>p.Description, f=>f.Lorem.Paragraphs(2,5))
        .RuleFor(p=>p.StartDate,f=>f.Date.BetweenDateOnly(new DateOnly(2023,5,4),new DateOnly(2025,3,6)))
        .RuleFor(p=>p.EndDate,(f,p)=>{
            
            if(f.Random.Bool())
            {
                return f.Date.BetweenDateOnly(p.StartDate.AddDays(3),p.StartDate.AddDays(70));
            }
            return null;
            });

        var projects = projectFaker.Generate(30);

        var rand = new Random();

        foreach(var p in projects)
        {
            int id = await pr.CreateAsync(new ProjectCreateDto
            (
                p.Name,
                p.Description,
                p.StartDate
            ),
            users.Where(u=>u.EmailConfirmed).ToList()[rand.Next(0,20)]    
            );

            for(int i = 0;i<rand.Next(5,20);i++)
            {
                await pr.AddUserToProjectAsync(id, users[i].Id, rand.Next(0,10) == 0);
            }
        }



        await db.SaveChangesAsync();

    
        });
        }
    }
}