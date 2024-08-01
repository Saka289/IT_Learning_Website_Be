using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.Common;
using LW.Services.Common.CommonServices.NotificationServices;
using LW.Shared.DTOs.Notification;
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
        public async Task<IActionResult> CreateNotification(NotificationCreateDto model)
        {
            var result = await _notificationService.CreateNotification(model);
            if(result == null)
            {
                return BadRequest();
            }
            return Ok("Tạo thành công");
        }
        [HttpPost("CreateNotificationPersonal")]

        public async Task<IActionResult> CreateNotificationPersonal(NotificationCreateDto notificationDto)
        {
          
            var result = await _notificationService.CreateNotification(notificationDto);
            if(result == null)
            {
                return BadRequest();
            }
            return Ok("Tạo thành công");
        }
    }
}
