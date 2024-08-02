using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.Common;
using LW.Services.NotificationServices;
using LW.Shared.DTOs.Notification;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpPost("CreateNotification")]
        public async Task<ActionResult<ApiResult<NotificationDto>>> CreateNotification(NotificationCreateDto model)
        {
            var result = await _notificationService.CreateNotification(model);
            if(!result.IsSucceeded)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpPost("CreateNotificationPersonal")]

        public async Task<ActionResult<ApiResult<NotificationDto>>> CreateNotificationPersonal(NotificationCreateDto notificationDto)
        {
          
            var result = await _notificationService.CreateNotification(notificationDto);
            if(!result.IsSucceeded)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpGet("GetAllNotificationByUser")]
        public async Task<ActionResult<ApiResult<IEnumerable<NotificationDto>>>> GetAllNotificationByUser(string userId)
        {
            var result = await _notificationService.GetAllNotificationByUser(userId);
            if(!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
        
        [HttpDelete("DeleteAllNotificationByUser")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteAllNotificationByUser(string userId)
        {
            var result = await _notificationService.DeleteAllNotificationOfUser(userId);
            if(!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
