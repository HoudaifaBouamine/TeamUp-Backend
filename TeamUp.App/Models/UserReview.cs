using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity ; 

namespace Models {        

[Table("UsersReviews")]
public class UserReview
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;  
    public required User ReviewerUserId { get; set; }
    public required User ReviewedUserId { get; set; }

    public byte stars { get; set; }
}


}