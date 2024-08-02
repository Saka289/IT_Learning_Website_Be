using AutoMapper;
using LW.Cache.Interfaces;
using LW.Data.Entities;
using LW.Data.Repositories.NotificationRepositories;
using LW.Infrastructure.Hubs;
using LW.Shared.Constant;
using LW.Shared.DTOs.HubConntectionDto;
using LW.Shared.DTOs.Notification;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace LW.Services.NotificationServices;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly IRedisCache<HubConnection> _redis;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationService(INotificationRepository notificationRepository, IMapper mapper, IHubContext<NotificationHub> notificationHub, IRedisCache<HubConnection> redis, UserManager<ApplicationUser> userManager)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _notificationHub = notificationHub;
        _redis = redis;
        _userManager = userManager;
    }

    public async Task<ApiResult<IEnumerable<NotificationDto>>> GetAllNotificationByUser(string userId)
    {
        var listNotifications = await _notificationRepository.GetAllNotificationByUser(userId);
        if (!listNotifications.Any())
        {
            return new ApiResult<IEnumerable<NotificationDto>>(false);
        }

        var result = _mapper.Map<IEnumerable<NotificationDto>>(listNotifications);
        return new ApiResult<IEnumerable<NotificationDto>>(true, result, "Get all list notification successfully");
    }

    public async Task<ApiResult<NotificationDto>> GetNotificationById(int id)
    {
        var notification = await _notificationRepository.GetNotificationById(id);
        if (notification == null)
        {
            return new ApiResult<NotificationDto>(false, "Not found");
        }

        var result = _mapper.Map<NotificationDto>(notification);
        return new ApiResult<NotificationDto>(true, result, "Get notification by id success");
    }

    public async Task<ApiResult<NotificationDto>> CreateNotification(NotificationCreateDto notificationCreateDto)
    {
        var userSend = await _userManager.FindByIdAsync(notificationCreateDto.UserSendId);
        if (userSend == null)
        {
            return new ApiResult<NotificationDto>(false, "Not found user send notification");
        }
        var userReceive = await _userManager.FindByIdAsync(notificationCreateDto.UserReceiveId);
        if (userReceive == null)
        {
            return new ApiResult<NotificationDto>(false, "Not found user Receive notification");
        }
        var createNotification = _mapper.Map<Notification>(notificationCreateDto);
        var notification = await _notificationRepository.CreateNotification(createNotification);
        var result = _mapper.Map<NotificationDto>(notification);
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
        return new ApiResult<NotificationDto>(true, result, "Create notification successfully");
    }

    public async Task<ApiResult<bool>> DeleteNotification(int id)
    {
        var noti = await _notificationRepository.GetNotificationById(id);
        if (noti == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }

        await _notificationRepository.DeleteNotification(id);
        return new ApiResult<bool>(true, "Delete success");
    }

    public async Task<ApiResult<bool>> DeleteAllNotificationOfUser(string userId)
    {
        var listNotifications = await _notificationRepository.GetAllNotificationByUser(userId);
        if (!listNotifications.Any())
        {
            return new ApiResult<bool>(false);
        }

        var result = await _notificationRepository.DeleteRangeNotification(listNotifications);
        return new ApiResult<bool>(true);
    }

    public async Task<ApiResult<int>> GetNumberNotificationOfUser(string userId)
    {
        var listNotifications = await _notificationRepository.GetAllNotificationByUser(userId);
        if (!listNotifications.Any())
        {
            return new ApiResult<int>(true, 0);
        }

        return new ApiResult<int>(true, listNotifications.Count());
    }
}