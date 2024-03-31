using Authentication;
using Authentication.IdentityApi;
using Authentication.UserManager;
using EmailServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Moq;
using static Authentication.IdentityApi.AuthEndpoints;

namespace TeamUp.Test.Integration.Authentication;

public class AuthIntegrationTests
{
    [Fact]
    async void RegisterAndConfirmeEmail()
    {
        // Arrange
        var emailSenderMock = new Mock<IEmailSenderCustome>();
        emailSenderMock.Setup(es=>es.SendConfirmationCodeAsync(
            It.IsAny<User>(),
            It.IsAny<string>(),
            It.IsAny<string>()
        )).ReturnsAsync(true);

        var authEndpoints = new AuthEndpoints(emailSenderMock.Object);
        
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext> ();
        optionsBuilder.UseInMemoryDatabase("TeamUpDb");
        var db = new AppDbContext(optionsBuilder.Options);
        var userRegisterDto = new UserRegisterDto
        (
            "displayName",
            "email@example.com",
            "password"
        );

        var userManagerMock = new Mock<FakeUserManager>();
        userManagerMock.Setup(um=>um.Users)
            .Returns(db.Users);
        userManagerMock.Setup(u=>u.SupportsUserEmail)
            .Returns(true);
        userManagerMock.Setup(u=>u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .Callback(()=>
            {
                db.Users.Add(new User
                {
                    Email = userRegisterDto.Email,
                    DisplayName = userRegisterDto.DisplayName,
                    PasswordHash = userRegisterDto.Password
                });
                db.SaveChangesAsync();
            })
            .ReturnsAsync(IdentityResult.Success);
            

        // Act

        var response = await authEndpoints.RegisterAsync
        (
            registration: userRegisterDto,
            userManager: userManagerMock.Object
        );
        
        // Assert

        Assert.IsType<Ok> (response.Result);
    }

    public class FakeUserManager : CustomUserManager
    {
        public FakeUserManager() : base(
            new Mock<IUserStore<User>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<User>>().Object,
            Array.Empty<IUserValidator<User>>(),
            Array.Empty<IPasswordValidator<User>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<User>>>().Object)
        {
        }
    }
}