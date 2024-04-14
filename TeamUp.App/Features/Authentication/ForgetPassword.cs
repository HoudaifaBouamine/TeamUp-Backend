using Authentication.UserManager;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Authentication.IdentityApi;

partial class AuthEndpoints
{
    async Task<Results<Ok,NotFound, ValidationProblem,StatusCodeHttpResult>> ForgetPasswordAsync
            ([FromBody] ForgotPasswordRequest resetRequest,
            [FromServices] AppDbContext db,
            [FromServices] CustomUserManager userManager)
        {
            var user = await db.Users.Include(u=>u.PasswordRestCode).FirstOrDefaultAsync(u=>u.Email == resetRequest.Email);

            if(user is null) return TypedResults.NotFound();
            
            if(user.PasswordRestCode is null) user.PasswordRestCode = VerificationCode.CreatePasswordResetCode();
            var code = user.PasswordRestCode.Code;
            
            await db.SaveChangesAsync();

            var success = await SendPasswordResetCodeEmailAsync(db,resetRequest.Email,VerificationCode.VerificationCodeTypes.PasswordRest);

            if(success)
                return TypedResults.Ok();
            else
                return TypedResults.StatusCode(500);

        }
    
}