using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.ProblemValidator;
using LW.Services.ProblemServices;
using LW.Shared.DTOs.Problem;
using LW.Shared.DTOs.Tag;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        private readonly IProblemService _problemService;

        public ProblemController(IProblemService problemService)
        {
            _problemService = problemService;
        }

        [HttpGet("GetAllProblem")]
        public async Task<ActionResult<ApiResult<IEnumerable<ProblemDto>>>> GetAllProblem(bool? status)
        {
            var result = await _problemService.GetAllProblem(status);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetAllProblemPagination")]
        public async Task<ActionResult<ApiResult<PagedList<ProblemDto>>>> GetAllProblemPagination([FromQuery] SearchProblemDto searchProblemDto)
        {
            var result = await _problemService.GetAllProblemPagination(searchProblemDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }
        
        [HttpGet("GetProblemIdByTag/{id}")]
        public async Task<ActionResult<ApiResult<IEnumerable<TagDto>>>> GetProblemIdByTag(int id)
        {
            var result = await _problemService.GetProblemIdByTag(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("GetProblemById/{id}")]
        public async Task<ActionResult<ApiResult<ProblemDto>>> GetProblemById([Required] int id)
        {
            var result = await _problemService.GetProblemById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpPost("CreateProblem")]
        public async Task<ActionResult<ApiResult<ProblemDto>>> CreateProblem([FromBody] ProblemCreateDto problemCreateDto)
        {
            var validationResult = await new CreateProblemCommandValidator().ValidateAsync(problemCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _problemService.CreateProblem(problemCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateProblem")]
        public async Task<ActionResult<ApiResult<ProblemDto>>> UpdateProblem([FromBody] ProblemUpdateDto problemUpdateDto)
        {
            var validationResult = await new UpdateProblemCommandValidator().ValidateAsync(problemUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _problemService.UpdateProblem(problemUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateStatusProblem/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusProblem([Required] int id)
        {
            var result = await _problemService.UpdateStatusProblem(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("DeleteProblem/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteProblem([Required] int id)
        {
            var result = await _problemService.DeleteProblem(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
