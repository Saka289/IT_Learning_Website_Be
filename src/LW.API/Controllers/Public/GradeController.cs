using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.GradeValidator;
using LW.Services.GradeServices;
using LW.Shared.DTOs.Grade;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        private readonly IGradeService _gradeService;

        public GradeController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet("GetAllGrade")]
        public async Task<ActionResult<ApiResult<IEnumerable<GradeDto>>>> GetAllGrade([Required] bool isInclude)
        {
            var result = await _gradeService.GetAllGrade(isInclude);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        [HttpGet("GetAllGradeByLevelId")]
        public async Task<ActionResult<ApiResult<IEnumerable<GradeDto>>>> GetAllGradeByLevelId([Required] int levelId)
        {
            var result = await _gradeService.GetListGradeByLevelId(levelId);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("GetAllGradePagination")]
        public async Task<ActionResult<ApiResult<PagedList<GradeDto>>>> GetAllGradePagination([FromQuery] SearchGradeDto searchGradeDto)
        {
            var result = await _gradeService.GetAllGradePagination(searchGradeDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetGradeById/{id}")]
        public async Task<ActionResult<ApiResult<GradeDto>>> GetGradeById([Required] int id, bool isInclude)
        {
            var result = await _gradeService.GetGradeById(id, isInclude);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateGrade")]
        public async Task<ActionResult<ApiResult<GradeDto>>> CreateGrade([FromBody] GradeCreateDto gradeCreateDto)
        {
            var validationResult = await new CreateGradeCommandValidator().ValidateAsync(gradeCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _gradeService.CreateGrade(gradeCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateGrade")]
        public async Task<ActionResult<ApiResult<GradeDto>>> UpdateGrade([FromBody] GradeUpdateDto gradeUpdateDto)
        {
            var validationResult = await new UpdateGradeCommandValidator().ValidateAsync(gradeUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _gradeService.UpdateGrade(gradeUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("UpdateStatusGrade")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusGrade(int id)
        {
            var result = await _gradeService.UpdateGradeStatus(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("DeleteGrade/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteGrade([Required] int id)
        {
            var result = await _gradeService.DeleteGrade(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}