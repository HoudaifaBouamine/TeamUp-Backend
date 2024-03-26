using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Models;

public class Project
{
    public int Id { get; set;}
    [Required]
    public string ProjectName { get; set; } = string.Empty;
    // public string ProjectVersion { get; set; } may be i will add this column in the future , because we forget it  
    public string ProjectDescription { get; set; } = string.Empty ;
    public DateOnly StartDateTime { get; set;}
    public DateOnly? EndDateTime { get; set;} = null;
    public ChatRoom ChatRoom { get; set;} = null!;

    // Project have many users, users have many projects, so we declare a list of users in project, and list of projects in user 
    public List<User> Users { get; set; } = [];
    public List<UsersProject> ProjectsUsers { get; set; } = [];
}