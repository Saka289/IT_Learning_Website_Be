using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LW.Services.UserService;
using LW.Shared.DTOs.Facebook;
using LW.Shared.DTOs.Google;
using LW.Shared.DTOs.User;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
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
        public async Task<ActionResult<ApiResult<RegisterResponseUserDto>>> RegisterUser([FromBody] RegisterUserDto registerUserDto)
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
        public async Task<ActionResult<ApiResult<LoginResponseUserDto>>> Login([FromBody] LoginUserDto loginUserDto)
        {
            var result = await _userService.Login(loginUserDto);
            return Ok(result);
        }

        [HttpPost("GoogleLogin")]
        public async Task<ActionResult<ApiResult<LoginResponseUserDto>>> GoogleLogin([FromBody] GoogleSignInDto googleSignInDto)
        {
            var result = await _userService.LoginGoogle(googleSignInDto);
            return Ok(result);
        }

        [HttpPost("FacebookLogin")]
        public async Task<ActionResult<ApiResult<LoginResponseUserDto>>> FacebookLogin([FromBody] FacebookSignInDto facebookSignInDto)
        {
            var result = await _userService.LoginFacebook(facebookSignInDto);
            return Ok(result);
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ApiResult<bool>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _userService.ChangePassword(changePasswordDto);
            return Ok(result);
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ApiResult<bool>>> ForgotPassword([FromBody] string email)
        {
            var result = await _userService.ForgotPassword(email);
            return Ok(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ApiResult<bool>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _userService.ResetPassword(resetPasswordDto);
            return Ok(result);
        }
    }
}