using LW.Shared.DTOs.Notification;
using LW.Shared.SeedWork;

namespace LW.Services.NotificationServices;

public interface INotificationService
{
    Task<ApiResult<PagedList<NotificationDto>>> GetAllNotificationByUser(string userId,PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<PagedList<NotificationDto>>> GetAllNotificationNotReadByUser(string userId,PagingRequestParameters pagingRequestParameters);
    Task<ApiResult<NotificationDto>> GetNotificationById(int id);
    Task<ApiResult<NotificationDto>> CreateNotification(NotificationCreateDto competitionCreateDto);
    Task<ApiResult<NotificationDto>> UpdateStatusNotification(int id);
    Task<ApiResult<bool>> DeleteNotification(int id);
    Task<ApiResult<bool>> DeleteAllNotificationOfUser(string userId);
    Task<ApiResult<bool>> MarkAllAsReadAsync(string userId);
    Task<ApiResult<int>> GetNumberNotificationOfUser(string userId);
}