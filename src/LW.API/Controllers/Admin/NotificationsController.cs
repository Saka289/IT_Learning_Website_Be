using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public async Task<ActionResult<ApiResult<NotificationDto>>> CreateNotification(
            [FromBody] NotificationCreateDto model)
        {
            var result = await _notificationService.CreateNotification(model);
            if (!result.IsSucceeded)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPost("CreateNotificationPersonal")]
        public async Task<ActionResult<ApiResult<NotificationDto>>> CreateNotificationPersonal(
            [FromBody] NotificationCreateDto notificationDto)
        {
            var result = await _notificationService.CreateNotification(notificationDto);
            if (!result.IsSucceeded)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPut("UpdateStatusOfNotification")]
        public async Task<ActionResult<ApiResult<NotificationDto>>> UpdateStatusOfNotification([Required] int id)
        {
            var result = await _notificationService.UpdateStatusNotification(id);
            if (!result.IsSucceeded)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPost("MarkAllAsReadAsync")]
        public async Task<ActionResult<ApiResult<bool>>> MarkAllAsReadAsync([Required] string userId)
        {
            var result = await _notificationService.MarkAllAsReadAsync(userId);
            if (!result.IsSucceeded)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet("GetAllNotificationByUser/{userId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<NotificationDto>>>> GetAllNotificationByUser(string userId)
        {
            var result = await _notificationService.GetAllNotificationByUser(userId);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("GetNumberOfNotificationByUser/{userId}")]
        public async Task<ActionResult<ApiResult<int>>> GetNumberOfNotificationByUser(string userId)
        {
            var result = await _notificationService.GetNumberNotificationOfUser(userId);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("DeleteAllNotificationByUser/{userId}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteAllNotificationByUser(string userId)
        {
            var result = await _notificationService.DeleteAllNotificationOfUser(userId);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}