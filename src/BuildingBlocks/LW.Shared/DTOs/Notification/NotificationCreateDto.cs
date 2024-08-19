using LW.Shared.Enums;

namespace LW.Shared.DTOs.Notification;

public class NotificationCreateDto
{
    public ENotificationType NotificationType { get; set; } // 1-All 2-Personal
    public string? UserSendId { get; set; }
    public string? UserSendName { get; set; }
    public string? UserReceiveId { get; set; }
    public string? UserReceiveName { get; set; }
    public string? Description { get; set; }
    public bool? IsRead { get; set; }
    public string? Link { get; set; }
}