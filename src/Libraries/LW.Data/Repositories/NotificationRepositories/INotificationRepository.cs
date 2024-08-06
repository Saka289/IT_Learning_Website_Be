using LW.Data.Common.Interfaces;
using LW.Data.Entities;

namespace LW.Data.Repositories.NotificationRepositories;

public interface INotificationRepository: IRepositoryBase<Notification,int>
{
    Task<Notification> CreateNotification(Notification notification);
    Task<Notification> UpdateNotification(Notification notification);
    Task<bool> DeleteNotification(int id);
    Task<bool> DeleteRangeNotification(IEnumerable<Notification> notifications);
    Task<Notification> GetNotificationById(int id);
    Task<IEnumerable<Notification>> GetAllNotificationByUser(string userId);
    Task<IEnumerable<Notification>> GetAllNotificationNotReadByUser(string userId);
    Task<bool> UpdateStatusOfNotification(int id);
}