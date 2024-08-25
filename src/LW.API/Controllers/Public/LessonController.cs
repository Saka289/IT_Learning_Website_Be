using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.LessonValidator;
using LW.Services.LessonServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Tag;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleConstant.RoleAdmin},{RoleConstant.RoleContentManager}")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet("GetAllLesson")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<IEnumerable<LessonDto>>>> GetAllLesson(bool? status)
        {
            var result = await _lessonService.GetAllLesson(status);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetAllLessonByTopic/{topicId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<IEnumerable<LessonDto>>>> GetAllLessonByTopic(int topicId, bool? status)
        {
            var result = await _lessonService.GetAllLessonByTopic(topicId, status);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetLessonIdByTag/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<IEnumerable<TagDto>>>> GetLessonIdByTag(int id)
        {
            var result = await _lessonService.GetLessonIdByTag(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetAllLessonPagination")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<PagedList<LessonDto>>>> GetAllLessonPagination([FromQuery] SearchLessonDto searchLessonDto)
        {
            var result = await _lessonService.GetAllLessonPagination(searchLessonDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetLessonById/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<LessonDto>>> GetLessonById([Required] int id)
        {
            var result = await _lessonService.GetLessonById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateLesson")]
        public async Task<ActionResult<ApiResult<LessonDto>>> CreateLesson([FromForm] LessonCreateDto lessonCreateDto)
        {
            var validationResult = await new CreateLessonCommandValidator().ValidateAsync(lessonCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _lessonService.CreateLesson(lessonCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateLesson")]
        public async Task<ActionResult<ApiResult<LessonDto>>> UpdateLesson([FromForm] LessonUpdateDto lessonUpdateDto)
        {
            var validationResult = await new UpdateLessonCommandValidator().ValidateAsync(lessonUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _lessonService.UpdateLesson(lessonUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = RoleConstant.RoleAdmin)]
        [HttpPut("UpdateStatusLesson")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusLesson([Required] int id)
        {
            var result = await _lessonService.UpdateLessonStatus(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteLesson/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteLesson([Required] int id)
        {
            var result = await _lessonService.DeleteLesson(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteRangeLesson")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteRangeLesson([Required] IEnumerable<int> ids)
        {
            var result = await _lessonService.DeleteRangeLesson(ids);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}