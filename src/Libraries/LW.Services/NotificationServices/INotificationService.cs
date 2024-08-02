using LW.Shared.DTOs.Notification;
using LW.Shared.SeedWork;

namespace LW.Services.NotificationServices;

public interface INotificationService
{
    Task<ApiResult<IEnumerable<NotificationDto>>> GetAllNotificationByUser(string userId);
    Task<ApiResult<NotificationDto>> GetNotificationById(int id);
    Task<ApiResult<NotificationDto>> CreateNotification(NotificationCreateDto competitionCreateDto);
    Task<ApiResult<bool>> DeleteNotification(int id);
    Task<ApiResult<bool>> DeleteAllNotificationOfUser(string userId);
    Task<ApiResult<int>> GetNumberNotificationOfUser(string userId);
}