namespace LW.Infrastructure.Hubs;

public interface INotificationHub 
{
    Task SendNotificationToAll(string message);
    Task SendNotificationToClient(string message, string userId);
}