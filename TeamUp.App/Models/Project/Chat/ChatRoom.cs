using Microsoft.CodeAnalysis;

namespace Models;

public partial class ChatRoom
{
    public int Id { get; set; }        
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public ICollection<Message> Messages{ get; set; } = [];
}

partial class ChatRoom
{
    public ChatRoom(){}
}