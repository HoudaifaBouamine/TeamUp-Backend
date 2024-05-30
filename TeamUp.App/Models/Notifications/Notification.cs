using System.ComponentModel.DataAnnotations;

namespace Models;

public class Notification
{
    public int Id { get; set; }
    public string Content { get; set; }
    public Guid? NotificationSourceUserId { get; set; }
    
    [AllowedValues("UserGetAcceptedToJoinProject","NewMemberJoinProject","MentorReciveJoinRequest")]
    public string Purpos { get; set; }
    public object PurposData { get; set; }
}