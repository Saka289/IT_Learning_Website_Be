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
using MockQueryable.Moq;

namespace LW.Services.NotificationServices;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly IRedisCache<HubConnection> _redis;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationService(INotificationRepository notificationRepository, IMapper mapper,
        IHubContext<NotificationHub> notificationHub, IRedisCache<HubConnection> redis,
        UserManager<ApplicationUser> userManager)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _notificationHub = notificationHub;
        _redis = redis;
        _userManager = userManager;
    }

    public async Task<ApiResult<PagedList<NotificationDto>>> GetAllNotificationByUser(string userId,
        PagingRequestParameters pagingRequestParameters)
    {
        var userExist = await _userManager.FindByIdAsync(userId);
        if (userExist == null)
        {
            return new ApiResult<PagedList<NotificationDto>>(false, "User not found");
        }

        var listNotifications = await _notificationRepository.GetAllNotificationByUser(userId);
        if (!listNotifications.Any())
        {
            return new ApiResult<PagedList<NotificationDto>>(false);
        }

        var result = _mapper.Map<IEnumerable<NotificationDto>>(listNotifications);
        //image
        foreach (var notificationDto in result)
        {
            var userSend = await _userManager.FindByIdAsync(notificationDto.UserSendId);
            if (userSend != null)
            {
                notificationDto.UserSendImage = userSend.Image;
            }
        }

        var pagedResult = await PagedList<NotificationDto>.ToPageListAsync(result.AsQueryable().BuildMock(),
            pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<NotificationDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<NotificationDto>>> GetAllNotificationNotReadByUser(string userId,
        PagingRequestParameters pagingRequestParameters)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult<PagedList<NotificationDto>>(false, "Not found user send notification");
        }

        var listNotifications = await _notificationRepository.GetAllNotificationNotReadByUser(userId);
        if (!listNotifications.Any())
        {
            var emptyResult = new PagedList<NotificationDto>(new List<NotificationDto>(), 0, pagingRequestParameters.PageIndex, pagingRequestParameters.PageSize);
            return new ApiResult<PagedList<NotificationDto>>(true, emptyResult, "List null");
        }

        var result = _mapper.Map<IEnumerable<NotificationDto>>(listNotifications);
        //image
        foreach (var notificationDto in result)
        {
            var userSend = await _userManager.FindByIdAsync(notificationDto.UserSendId);
            if (userSend != null)
            {
                notificationDto.UserSendImage = userSend.Image;
            }
        }

        var pagedResult = await PagedList<NotificationDto>.ToPageListAsync(result.AsQueryable().BuildMock(),
            pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<NotificationDto>>(pagedResult);
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
            _notificationHub.Clients.All.SendAsync("ReceivedNotification", notificationCreateDto.Description);
        }
        else if (notificationCreateDto.NotificationType == ENotificationType.Personal)
        {
            var hubConnections = await _redis.GetAllKeysByPattern(RedisPatternConstant.PatternHub, "UserId",
                notificationCreateDto.UserReceiveId);
            if (hubConnections.Any())
            {
                foreach (var hubConnection in hubConnections)
                {
                    await _notificationHub.Clients.Client(hubConnection.ConnectionId)
                        .SendAsync("ReceivedPersonalNotification", notificationCreateDto.Description,
                            notificationCreateDto.UserReceiveId, notificationCreateDto.UserSendId);
                }
            }
        }

        return new ApiResult<NotificationDto>(true, result, "Create notification successfully");
    }

    public async Task<ApiResult<NotificationDto>> UpdateStatusNotification(int id)
    {
        var notification = await _notificationRepository.GetNotificationById(id);
        if (notification == null)
        {
            return new ApiResult<NotificationDto>(false, "Not found");
        }

        var result = await _notificationRepository.UpdateStatusOfNotification(id);
        if (result == false)
        {
            return new ApiResult<NotificationDto>(false, "Update status fail");
        }

        return new ApiResult<NotificationDto>(true, "Update status success");
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

    public async Task<ApiResult<bool>> MarkAllAsReadAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult<bool>(false, "Not found user");
        }

        await _notificationRepository.MarkAllAsRead(userId);
        return new ApiResult<bool>(true, "Mark all as read success");
    }

    public async Task<ApiResult<int>> GetNumberNotificationOfUser(string userId)
    {
        var listNotifications = await _notificationRepository.GetAllNotificationByUser(userId);
        var listNotificationNotRead = listNotifications.Where(x => x.IsRead == false);
        if (!listNotificationNotRead.Any())
        {
            return new ApiResult<int>(true, 0);
        }
        return new ApiResult<int>(true, listNotificationNotRead.Count());
    }
}