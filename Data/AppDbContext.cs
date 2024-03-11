using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Models;

class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<VerificationCode> VerificationCodes { get; set; }
}

class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public VerificationCode VerificationCode { get; set; } = new VerificationCode();
} 
class VerificationCode
{
    public const int CodeLifeInMin = 1;

    public int Id { get; set; } 

    public string? Code { get; set; } = null; 

    public DateTime? ExpireTime { get; set; } = null;

    public VerificationCode()
    {
    }

    public bool IsExpired()
    {
        if(Code is null || ExpireTime is null)
            return true;

        return ExpireTime < DateTime.UtcNow;
    }

    public bool IsValied(string code)
    {
        if(IsExpired())
        {
            return false;
        }

        return this.Code == code;
    }

    public string GenerateRandomCode(string userId)
    {
        Random rand = new Random();
        int randomNumber = rand.Next(100000, 999999); 

        string combinedString = userId + randomNumber.ToString();

        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(combinedString));

            int hashInteger = BitConverter.ToInt32(bytes, 0);

            hashInteger = Math.Abs(hashInteger);

            int sixDigitNumber = hashInteger % 1000000;

            Code = sixDigitNumber.ToString("D6");
            ExpireTime = DateTime.UtcNow.AddMinutes(CodeLifeInMin);

            return Code;
        }
    }
}
