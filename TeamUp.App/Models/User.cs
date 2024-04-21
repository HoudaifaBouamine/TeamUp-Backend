using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Models;





public partial class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Handler {get;set;} = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public VerificationCode? EmailVerificationCode { get; set; }
    public VerificationCode? PasswordRestCode { get; set; }
    public string? PasswordResetToken { get; set; } 
    public float Rate { get; set; } = MaxRate;
    public string ProfilePicture { get; set; } = "https://i.ibb.co/5vC2qyP/unknown.jpg";
    public string? FullAddress { get; set; }

    public ICollection<Project> Projects { get; set; } = [];
    public ICollection<UsersProject> UsersProjects { get; set; } = [];
    public ICollection<UserSkill> UserSkills { get; set; } = [];
    public ICollection<Skill> Skills { get; set; } = [];
}


// Seperating Fields and Methods to 2 classes to simplify the work
partial class User
{

    public User(string firstName, string lastName, string email, string profilePicture)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DisplayName = FirstName + " " + LastName;
        Email = email.Trim();
        UserName = Email;
        ProfilePicture = profilePicture.Trim();
    }

    public User(string displayName,string email)
    {
        DisplayName = displayName.Trim();
        Email = email.Trim();
        UserName = Email;
    }
    public User() {}
    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public static bool operator == (User user1,User user2)
    {
        return user1.Equals(user2);
    }

    public override bool Equals(object? obj)
    {
        User? user2 = (User?) obj;
        User? user1 = this;

        if(user1 is null && user2 is null)
            return true;

        if(user1 is null || user2 is null)
            return false;
        
        if(base.Equals(user2))
        {
            return true;
        }

        if(user1.Id == user2.Id)
        {
            return true;
        }

        return false;
    }
    public static bool operator != (User user1,User user2)
    {
        return ! user1.Equals(user2);
    }
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

