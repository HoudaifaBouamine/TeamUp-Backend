namespace Models;

public class Follow
{
    public int Id { get; set; }
    public User Follower { get; set; }
    public User Followee { get; set; }
}