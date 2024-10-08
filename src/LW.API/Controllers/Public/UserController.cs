using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LW.API.Application.Validators.UserValidator;
using LW.Services.UserServices;
using LW.Shared.DTOs.Facebook;
using LW.Shared.DTOs.Google;
using LW.Shared.DTOs.Token;
using LW.Shared.DTOs.User;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("RegisterUser")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<RegisterResponseUserDto>>> RegisterUser(
            [FromBody] RegisterUserDto registerUserDto)
        {
            var validationResult = await new CreateUserCommandValidator().ValidateAsync(registerUserDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _userService.Register(registerUserDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("SendVerifyEmail")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<bool>>> SendVerifyEmail([Required] string email)
        {
            var result = await _userService.SendVerifyEmail(email);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("VerifyEmail")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<bool>>> VerifyEmail([Required] string email, [Required] string token)
        {
            var result = await _userService.VerifyEmail(email, token);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<LoginResponseUserDto>>> Login([FromBody] LoginUserDto loginUserDto)
        {
            var result = await _userService.Login(loginUserDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("GoogleLogin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<LoginResponseUserDto>>> GoogleLogin(
            [FromBody] GoogleSignInDto googleSignInDto)
        {
            var result = await _userService.LoginGoogle(googleSignInDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("FacebookLogin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<LoginResponseUserDto>>> FacebookLogin(
            [FromBody] FacebookSignInDto facebookSignInDto)
        {
            var result = await _userService.LoginFacebook(facebookSignInDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("ChangePassword")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<bool>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _userService.ChangePassword(changePasswordDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<bool>>> ForgotPassword([FromBody] string email)
        {
            var result = await _userService.ForgotPassword(email);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<bool>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _userService.ResetPassword(resetPasswordDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<TokenResponseDto>>> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
        {
            var result = await _userService.RefreshToken(tokenRequestDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("Revoke")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<TokenResponseDto>>> RevokeToken([FromBody] string emailOrUserName)
        {
            var result = await _userService.Revoke(emailOrUserName);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<ApiResult<UpdateResponseUserDto>>> UpdateUser(
            [FromForm] UpdateUserDto updateUserDto)
        {
            var validationResult = await new UpdateUserCommandValidator().ValidateAsync(updateUserDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _userService.UpdateUser(updateUserDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("GetUser/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<UserResponseDto>>> GetUser([Required] string userId)
        {
            var result = await _userService.GetUserByUserId(userId);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}