using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks; 
// NOTE (ISHAK) : see the ER diagram and transfer it to models in this file, then seperate the models to different files inside Models folder


namespace Models
{

    // [Table("Users")]
    // public class User
    // {   
    //     public int Id { get; set; }

    //     [Required]
    //     public string FirstName { get; set; } = String.Empty;
    //     [Required]
    //     public string LastName { get; set; } = String.Empty;
    //     [Required]
    //     public string Email { get; set; } = String.Empty;
    //     [Required]
    //     public string HashedPassword { get; set; } = String.Empty; 
    //     public string? Handle { get; set;}
    //     public string? PdfUrlCv { get; set;}

    //     public string? ImageUrl { get; set; }
    //     public float Rank { get; set; }

    
    //     /// complete the rest
    // }


    [Table("Projects")]
    public class Project
    {
        public int Id { get; set;}
        [Required]
        public string ProjectName { get; set; } = String.Empty;
       // public string ProjectVersion { get; set; } may be i will add this column in the future , because we forget it  
        public string ProjectDescription { get; set; } = String.Empty ;

        public string Email { get; set; } = String.Empty ;// what f email is doing here
        public DateTime StartDateTime { get; set;}
        public DateTime EndDateTime { get; set;}
        public required ChatRoom ChatRoomId { get; set;}


    }

    // add other classes


    [Table("Messages")]
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty; 
        public DateTime DateTime { get; set; }

        [Required]
        public required ChatRoom ChatRoomId { get; set; }
        
        public required User UserID { get; set; }
        public bool pinned { get; set; } 

    }

    [Table("ChatRooms")]
    public class ChatRoom
    {
        public int Id { get; set; }
        
        public required Project ProjectID { get; set; } 
    }

    [Table("ProjectViews")]
    public class ProjectView
    {
        public int Id { get; set;}
        public string Text { get; set; } = string.Empty;
        public byte stars { get; set; } 
        public User? ReviewerUserId { get; set; } 
        public Project? ReviewedProjectId { get; set; }
    }

    [Table("UsersAccessCategory")]
    public class UsersProject
    {
        public int Id { get; set; }
        public required User UserId { get; set; }
        public required Category CategoryId { get; set; }

    }

    [Table("Categories")]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 

    }

    [Table("ProjectsCategory")]
    public class ProjectCategory
    {
        public int Id { get; set; }
        public required Category CategoryId { get; set; } 
        public required Project ProjectId { get; set; }

    }

    [Table("UsersProject")]
    public class UserProject
    {
        public int Id { get; set; }
        public required User UserId { get; set; }

        public required Project ProjectId { get; set; }
    }


    [Table("UsersReviews")]
    public class UserReview
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;  
        public required User ReviewerUserId { get; set; }
        public required User ReviewedUserId { get; set; }

        public byte stars { get; set; }
    }

    [Table("Skills")]
    public class Skill
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    [Table("UsersSkills")]
    public class UserSkill
    {
        public int Id { get; set; }
        public required User UserId { get; set; }
        public required Skill SkillId { get; set;}
    }
}

    
