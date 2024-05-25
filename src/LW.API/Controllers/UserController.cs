using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.UserService;
using LW.Shared.DTOs.User;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult<ApiResult<RegisterResponseUserDto>>> RegisterUser(
            [FromBody] RegisterUserDto registerUserDto)
        {
            var result = await _userService.Register(registerUserDto);
            return Ok(result);
        }

        [HttpGet("SendVerifyEmail")]
        public async Task<ActionResult<ApiResult<bool>>> SendVerifyEmail([Required] string email)
        {
            var result = await _userService.SendVerifyEmail(email);
            return Ok(result);
        }

        [HttpGet("VerifyEmail")]
        public async Task<ActionResult<ApiResult<bool>>> VerifyEmail([Required] string token)
        {
            var result = await _userService.VerifyEmail(token);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ApiResult<bool>>> Login([FromBody] LoginUserDto loginUserDto)
        {
            var result = await _userService.Login(loginUserDto);
            return Ok(result);
        }
    }
}