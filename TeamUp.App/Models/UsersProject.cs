using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity ; 


namespace Models {


[Table("UsersAccessCategory")]
public class UsersProject
{
    public int Id { get; set; }
    public required User UserId { get; set; }
    public required Category CategoryId { get; set; }

}

}

