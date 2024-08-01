using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.ExecuteCodeValidator;
using LW.Services.ExecuteCodeServices;
using LW.Shared.DTOs.ExecuteCode;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecuteCodeController : ControllerBase
    {
        private readonly IExecuteCodeService _executeCodeService;

        public ExecuteCodeController(IExecuteCodeService executeCodeService)
        {
            _executeCodeService = executeCodeService;
        }
        
        [HttpGet("GetAllExecuteCode")]
        public async Task<ActionResult<ApiResult<IEnumerable<ExecuteCodeDto>>>> GetAllExecuteCode()
        {
            var result = await _executeCodeService.GetAllExecuteCode();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetAllExecuteCodeByProblemId/{problemId}")]
        public async Task<ActionResult<ApiResult<IEnumerable<ExecuteCodeDto>>>> GetAllExecuteCodeByProblemId([Required]int problemId, int? languages)
        {
            var result = await _executeCodeService.GetAllExecuteCodeByProblemId(problemId, languages);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetExecuteCodeById/{id}")]
        public async Task<ActionResult<ApiResult<ExecuteCodeDto>>> GetExecuteCodeById(int id)
        {
            var result = await _executeCodeService.GetExecuteCodeById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpPost("CreateExecuteCode")]
        public async Task<ActionResult<ApiResult<ExecuteCodeDto>>> CreateExecuteCode([FromBody] ExecuteCodeCreateDto executeCodeCreateDto)
        {
            var validationResult = await new CreateExecuteCodeCommandValidator().ValidateAsync(executeCodeCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _executeCodeService.CreateExecuteCode(executeCodeCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateExecuteCode")]
        public async Task<ActionResult<ApiResult<ExecuteCodeDto>>> UpdateExecuteCode([FromBody] ExecuteCodeUpdateDto executeCodeUpdateDto)
        {
            var validationResult = await new UpdateExecuteCodeCommandValidator().ValidateAsync(executeCodeUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _executeCodeService.UpdateExecuteCode(executeCodeUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPost("CreateRangeExecuteCode")]
        public async Task<ActionResult<ApiResult<bool>>> CreateRangeExecuteCode([FromBody] IEnumerable<ExecuteCodeCreateDto> executeCodeCreateDto)
        {
            var validator =  new CreateExecuteCodeCommandValidator();
            var validationResults =  executeCodeCreateDto.Select(async model => await validator.ValidateAsync(model));
            if (validationResults.Any(result => !result.IsCompleted))
            {
                return BadRequest(validationResults);
            }
            var result = await _executeCodeService.CreateRangeExecuteCode(executeCodeCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateRangeExecuteCode")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateRangeExecuteCode([FromBody] IEnumerable<ExecuteCodeUpdateDto> executeCodeUpdateDto)
        {
            var validator =  new UpdateExecuteCodeCommandValidator();
            var validationResults =  executeCodeUpdateDto.Select(async model => await validator.ValidateAsync(model));
            if (validationResults.Any(result => !result.IsCompleted))
            {
                return BadRequest(validationResults);
            }
            var result = await _executeCodeService.UpdateRangeExecuteCode(executeCodeUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("DeleteExecuteCode/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteExecuteCode([Required] int id)
        {
            var result = await _executeCodeService.DeleteExecuteCode(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("DeleteRangeExecuteCode")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteRangeExecuteCode(IEnumerable<int> ids)
        {
            var result = await _executeCodeService.DeleteRangeExecuteCode(ids);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
