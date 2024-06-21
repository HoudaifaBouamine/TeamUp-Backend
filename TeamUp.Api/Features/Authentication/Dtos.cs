using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Authentication.IdentityApi;
partial class AuthEndpoints
{
    public record UserInfoResponseDto
    (
        Guid Id, 
        string UserName,
        [EmailAddress]
        string Email,
        bool IsEmailConfirmed
    );
    public record UserLoginRequestDto
    (
        [EmailAddress]
        /// <example>string@gmail.com</example>
        string Email,
        /// <example>1234</example>
        string Password
    );
    public record UserRegisterRequestDto
    (
        string DisplayName,
        [EmailAddress]
        string Email,
        string Password
    );
    public record EmailConfirmationRequestDto
    (
        [EmailAddress] 
        string Email,
        string Code
    );
    public record GetResetPasswordTokenRequestDto
    (
        [EmailAddress] 
        string Email,
        string Code
    );
    public record GetResetPasswordTokenResponseDto
    (
        string ResetToken
    );
    public record ResetPasswordByTokenRequestDto
    (
        [EmailAddress] 
        string Email,
        string ResetToken,
        string NewPassword
    );

}