using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Models;

public class FireBaseNotificationSession
{
    [Key]
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string SessionToken { get; set; }
    public DateTime CreatedAt { get; private init; }

    public FireBaseNotificationSession(Guid userId, string sessionToken)
    {
        UserId = userId;
        SessionToken = sessionToken;
    }
    protected FireBaseNotificationSession() { } // for ORM
}

// public class Notification
// {
//     public int Id { get; set; }
//     public string Content { get; set; }
//     public Guid? NotificationSourceUserId { get; set; }
//     
//     [AllowedValues("UserGetAcceptedToJoinProject","NewMemberJoinProject","MentorReciveJoinRequest")]
//     public string Purpos { get; set; }
//     public object PurposData { get; set; }
// }