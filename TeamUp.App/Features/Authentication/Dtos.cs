using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Authentication.IdentityApi;
partial class AuthEndpoints
{
    public record UserInfoResponseDto
    (
        string Id, 
        string UserName,
        string Email,
        bool IsEmailConfirmed
    );
    public record UserLoginRequestDto
    (
        [EmailAddress]
        string Email,
        string Password
    );
    public record UserRegisterRequestDto
    (
        string DisplayName,
        [EmailAddress]
        string Email,
        string Password
    );
    public record EmailConfirmationRequestDto(string Email,string Code);
    public record GetResetPasswordTokenRequestDto(string Email,string Code);
    public record GetResetPasswordTokenResponseDto(string ResetToken);
    public record ResetPasswordByTokenRequestDto(string Email,string ResetToken,string NewPassword);

}