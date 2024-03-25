using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Models {


[Table("Projects")]
public class Project
{
    public int Id { get; set;}
    [Required]
    public string ProjectName { get; set; } = String.Empty;
    // public string ProjectVersion { get; set; } may be i will add this column in the future , because we forget it  
    public string ProjectDescription { get; set; } = String.Empty ;
    public DateTime StartDateTime { get; set;}
    public DateTime EndDateTime { get; set;}
    public ChatRoom ChatRoomId { get; set;} = null!;


}



}