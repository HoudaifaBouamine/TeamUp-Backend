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
    /// <summary>
    /// 
    /// the user click 'reset password' so you do :
    /// call : /api/v2/auth/forgotPassword
    /// paramters : email
    /// response : ok, with sending code to user's emai  
    /// when the user enter the code reseved from email you do   
    /// call : /api/v3/auth/exchangeResetCodeForToken
    /// paramters : email, code (code recived in the email)
    /// response : ok with "resetToken   
    /// when you recive resetToken successfuly you do :
    /// call : /api/v3/auth/resetPassword
    /// paramters : email,resetToken, newPassword
    /// response : ok
    /// </summary>
    /// <returns></returns>
    void ResetPassoword()
    {

    }


    [Fact]
    async void RegisterAndConfirmeEmail()
    {
        // Arrange
        string VerificationCode = "NoCode";
        var emailSenderMock = new Mock<IEmailSenderCustome>();
        emailSenderMock.Setup(es=>es.SendConfirmationCodeAsync(
            It.IsAny<User>(),
            It.IsAny<string>(),
            It.IsAny<string>()
        ))
            .Callback((User user, string email, string code)=>
            {
                VerificationCode = code;
            })
            .ReturnsAsync(true);

        var authEndpoints = new AuthEndpoints(emailSenderMock.Object);
        
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext> ();
        optionsBuilder.UseInMemoryDatabase("TeamUpDb");
        var db = new AppDbContext(optionsBuilder.Options);
        var userRegisterDto = new UserRegisterRequestDto
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
        var registredUser =  await db.Users.Where(u=>u.Email == "email@example.com").FirstOrDefaultAsync();
        Assert.Equal(false,registredUser?.EmailConfirmed);
        Assert.Equal(userRegisterDto.Email, registredUser?.Email);
        System.Console.WriteLine("Code " + VerificationCode);
        Assert.Equal(6,VerificationCode.Length);
        // arrange

        var emailConfirmation = new EmailConfirmationRequestDto
        (
            Email: registredUser?.Email!,
            Code: VerificationCode
        );

        userManagerMock.Setup(u=>u.ConfirmEmailAsync
        (
            It.IsAny<User>(),
            It.IsAny<string>()
        ))
        .Callback((User user, string code)=>
        {
            user.EmailConfirmed = true;
        })
        .ReturnsAsync(IdentityResult.Success);
        // act

        var emailConfirmationRespones = await authEndpoints.ConfirmEmailAsync
        (
            emailConfirmation,
            userManager: userManagerMock.Object
        );

        // assert
        Assert.IsType<Ok<object>>(emailConfirmationRespones.Result);
        Assert.Equal(true,registredUser?.EmailConfirmed);

        await db.SaveChangesAsync();

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