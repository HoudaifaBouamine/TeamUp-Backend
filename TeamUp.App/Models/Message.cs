using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Models {

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



}