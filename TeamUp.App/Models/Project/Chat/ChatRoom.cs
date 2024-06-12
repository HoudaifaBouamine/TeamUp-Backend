namespace Models;

public class ChatRoom
{
    public Guid Id { get; set; }        
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public ICollection<Message> Messages{ get; set; } = [];
    public ChatRoom(){}
}
