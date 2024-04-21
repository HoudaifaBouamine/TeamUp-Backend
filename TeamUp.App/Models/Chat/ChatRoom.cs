namespace Models;

public class ChatRoom
{
    public int Id { get; set; }        
    public Project Project { get; set; } = null!;
}