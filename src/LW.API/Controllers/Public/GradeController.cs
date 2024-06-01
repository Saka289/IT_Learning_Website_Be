using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.GradeService;
using LW.Shared.DTOs.Grade;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<ApiResult<IEnumerable<GradeDto>>>> GetAllGrade()
        {
            var result = await _gradeService.GetAllGrade();
            return Ok(result);
        }

        [HttpGet("GetGradeById/{id}")]
        public async Task<ActionResult<ApiResult<GradeDto>>> GetGradeById([Required] int id)
        {
            var result = await _gradeService.GetGradeById(id);
            return Ok(result);
        }

        [HttpPost("CreateGrade")]
        public async Task<ActionResult<ApiResult<GradeDto>>> CreateGrade([FromBody] GradeCreateDto gradeCreateDto)
        {
            var result = await _gradeService.CreateGrade(gradeCreateDto);
            return Ok(result);
        }
        
        [HttpPut("UpdateGrade")]
        public async Task<ActionResult<ApiResult<GradeDto>>> UpdateGrade([FromBody] GradeUpdateDto gradeUpdateDto)
        {
            var result = await _gradeService.UpdateGrade(gradeUpdateDto);
            return Ok(result);
        }
        
        [HttpDelete("DeleteGrade/{id}")]
        public async Task<ActionResult<ApiResult<GradeDto>>> DeleteGrade([Required] int id)
        {
            var result = await _gradeService.DeleteGrade(id);
            return Ok(result);
        }
    }
}