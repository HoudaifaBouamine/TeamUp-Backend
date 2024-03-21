using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
namespace Authentication.UserManager;

class CustomUserManager : UserManager<User>
{
    private readonly AppDbContext db;
    public CustomUserManager(AppDbContext db,IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        this.db = db;
    }

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
