using Authentication.IdentityApi;
using Authentication.UserManager;
using EmailServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Models;
using Moq;
using static Authentication.IdentityApi.AuthEndpoints;

namespace TeamUp.Test.AuthenticationFeature;

public class AuthTest
{
    [Fact]
    async void Register_WhereUserInfoIsValied_ShouldReturnOk()
    {
        // Arrange
        var userRegister = new UserRegisterDto
        (
            "displayName",
            "email@example.com",
            "password"
        );

        var emailSender = new Mock<IEmailSenderCustome>();
        emailSender.Setup(es=>es.SendConfirmationCodeAsync(
            It.IsAny<User>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
                .ReturnsAsync(true);
                
        var authEndpoints = new AuthEndpoints(emailSender.Object);
        
        var store = new Mock<IUserEmailStore<User>>();
        var userManagerMock = new Mock<CustomUserManager>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        userManagerMock.Setup(um=>um.CreateAsync(
            It.IsAny<User>(),
            It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

        List<User> users = [new User
        {
            Email = userRegister.Email
        }];
        userManagerMock.Setup(um=>um.Users).Returns(users.AsQueryable());


        userManagerMock.Setup(um=>um.SupportsUserEmail).Returns(true);
        // act
        var response = await authEndpoints.RegisterAsync(userRegister,userManagerMock.Object);

        // Assert
        Assert.IsType<Ok>(response.Result);
    }
}
