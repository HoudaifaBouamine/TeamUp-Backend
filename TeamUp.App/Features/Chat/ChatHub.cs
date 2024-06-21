using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;
using SignalRSwaggerGen.Attributes;
using System.Collections.Concurrent;

namespace TeamUp.Features.Chat;

internal partial class ChatHub (AppDbContext db, UserManager<User> userManager) : Hub
{

    private static ConcurrentDictionary<string,Guid> UsersIds = new();

    public async Task Hi(string name)
    {
        await Clients.All.SendAsync("GreatingRecieved",$"Hi {name}");
    }

    [Authorize]
    public async Task<bool> SendRoomMessage(Guid roomId, string message)
    {
        var userId = UsersIds.GetValueOrDefault(Context.ConnectionId);

        if (userId == default)
        {
            Log.Error("[Features/Chat/ChatHub.cs] : Can not find userId");
            return false;
        }

        var isAllowed = await db.UsersProjects
            .Where(up => up.UserId == userId)
            .AnyAsync(up => up.Project.ChatRoomId == roomId);

        if (!isAllowed)
        {
            Log.Information("[Features/Chat/ChatHub.cs] : This user can not send message to this room");
            return false;
        }

        var newMessage = new Message(roomId, userId, message);
        db.Messages.Add(newMessage);
        var rowsChanged = await db.SaveChangesAsync();

        if (rowsChanged <= 0) return false;
        
        await Clients.OthersInGroup(roomId.ToString())
            .SendAsync("RecieveRoomMessage",
                new MessageDto
                (
                    Message    : newMessage.Text,
                    UserId     : newMessage.UserId,
                    ChatRoomId : newMessage.ChatRoomId,
                    SendTime   : newMessage.SendTime
                )
            );

        return true;
    }

    record MessageDto(string Message, Guid UserId, Guid ChatRoomId, DateTime SendTime);

    public override async Task OnConnectedAsync()
    {
        if (!Context.User.IsAuthenticated()) return;

        var user = await userManager.GetUserAsync( Context.User! );

        bool isAdded = UsersIds.TryAdd(Context.ConnectionId, user!.Id);

        if (!isAdded)
        {
            Log.Error("[Features/Chat/ChatHub.cs] : Failed to add userId to dictionary");
            return;
        }

        var chatRoomsIds = await db.UsersProjects
            .AsNoTracking()
            .Where(up => up.UserId == user.Id)
            .Include(up=>up.Project)
            .Select(up => up.Project.ChatRoomId)
            .ToListAsync();

        chatRoomsIds.ForEach(chatId =>
        {
            Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        });

        await db.Users.Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync(p => p.SetProperty(u => u.IsConnected, true));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        UsersIds.Remove(Context.ConnectionId, out Guid id);

        await db.Users.Where(u => u.Id == id)
            .ExecuteUpdateAsync(p => p.SetProperty(u => u.IsConnected, false));
    }
}