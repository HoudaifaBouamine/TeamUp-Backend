using Microsoft.EntityFrameworkCore;
using Models;

namespace Features;

public class FirebaseNotificationService : INotificationService
{
    public async Task<bool> SendJoinRequestNotification(AppDbContext db, User user ,JoinRequestNotificationData data)
    {
        var title = "Title";
        var body = "Body";

        var session = await db.FireBaseNotificationSessions.FirstOrDefaultAsync(s => s.UserId == user.Id);
        if (session is null) return false;
        
        var sent = await FireBaseNotification.SendMessageAsync(session.SessionToken,title,body,data);
        return sent;
    }
}

interface INotificationService
{
    Task<bool> SendJoinRequestNotification(AppDbContext db, User user, JoinRequestNotificationData data);
}

public record JoinRequestNotificationData
{
    public Guid SenderId { get; set; }
    public string SenderName { get; set; }
    public string SenderPicture { get; set; }
    public string Message { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; }

    public JoinRequestNotificationData()
    {
        
    }

    public JoinRequestNotificationData(ProjectJoinRequest joinRequest)
    {
        SenderId = joinRequest.User.Id;
        SenderName = joinRequest.User.DisplayName;
        SenderPicture = joinRequest.User.ProfilePicture;
        Message = joinRequest.JoinMessage;
        ProjectId = joinRequest.ProjectPost.Id;
        ProjectTitle = joinRequest.ProjectPost.Title;
    }
}