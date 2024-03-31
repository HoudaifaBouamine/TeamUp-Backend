namespace Authentication.IdentityApi;
partial class AuthEndpoints
{
    public record UserInfoDto(string Id, string UserName,string Email,bool IsEmailConfirmed);
    public record UserLoginDto(string Email,string Password);
    public record UserRegisterDto(string DisplayName,string Email,string Password);
    public record EmailConfirmationDto(string Email,string Code);
    public record GetResetPasswordTokenRequestDto(string Email,string Code);
    public record GetResetPasswordTokenResponseDto(string ResetToken);
    public record ResetPasswordByTokenRequestDto(string Email,string ResetToken,string NewPassword);

}