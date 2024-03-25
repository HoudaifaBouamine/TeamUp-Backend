// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using System.Security.Cryptography;
// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Identity;
// NOTE (ISHAK) : see the ER diagram and transfer it to models in this file, then seperate the models to different files inside Models folder


//namespace Models;



// public class User : IdentityUser
// {
//     public string? FirstName { get; set; }
//     public string? LastName { get; set; }
//     public string Handler {get;set;} = string.Empty;
//     [Required] public string DisplayName { get; set; } = string.Empty;
//     public VerificationCode? EmailVerificationCode { get; set; }
//     public VerificationCode? PasswordRestCode { get; set; }
//     public float Rate { get; set; } = MaxRate;
//     public string ProfilePicture { get; set; } = "https://i.ibb.co/5vC2qyP/unknown.jpg";
//     public string? FullAddress { get; set; } = null!;

//     const int MaxRate = 5;

//     public string GetFullName()
//     {
//         return CapitalizeFirstLetter(FirstName) + " " + CapitalizeFirstLetter(LastName);
//     }

//     private static string CapitalizeFirstLetter(string? input)
//     {
//         if (string.IsNullOrEmpty(input))
//         {
//             return "";
//         }

//         // Convert the input string to lowercase
//         input = input.ToLower();

//         // Convert the first character to uppercase
//         char firstChar = char.ToUpper(input[0]);

//         // Concatenate the first uppercase character with the rest of the string
//         string result = firstChar + input.Substring(1);

//         return result;
//     }
// }


// [Table("Projects")]
// public class Project
// {
//     public int Id { get; set;}
//     [Required]
//     public string ProjectName { get; set; } = String.Empty;
//     // public string ProjectVersion { get; set; } may be i will add this column in the future , because we forget it  
//     public string ProjectDescription { get; set; } = String.Empty ;
//     public DateTime StartDateTime { get; set;}
//     public DateTime EndDateTime { get; set;}
//     public ChatRoom ChatRoomId { get; set;} = null!;


// }

// add other classes


// [Table("Messages")]
// public class Message
// {
//     public int Id { get; set; }
//     public string Text { get; set; } = string.Empty; 
//     public DateTime DateTime { get; set; }

//     [Required]
//     public required ChatRoom ChatRoomId { get; set; }
    
//     public required User UserID { get; set; }
//     public bool pinned { get; set; } 

// }

// [Table("ChatRooms")]
// public class ChatRoom
// {
//     public int Id { get; set; }
    
//     public required Project ProjectID { get; set; } 
// }

// [Table("ProjectViews")]
// public class ProjectView
// {
//     public int Id { get; set;}
//     public string Text { get; set; } = string.Empty;
//     public byte stars { get; set; } 
//     public User? ReviewerUserId { get; set; } 
//     public Project? ReviewedProjectId { get; set; }
// }

// [Table("UsersAccessCategory")]
// public class UsersProject
// {
//     public int Id { get; set; }
//     public required User UserId { get; set; }
//     public required Category CategoryId { get; set; }

// }

// [Table("Categories")]
// public class Category
// {
//     public int Id { get; set; }
//     public string Name { get; set; } = string.Empty; 

// }

// [Table("ProjectsCategory")]
// public class ProjectCategory
// {
//     public int Id { get; set; }
//     public required Category CategoryId { get; set; } 
//     public required Project ProjectId { get; set; }

// }

// [Table("UsersProject")]
// public class UserProject
// {
//     public int Id { get; set; }
//     public required User UserId { get; set; }

//     public required Project ProjectId { get; set; }
// }


// [Table("UsersReviews")]
// public class UserReview
// {
//     public int Id { get; set; }
//     public string Text { get; set; } = string.Empty;  
//     public required User ReviewerUserId { get; set; }
//     public required User ReviewedUserId { get; set; }

//     public byte stars { get; set; }
// }

// [Table("Skills")]
// public class Skill
// {
//     public int Id { get; set; }
//     public required string Name { get; set; }
// }

// [Table("UsersSkills")]
// public class UserSkill
// {
//     public int Id { get; set; }
//     public required User UserId { get; set; }
//     public required Skill SkillId { get; set;}
// }


    
// all things is done here 