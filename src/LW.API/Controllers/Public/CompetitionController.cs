using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.CompetitionValidator;
using LW.Services.CompetitionServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.Competition;
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
    public class CompetitionController : ControllerBase
    {
        private readonly ICompetitionService _competitionService;
        public CompetitionController(ICompetitionService competitionService)
        {
            _competitionService = competitionService;
        }
        
        [HttpGet("GetAllCompetition")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<IEnumerable<CompetitionDto>>>> GetAllCompetition(bool? status)
        {
            var result = await _competitionService.GetAllCompetition(status);
            if (!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("GetAllCompetitionPagination")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<PagedList<CompetitionDto>>>> GetAllCompetitionPagination([FromQuery]SearchCompetitionDto searchCompetitionDto)
        {
            var result = await _competitionService.GetAllCompetitionPagination(searchCompetitionDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetCompetitionById")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<CompetitionDto>>> GetAllCompetitionById(int id)
        {
            var result = await _competitionService.GetCompetitionById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("CreateCompetition")]
        public async Task<ActionResult<ApiResult<bool>>> CreatCompetition([FromBody] CompetitionCreateDto model)
        {
            var validationResult = await new CreateCompetitionCommandValidator().ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            
            var result = await _competitionService.CreateCompetition(model);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("UpdateCompetition")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateCompetition([FromBody] CompetitionUpdateDto model)
        {
            var validationResult = await new UpdateCompetitionCommandValidator().ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _competitionService.UpdateCompetition(model);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("UpdateStatusCompetition/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusCompetition(int id)
        {
            var result = await _competitionService.UpdateStatusCompetition(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("DeleteCompetition/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteCompetition(int id)
        {
            var result = await _competitionService.DeleteCompetition(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
