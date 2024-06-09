using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;

namespace Features;

public class FirebaseNotificationService : INotificationService
{
    public async Task<bool> SendJoinRequestNotification(AppDbContext db, User user ,JoinRequestNotificationData data)
    {
        var title = data.senderName;
        var body = data.message;
        
        var session = await db.FireBaseNotificationSessions.FirstOrDefaultAsync(s => s.UserId == user.Id);

        var ow = JsonSerializer.Serialize(db.FireBaseNotificationSessions.ToList());
        Log.Error($"FireBaseNotificationSessions : {ow}");
        Log.Error($"session : {session}");
        
        if (session is null) return false;
        
        var sent = await FireBaseNotification.SendMessageAsync(session.SessionToken,title,body,data);
        
        Log.Error($"sent : {sent}");

        return sent;
    }
}

public interface INotificationService
{
    Task<bool> SendJoinRequestNotification(AppDbContext db, User user, JoinRequestNotificationData data);
}

public record JoinRequestNotificationData
{
    public string requestId { get; set; }
    public Guid senderId { get; set; }
    public string senderName { get; set; }
    public string senderPicture { get; set; }
    public string message { get; set; }
    public string projectId { get; set; }
    public string projectTitle { get; set; }

    public JoinRequestNotificationData()
    {
        
    }

    public JoinRequestNotificationData(ProjectJoinRequest joinRequest)
    {
        requestId = joinRequest.Id.ToString();
        senderId = joinRequest.User.Id;
        senderName = joinRequest.User.DisplayName;
        senderPicture = joinRequest.User.ProfilePicture;
        message = joinRequest.JoinMessage;
        projectId = joinRequest.ProjectPost.Id.ToString();
        projectTitle = joinRequest.ProjectPost.Title;
    }
}