using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.ResponseHandling;
using Enums;
using Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Extensions;
using Presentation;
using Serilog;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Authentication.Oauth.Google;

public class GoogleLoginDto
{
    /// <summary>
    /// This token being passed here is generated from the client side when a request is made  to 
    /// i.e. react, angular, flutter etc. It is being returned as A jwt from google oauth server. 
    /// </summary>
    [Required]
    public string IdToken { get; set; } 
}

public class GoogleAuthConfig 
{
    public string AndroidClientId { get; set; }
    public string WebClientId { get; set; }
    public string ClientSecret { get; set; } 
}

public class CreateUserFromSocialLogin
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string ProfilePicture { get; set; }
    public string LoginProviderSubject { get; set; }
}

    public static class CreateUserFromSocialLoginExtension
{
    /// <summary>
    /// Creates user from social login
    /// </summary>
    /// <param name="userManager">the usermanager</param>
    /// <param name="context">the context</param>
    /// <param name="model">the model</param>
    /// <param name="loginProvider">the login provider</param>
    /// <returns>System.Threading.Tasks.Task&lt;User&gt;</returns>

    
        public static async Task<User> CreateUserFromSocialLogin(this UserManager<User> userManager, AppDbContext context, CreateUserFromSocialLogin model, LoginProvider loginProvider)
        {
            //CHECKS IF THE USER HAS NOT ALREADY BEEN LINKED TO AN IDENTITY PROVIDER
            var user = await userManager.FindByLoginAsync(loginProvider.GetDisplayName(), model.LoginProviderSubject);

            if (user is not null)
                return user; //USER ALREADY EXISTS.

            user = await userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email,
                    ProfilePicture = model.ProfilePicture,
                };
                
                if(user.FirstName is null && user.LastName is null)
                    user.DisplayName = user.Email.Split("@")[0];
                else
                    user.DisplayName = user.GetFullName().Trim();

                var wow = await userManager.CreateAsync(user);
                if(wow.Succeeded)
                {
                    //EMAIL IS CONFIRMED; IT IS COMING FROM AN IDENTITY PROVIDER
                    user.EmailConfirmed = true;

                    var idResult = await userManager.UpdateAsync(user);
                    
                    if(idResult.Succeeded)
                    {
                        await context.SaveChangesAsync();

                    }
                    else
                    {
                        Log.Error(" --> Error : " + wow.Errors.First().Description);
                        return null;
                    }
                    

                }
                else
                {

                    Log.Error(" --> Error : " + wow.Errors.First().Description);
                    return null;
                }
            }

            UserLoginInfo userLoginInfo = null;
            switch (loginProvider)
            {
                case LoginProvider.Google:
                    {
                        userLoginInfo = new UserLoginInfo(loginProvider.GetDisplayName(), model.LoginProviderSubject, loginProvider.GetDisplayName().ToUpper());
                    }
                    break;
                // case LoginProvider.Facebook:
                //     {
                //         userLoginInfo = new UserLoginInfo(loginProvider.GetDisplayName(), model.LoginProviderSubject, loginProvider.GetDisplayName().ToUpper());
                //     }
                //     break;
                default:
                    break;
            }

            //ADDS THE USER TO AN IDENTITY PROVIDER
            var result = await userManager.AddLoginAsync(user, userLoginInfo);

            if (result.Succeeded)
                return user;

            else
            {
                Log.Error(" ---> Error : " + result.Errors.First());
                return null;
            }
        }
    
    
}

public class GoogleAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;
    private readonly GoogleAuthConfig _googleAuthConfig;

    public GoogleAuthService(
        UserManager<User> userManager, 
        AppDbContext context, 
        IOptions<GoogleAuthConfig> googleAuthConfig
        )
    {
        _userManager = userManager;
        _context = context;
        _googleAuthConfig = googleAuthConfig.Value;
    }

     
    public async Task<BaseResponse<User>> GoogleSignIn(GoogleLoginDto model)
    {

        Payload payload = new();

        try
        {
            payload = await ValidateAsync(model.IdToken, new ValidationSettings
            {
                Audience = [ _googleAuthConfig.AndroidClientId, _googleAuthConfig.WebClientId ]
            });

        }
        catch (Exception ex)
        {
            return new BaseResponse<User>(ex.Message, new List<string> { "Failed to get a response.",ex.Message });       
        }

        var userToBeCreated = new CreateUserFromSocialLogin
        {
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
            Email = payload.Email,
            ProfilePicture = payload.Picture,
            LoginProviderSubject = payload.Subject,
        };
        
        var user = await _userManager.CreateUserFromSocialLogin(_context, userToBeCreated, LoginProvider.Google);

        if (user is not null)
            return new BaseResponse<User>(user);

        else
            return new BaseResponse<User>(null, new List<string> { "Failed to get response." });
    }

public async Task<BaseResponse<string>> SignInWithGoogle(GoogleLoginDto model) 
{
    var response = await GoogleSignIn(model);
    if(response.Status == RequestExecution.Successful)
        return new  BaseResponse<string>(response.ResponseMessage,response.TotalCount,response.ResponseMessage);
    else if (response.Status == RequestExecution.Failed)
        return new BaseResponse<string>(response.ResponseMessage,response.Errors);
    else
        return new BaseResponse<string>( "Failed");
}

}


public static class GoogleAuthEndpoint
{
    public static void MapGoogleAuth(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/google",async Task<object>(
            [FromBody] GoogleLoginDto googleLogin,
            [FromServices] GoogleAuthService authService)=>
        {
            try
            {
                return await authService.SignInWithGoogle(googleLogin);
            }
            catch (Exception ex)
            {
                return "Some error";
            }
        }).WithTags("Oauth");
    }
} 




