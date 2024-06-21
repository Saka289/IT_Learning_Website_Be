using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.LessonService;
using LW.Shared.DTOs.Lesson;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        
        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet("GetAllLesson")]
        public async Task<ActionResult<ApiResult<IEnumerable<LessonDto>>>> GetAllLesson()
        {
            var result = await _lessonService.GetAllLesson();
            return Ok(result);
        }

        [HttpGet("GetLessonById/{id}")]
        public async Task<ActionResult<ApiResult<LessonDto>>> GetLessonById([Required] int id)
        {
            var result = await _lessonService.GetLessonById(id);
            return Ok(result);
        }
        
        [HttpGet("SearchByLesson")]
        public async Task<ActionResult<ApiResult<LessonDto>>> SearchByLesson([FromQuery] SearchLessonDto searchLessonDto)
        {
            var result = await _lessonService.SearchByLesson(searchLessonDto);
            return Ok(result);
        }

        [HttpPost("CreateLesson")]
        public async Task<ActionResult<ApiResult<LessonDto>>> CreateLesson([FromForm] LessonCreateDto LessonCreateDto)
        {
            var result = await _lessonService.CreateLesson(LessonCreateDto);
            return Ok(result);
        }

        [HttpPut("UpdateLesson")]
        public async Task<ActionResult<ApiResult<LessonDto>>> UpdateLesson([FromForm] LessonUpdateDto LessonUpdateDto)
        {
            var result = await _lessonService.UpdateLesson(LessonUpdateDto);
            return Ok(result);
        }

        [HttpPut("UpdateStatusLesson")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusLesson(int id)
        {
            var result = await _lessonService.UpdateLessonStatus(id);
            return Ok(result);
        }

        [HttpDelete("DeleteLesson/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteLesson([Required] int id)
        {
            var result = await _lessonService.DeleteLesson(id);
            return Ok(result);
        }
    }
}
