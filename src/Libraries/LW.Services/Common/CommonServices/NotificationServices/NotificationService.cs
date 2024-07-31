using AutoMapper;
using LW.Cache.Interfaces;
using LW.Data.Entities;
using LW.Data.Repositories.NotificationRepositories;
using LW.Infrastructure.Hubs;
using LW.Shared.Constant;
using LW.Shared.DTOs.HubConntectionDto;
using LW.Shared.DTOs.Notification;
using LW.Shared.Enums;
using Microsoft.AspNetCore.SignalR;

namespace LW.Services.Common;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    //private readonly INotificationHub _notificationHub;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly IRedisCache<HubConnection> _redis;


    public NotificationService(INotificationRepository notificationRepository, IMapper mapper, IHubContext<NotificationHub> notificationHub, IRedisCache<HubConnection> redis)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _notificationHub = notificationHub;
        _redis = redis;
    }

    public async Task<IEnumerable<NotificationDto>> GetAllNotificationByUser(string userId)
    {
        var listNoti = await _notificationRepository.GetAllNotificationByUser(userId);
        if (!listNoti.Any())
        {
            return null;
        }

        var result = _mapper.Map<IEnumerable<NotificationDto>>(listNoti);
        return result;
    }

    public async Task<NotificationDto> GetNotificationById(int id)
    {
        var noti = await _notificationRepository.GetNotificationById(id);
        if (noti == null)
        {
            return null;
        }

        var result = _mapper.Map<NotificationDto>(noti);
        return result;
    }

    public async Task<NotificationDto> CreateNotification(NotificationCreateDto notificationCreateDto)
    {
        var createNotification = _mapper.Map<Notification>(notificationCreateDto);
        var noti = await _notificationRepository.CreateNotification(createNotification);
        var result = _mapper.Map<NotificationDto>(noti);
        // Gửi notification = hub 
        if (notificationCreateDto.NotificationType == ENotificationType.All)
        {
            _notificationHub.Clients.All.SendAsync("ReceivedNotification",notificationCreateDto.Description);
        }
        else if(notificationCreateDto.NotificationType == ENotificationType.Personal)
        {
            var hubConnections = await _redis.GetAllKeysByPattern(RedisPatternConstant.PatternHub, "UserId", notificationCreateDto.UserReceiveId);
            if (hubConnections.Any())
            {
                foreach (var hubConnection in hubConnections)
                {
                    await _notificationHub.Clients.Client(hubConnection.ConnectionId)
                        .SendAsync("ReceivedPersonalNotification", notificationCreateDto.Description, notificationCreateDto.UserReceiveId);
                }
            }
        }
        return result;
    }

    public async Task<bool> DeleteNotification(int id)
    {
        var noti = await _notificationRepository.GetNotificationById(id);
        if (noti == null)
        {
            return false;
        }

        await _notificationRepository.DeleteNotification(id);
        return true;
    }

    public async Task<bool> DeleteAllNotificationOfUser(string userId)
    {
        var listNoti = await _notificationRepository.GetAllNotificationByUser(userId);
        if (!listNoti.Any())
        {
            return false;
        }

        var result = await _notificationRepository.DeleteRangeNotification(listNoti);
        return true;
    }

    public async Task<int> GetNumberNotificationOfUser(string userId)
    {
        var listNoti = await _notificationRepository.GetAllNotificationByUser(userId);
        if (!listNoti.Any())
        {
            return 0;
        }

        return listNoti.Count();
    }
}