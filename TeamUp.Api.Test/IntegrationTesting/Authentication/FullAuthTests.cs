using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using static Authentication.IdentityApi.AuthEndpoints;

namespace TeamUp.Test.IntegrationTesting.Authentication;

public partial class FullAuthTests
{
    [Fact]
    public async void RegsiterAndLogin()
    {
        // Arrange
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
        return;
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
    }
}
