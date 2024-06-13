using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

[Table("Messages")]
public class Message
{

    [Key] public int Id { get; set; }
    [MinLength(1)] public string Text { get; set; } = string.Empty; 
    public DateTime SendTime { get; set; }
    [Required] public Guid ChatRoomId { get; set; }
    [Required] public Guid UserId { get; set; }
    public bool IsPinned { get; set; }

    protected Message() { }
    
    public Message(Guid chatRoom, Guid userId, string text)
    {
        this.ChatRoomId = chatRoom;
        this.UserId = userId;
        this.Text = text;
        this.IsPinned = false;
        this.SendTime = DateTime.UtcNow;
    }
}
