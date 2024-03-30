using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Models;

namespace Authentication.UserManager;

public class CustomUserManager(
    IUserStore<User> store, 
    IOptions<IdentityOptions> optionsAccessor, 
    IPasswordHasher<User> passwordHasher, 
    IEnumerable<IUserValidator<User>> userValidators, 
    IEnumerable<IPasswordValidator<User>> passwordValidators, 
    ILookupNormalizer keyNormalizer, 
    IdentityErrorDescriber errors, 
    IServiceProvider services, 
    ILogger<UserManager<User>> logger
    ) : UserManager<User>(
        store,
        optionsAccessor, 
        passwordHasher, 
        userValidators, 
        passwordValidators, 
        keyNormalizer, 
        errors, 
        services, 
        logger)
{

    public override Task<bool> VerifyUserTokenAsync(User user, string tokenProvider, string purpose, string code)
    {
        ThrowIfDisposed();

        if(purpose == ResetPasswordTokenPurpose)
        {
            if(user.PasswordRestCode?.IsValid(code) is true)
            {
                return Task.FromResult(true);
            }
        
            return Task.FromResult(false);
        }
        else if(purpose == ConfirmEmailTokenPurpose)
        {
            if(user.EmailVerificationCode?.IsValid(code) is true)
            {
                return Task.FromResult(true);
            }
        
            return Task.FromResult(false);
        }
        return Task.FromResult(false);
    }
}
