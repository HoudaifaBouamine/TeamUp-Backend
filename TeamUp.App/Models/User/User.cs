using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Models;

public partial class User : IdentityUser<Guid>
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string Handler {get; set;} = string.Empty;
    private string _displayName = string.Empty;
    public string DisplayName
    {
        get
        {
            return _displayName; 
        }
        set 
        { 
            value = value.Trim();

            if(!string.IsNullOrWhiteSpace(value)) 
                _displayName = value; 
        }
    }
    
    // public override string? Email { get; set; }
    public VerificationCode? EmailVerificationCode { get; private set; }
    public VerificationCode? PasswordRestCode { get; private set; }
    public string? PasswordResetToken { get; private set; } 
    public float Rate { get; private set; } = MaxRate;
    public string ProfilePicture { get; set; } = "https://i.ibb.co/5vC2qyP/unknown.jpg";
    public string? FullAddress { get; private set; }


    // private List<Project> _projects { get; set; } = [];
    // public IEnumerable<Project> Projects => _projects.AsReadOnly();
    // private List<UsersProject> _usersProjects { get; set; } = [];

    
    // public IEnumerable<UsersProject> UsersProjects => _usersProjects.AsReadOnly();
    
    public List<UsersProject> UsersProjects { get; set; } = [];
    public List<Project> Projects { get; set; } = [];
    public ICollection<UserSkill> UserSkills { get; set; } = [];
    public ICollection<Skill> Skills { get; set; } = [];
    
    public ICollection<Category> Categories { get; set; } = [];
}


// Seperating Fields and Methods to 2 classes to simplify the work
partial class User
{
    public bool SetPassword(string password)
    {
        PasswordHasher<User> hasher = new();

        if (this.PasswordHash is not null)
        {
            Log.Error("PasswordHash is not null, use ChangePassword function instead");
            return false;
        }

        this.PasswordHash = hasher.HashPassword(this, password);
        return true;
    }

    public bool CheckPassword(string password)
    {
        if (PasswordHash is null)
        {
            Log.Error("Password hash is null, password can not be checked");
            return false;
        }
        
        PasswordHasher<User> hasher = new();
        var result = hasher.VerifyHashedPassword(this, this.PasswordHash,password);

        if (result != PasswordVerificationResult.Failed)
            return true;

        return false;
    }
    public bool ChangePassword(string prevPassword, string newPassword)
    {
        if (this.PasswordHash is null) return false;
        
        PasswordHasher<User> hasher = new();
        var result = hasher.VerifyHashedPassword(this, this.PasswordHash, prevPassword);
        
        if (result == PasswordVerificationResult.Success)
        {
             var newPasswordHash = hasher.HashPassword(this, newPassword);
             this.PasswordHash = newPasswordHash;
             return true;
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            Log.Error("result == PasswordVerificationResult.SuccessRehashNeeded");
        }

        return false;
    }
    public bool SetDisplayName(string displayName)
    {
        DisplayName = displayName;
        return displayName == DisplayName;
    }
    public string? GetPasswordResetCode()
    {
        return PasswordRestCode?.Code;
    }
    public void CreateNewPasswordResetCode()
    {
        PasswordRestCode = VerificationCode.CreatePasswordResetCode();
    }

    public string? GetEmailVerificationCode()
    {
        return PasswordRestCode?.Code;
    }
    public void CreateNewEmailVerificationCode()
    {
        EmailVerificationCode = VerificationCode.CreateEmailVerificationCode();
    }

    public void Update(string? firstName, string? lastName,string? email, string? displayName, string? handler, string? fullAddress, string? profilePicture)
    {
        this.FirstName = firstName ?? this.FirstName;
        this.LastName = lastName ?? this.LastName;
        this.DisplayName = displayName?? this.DisplayName;
        this.Handler = handler?? this.Handler;
        this.FullAddress = fullAddress?? this.FullAddress;
        this.ProfilePicture = profilePicture?? this.ProfilePicture;
        this.Email = email?? this.Email;
    }
    
    public void UpdateSkills(List<Skill> skills)
    {
        this.Skills = skills;
    }

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


    public bool SetPasswordRestToken(string token)
    {
        PasswordResetToken = token;
        return true;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(new {Id = this.Id, Email = this.Email, DisplayName = this.DisplayName});
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

