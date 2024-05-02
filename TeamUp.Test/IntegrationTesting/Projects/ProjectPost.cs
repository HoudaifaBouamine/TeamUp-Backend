// using unit test to build integration testing for project posts endpoints , for this secenarios :
// 0 - register an account and login with it, and use the provided token for authorization
// 1 - Create new project post as a mentro
// 3 - register and join with another account
// 4 - Get Project's posts list, with pagination and search as student
// 5 - send join request to a project post as student
// 6 - get project join requests list as the mentor who create it
// 7 - get project join request by id as student
// 8 - response to project join request as the mentor who create it


using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TeamUp.Test;
using static Authentication.IdentityApi.AuthEndpoints;
using static Features.ProjectPostEndpoints;

public class ProjectPostTests
{
    
    // implement unit tests for project posts endpoints

    [Fact]
    public async Task CreateProjectPostAsMentor_ShouldReturnSuccess_WhenValidDataIsProvided()
    {
var application = new ApplicationFactory();
        var client = application.CreateClient();
        var userRegister = new UserRegisterRequestDto
        (
            "Houdaifa Bouamine",
            "h.bouamine@esi-sba.dz",
            "1234"
        );
        // Act
        var response = await client.PostAsJsonAsync("/api/v2/auth/register", userRegister);

        // Assert
        response.EnsureSuccessStatusCode();
        
        // Arrange

        var userLogin = new UserLoginRequestDto
        (
            "h.bouamine@esi-sba.dz",
            "1234"
        );

        // Act
        response = await client.PostAsJsonAsync("/api/v2/auth/login", userLogin);
        var authToken = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
        // Arrange
        
        // Act 
        client.DefaultRequestHeaders.Add("Authorization","Bearer " + authToken?.AccessToken);
        response = await client.GetAsync("/api/v2/auth/currenUser");

        // Assert
        response.EnsureSuccessStatusCode();


        /// 
        Log.Debug("\n\n\n\n\n\n--------------> Hey Hey ===> It is find intel know\n\n\n\n\n\n\n\n\n\n");

        var posts_response = await client.PostAsJsonAsync("api/v4/projects-posts", new ProjectPostCreateDto
        (
            "project title",
            "project summary",
            "project Scenario",
            "prject learning Goals",
            "project team and roles",
            5,
            1,
            [".NET","C#","React.js","Angular.js","HTML"]            
        ));
        posts_response.EnsureSuccessStatusCode();
        
        var ok_post_repsonse = Assert.IsType<OkObjectResult> (posts_response);
        var result = Assert.IsType<ProjectPostReadDto>(ok_post_repsonse.Value);

        System.Console.WriteLine("\n\n\n--> " + result.Summary);
    }
}
