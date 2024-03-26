using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bogus;
using Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;

namespace EndpointsManager
{
    public static class HelpersEndpoints
    {
        /// <summary>
        /// Containes endpoints for testing and documentations 
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

            testingGroup.MapGet("drop-tables",(AppDbContext db)=>
            {
                db.Users.ExecuteDelete();
                
            });

            
            testingGroup.MapGet("/generate-fake-data",async (AppDbContext db,UserManager<User> userManager)=>
            {
                var usersFaker = new Faker<User>()
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
                .RuleFor(u=>u.Handler,(f,u)=>$"{f.Name.JobTitle()} @{f.Company.CompanyName()}");

                var users = usersFaker.Generate(100);

                var projectFaker = new Faker<Project>()
                .RuleFor(p=>p.ProjectName, (f,p)=>f.Company.CompanyName())
                .RuleFor(p=>p.ProjectDescription, f=>f.Lorem.Paragraphs(2,5))
                .RuleFor(p=>p.StartDateTime,f=>f.Date.BetweenDateOnly(new DateOnly(2023,5,4),new DateOnly(2025,3,6)))
                .RuleFor(p=>p.EndDateTime,(f,p)=>{
                    
                    if(f.Random.Bool())
                    {
                        return f.Date.BetweenDateOnly(p.StartDateTime.AddDays(3),p.StartDateTime.AddDays(70));
                    }
                    return null;
                    });

                var projects = projectFaker.Generate(30);

                var rand = new Random();

                foreach(var p in projects)
                {
                    var mentor = users.ElementAt(rand.Next(users.Count()));
                    p.ProjectsUsers.Add(new UsersProject()
                    {
                        User = mentor,
                        IsMentor = true
                    });
                    p.Users.Add(mentor);

                    var usersInProjectCount = rand.Next(5,15);

                    for(int i = 0; i < usersInProjectCount;i++)
                    {
                        p.Users.Add(users.ElementAt(rand.Next(users.Count())));
                    }
                }

                await db.Projects.AddRangeAsync(projects);
                await db.Users.AddRangeAsync(users);
                await db.SaveChangesAsync();
            });

        }
    }
}