using LW.Cache;
using LW.Cache.Interfaces;
using LW.Shared.Constant;
using LW.Shared.DTOs.HubConntectionDto;
using Microsoft.AspNetCore.SignalR;

namespace LW.Infrastructure.Hubs;

public class NotificationHub : Hub
{
    private readonly IRedisCache<HubConnection> _redis;

    public NotificationHub(IRedisCache<HubConnection> redis)
    {
        _redis = redis;
    }

    public override Task OnConnectedAsync()
    {
        Clients.Caller.SendAsync("OnConnected");
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // var hubConnection1 = _context.HubConnections.FirstOrDefault(con => con.ConnectionId == Context.ConnectionId);
        var hubConnection = await _redis.GetStringKey($"hub:{Context.ConnectionId}");
        if (hubConnection != null)
        {
            // _context.HubConnections.Remove(hubConnection);
            // _context.SaveChanges();
            await _redis.RemoveStringKey(hubConnection.Key);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SaveUserConnection(string userId)
    {
        var connectionId = Context.ConnectionId;
        HubConnection hubConnection = new HubConnection()
        {
            Key = $"hub:{connectionId}",
            ConnectionId = connectionId,
            UserId = userId
        };
        //_context.HubConnections.Add(hubConnection);
        // await _context.SaveChangesAsync();
        await _redis.SetStringKey($"hub:{hubConnection.ConnectionId}" ,hubConnection);
    }

    // public async Task SendNotificationToAll(string message)
    // {
    //     await Clients.All.SendAsync("ReceivedNotification",message);
    // }
    //
    // public async Task SendNotificationToClient1(string message, string userId)
    // {
    //     var hubConnections = await _redis.GetAllKeysByPattern(RedisPatternConstant.PatternHub, "UserId", userId);
    //     if (hubConnections.Any())
    //     {
    //         foreach (var hubConnection in hubConnections)
    //         {
    //             await Clients.Client(hubConnection.ConnectionId)
    //                 .SendAsync("ReceivedPersonalNotification", message, userId);
    //         }
    //     }
    // }
}