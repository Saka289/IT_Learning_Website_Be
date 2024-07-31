using LW.Data.Common;
using LW.Data.Common.Interfaces;
using LW.Data.Entities;
using LW.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LW.Data.Repositories.NotificationRepositories;

public class NotificationRepository : RepositoryBase<Notification, int>, INotificationRepository
{
    public NotificationRepository(AppDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<Notification> CreateNotification(Notification notification)
    {
        await CreateAsync(notification);
        return notification;
    }

    public async Task<Notification> UpdateNotification(Notification notification)
    {
        await UpdateAsync(notification);
        return notification;
    }

    public async Task<bool> DeleteNotification(int id)
    {
        var noti = await GetNotificationById(id);
        if (noti == null)
        {
            return false;
        }

        await DeleteAsync(noti);
        return true;
    }

    public async Task<bool> DeleteRangeNotification(IEnumerable<Notification> notifications)
    {
        await DeleteListAsync(notifications);
        return true;
    }

    public async Task<Notification> GetNotificationById(int id)
    {
        return await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Notification>> GetAllNotificationByUser(string userId)
    {
        return await FindByCondition(x => x.UserReceiveId == userId).OrderByDescending(x => x.NotificationTime)
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusOfNotification(int id)
    {
        var noti = await GetNotificationById(id);
        if (noti == null)
        {
            return false;
        }

        noti.IsRead = true;
        await UpdateNotification(noti);
        return true;
    }
}