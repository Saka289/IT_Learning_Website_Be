using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.SolutionValidator;
using LW.Services.SolutionServices;
using LW.Shared.Constant;
using LW.Shared.SeedWork;
using LW.Shared.DTOs.Solution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SolutionController : ControllerBase
    {
        private readonly ISolutionService _solutionService;

        public SolutionController(ISolutionService solutionService)
        {
            _solutionService = solutionService;
        }

        [HttpGet("GetAllSolutionByProblemIdPagination")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<IEnumerable<SolutionDto>>>> GetAllSolutionByProblemIdPagination([FromQuery] SearchSolutionDto searchSolutionDto)
        {
            var result = await _solutionService.GetAllSolutionByProblemIdPagination(searchSolutionDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetSolutionById/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<SolutionDto>>> GetSolutionById(int id)
        {
            var result = await _solutionService.GetSolutionById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CreateSolution")]
        public async Task<ActionResult<ApiResult<SolutionDto>>> CreateSolution([FromBody] SolutionCreateDto solutionCreateDto)
        {
            var validationResult = await new CreateSolutionCommandValidator().ValidateAsync(solutionCreateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _solutionService.CreateSolution(solutionCreateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateSolution")]
        public async Task<ActionResult<ApiResult<SolutionDto>>> UpdateSolution([FromBody] SolutionUpdateDto solutionUpdateDto)
        {
            var validationResult = await new UpdateSolutionCommandValidator().ValidateAsync(solutionUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            var result = await _solutionService.UpdateSolution(solutionUpdateDto);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpPut("UpdateStatusSolution/{id}")]
        [Authorize(Roles = $"{RoleConstant.RoleAdmin},{RoleConstant.RoleContentManager}")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusSolution([Required] int id)
        {
            var result = await _solutionService.UpdateStatusSolution(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        [HttpDelete("DeleteSolution/{id}")]
        [Authorize(Roles = $"{RoleConstant.RoleAdmin},{RoleConstant.RoleContentManager}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteSolution([Required] int id)
        {
            var result = await _solutionService.DeleteSolution(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
