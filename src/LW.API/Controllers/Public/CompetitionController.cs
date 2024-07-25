using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.CompetitionValidator;
using LW.API.Application.Validators.LevelValidator;
using LW.Services.CompetitionServices;
using LW.Shared.DTOs.Competition;
using LW.Shared.DTOs.Level;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitionController : ControllerBase
    {
        private readonly ICompetitionService _competitionService;
        public CompetitionController(ICompetitionService competitionService)
        {
            _competitionService = competitionService;
        }
         [HttpGet("GetAllCompetition")]
        public async Task<ActionResult<ApiResult<IEnumerable<CompetitionDto>>>> GetAllCompetition()
        {
            var result = await _competitionService.GetAllCompetition();
            if (!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("GetAllCompetitionPagination")]
        public async Task<ActionResult<ApiResult<PagedList<CompetitionDto>>>> GetAllCompetitionPagination([FromQuery]PagingRequestParameters pagingRequestParameters)
        {
            var result = await _competitionService.GetAllCompetitionPagination(pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetCompetitionById")]
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

        [HttpGet("SearchCompetitionPagination")]
        public async Task<ActionResult<ApiResult<IEnumerable<CompetitionDto>>>> SearchCompetitionPagination(
            [FromQuery] SearchCompetitionDto searchCompetitionDto)
        {
            var result = await _competitionService.SearchByCompetitionPagination(searchCompetitionDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }
    }
}
