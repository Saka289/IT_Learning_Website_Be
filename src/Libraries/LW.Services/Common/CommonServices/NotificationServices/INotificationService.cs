using LW.Shared.DTOs.Notification;
using LW.Shared.SeedWork;

namespace LW.Services.Common.CommonServices.NotificationServices;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetAllNotificationByUser(string userId);
    Task<NotificationDto> GetNotificationById(int id);
    Task<NotificationDto> CreateNotification(NotificationCreateDto competitionCreateDto);
    Task<bool> DeleteNotification(int id);
    Task<bool> DeleteAllNotificationOfUser(string userId);
    Task<int> GetNumberNotificationOfUser(string userId);
}