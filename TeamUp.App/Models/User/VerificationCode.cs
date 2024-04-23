using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Models;

public partial class VerificationCode
{
    [Key]
    public int Id { get;protected set; } 
    public string? Code { get; set; } = null; 
    public DateTime? ExpireTime { get; set; } = null;
    public int? CodeTriesCount { get; set; } = null;
    public VerificationCodeTypes VerificationCodeType { get; set; }
}

partial class VerificationCode
{
    private VerificationCode() {}


    public enum CodeMaxLifeInMin { EmailVerification = 10, PasswordRest = 5 }
    public enum CodeMaxTries { EmailVerification = 4, PasswordRest = 3 }
    public enum VerificationCodeTypes {EmailVerification = 1, PasswordRest = 2}


    public static VerificationCode CreateEmailVerificationCode()
    {
        
        var verificationCode = new VerificationCode()
        {
            VerificationCodeType = VerificationCodeTypes.EmailVerification,
            ExpireTime = DateTime.UtcNow.AddMinutes((int) CodeMaxLifeInMin.EmailVerification)
        };

        verificationCode.GenerateEmailVerificationCode();

        return verificationCode;
    }

    public static VerificationCode CreatePasswordResetCode()
    {
        var resetPasswordCode = new VerificationCode()
        {
            VerificationCodeType = VerificationCodeTypes.PasswordRest,
            ExpireTime = DateTime.UtcNow.AddMinutes((int) CodeMaxLifeInMin.PasswordRest),
        };

        resetPasswordCode.GeneratePasswordRestCode();

        return resetPasswordCode;
    }

    public static VerificationCode Create(string code, int minitesBeforeExpire)
    {
        return new VerificationCode()
        {
            Code = code,
            ExpireTime = DateTime.UtcNow.AddMinutes(minitesBeforeExpire)
        };
    }

    /// <summary>
    /// Get max code life time in minutes
    /// </summary>
    /// <returns>integer represent code max life in minutes</returns>
    public int GetCodeMaxLife()
    {
        return Convert.ToInt32(VerificationCodeType switch
        {
            VerificationCodeTypes.EmailVerification => CodeMaxLifeInMin.EmailVerification,
            VerificationCodeTypes.PasswordRest      => CodeMaxLifeInMin.PasswordRest,
            _ => 0,
        });
    }

    public int GetCodeMaxTries()
    {
        return Convert.ToInt32(VerificationCodeType switch
        {
            VerificationCodeTypes.EmailVerification => CodeMaxTries.EmailVerification,
            VerificationCodeTypes.PasswordRest      => CodeMaxTries.PasswordRest,
            _ => 0,
        });
    }

    public bool IsEmailVerificationCode()
    {
        return VerificationCodeType == VerificationCodeTypes.EmailVerification;
    }

    public bool IsPasswordRestCode()
    {
        return VerificationCodeType == VerificationCodeTypes.PasswordRest;
    }
    public bool IsExpired()
    {
        if(Code is null || ExpireTime is null)
            return true;

        return ExpireTime < DateTime.UtcNow;
    }

    public bool IsValid(string code)
    {
        if(IsExpired())
        {
            return false;
        }

        return this.Code == code;
    }

    
    public string GenerateEmailVerificationCode()
    {
        VerificationCodeType = VerificationCodeTypes.EmailVerification;
        Code = GenerateRandomCode(); // user userId to generate user speacific code for security perposes (may be deleted if I find that it is useless)
        ExpireTime = DateTime.UtcNow.AddMinutes(GetCodeMaxLife());
        CodeTriesCount = GetCodeMaxTries();

        return Code;
    }

    public string GeneratePasswordRestCode()
    {
        VerificationCodeType = VerificationCodeTypes.PasswordRest;
        Code = GenerateRandomCode(); // user userId to generate user speacific code for security perposes (may be deleted if I find that it is useless)
        ExpireTime = DateTime.UtcNow.AddMinutes(GetCodeMaxLife());
        CodeTriesCount = GetCodeMaxTries();

        return Code;
    }


    private static string GenerateRandomCode()
    {
        return new Random().Next(100000, 999999).ToString(); 
    }
}