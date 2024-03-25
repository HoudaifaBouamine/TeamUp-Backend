using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Models {





public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Handler {get;set;} = string.Empty;
    [Required] public string DisplayName { get; set; } = string.Empty;
    public VerificationCode? EmailVerificationCode { get; set; }
    public VerificationCode? PasswordRestCode { get; set; }
    public float Rate { get; set; } = MaxRate;
    public string ProfilePicture { get; set; } = "https://i.ibb.co/5vC2qyP/unknown.jpg";
    public string? FullAddress { get; set; } = null!;

    const int MaxRate = 5;

    public string GetFullName()
    {
        return CapitalizeFirstLetter(FirstName) + " " + CapitalizeFirstLetter(LastName);
    }

    private static string CapitalizeFirstLetter(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "";
        }

        // Convert the input string to lowercase
        input = input.ToLower();

        // Convert the first character to uppercase
        char firstChar = char.ToUpper(input[0]);

        // Concatenate the first uppercase character with the rest of the string
        string result = firstChar + input.Substring(1);

        return result;
    }
}

};