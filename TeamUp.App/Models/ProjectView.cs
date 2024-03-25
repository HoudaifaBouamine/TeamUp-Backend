using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity ; 


namespace Models {
 
[Table("ProjectViews")]
public class ProjectView
{
    public int Id { get; set;}
    public string Text { get; set; } = string.Empty;
    public byte stars { get; set; } 
    public User? ReviewerUserId { get; set; } 
    public Project? ReviewedProjectId { get; set; }
}


}