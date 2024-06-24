using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.API.Application.Validators.LevelValidator;
using LW.Services.LevelServices;
using LW.Shared.DTOs.Level;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelController : ControllerBase
    {
        private readonly ILevelService _levelService;

        public LevelController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        [HttpGet("GetAllLevel")]
        public async Task<ActionResult<ApiResult<IEnumerable<LevelDto>>>> GetAllLevel()
        {
            var result = await _levelService.GetAll();
            if (!result.IsSucceeded)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("GetAllLevelPagination")]
        public async Task<ActionResult<ApiResult<PagedList<LevelDto>>>> GetAllLevelPagination([FromQuery]PagingRequestParameters pagingRequestParameters)
        {
            var result = await _levelService.GetAllLevelPagination(pagingRequestParameters);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }

        [HttpGet("GetLevelById")]
        public async Task<ActionResult<ApiResult<LevelDto>>> GetAllLevelById(int id)
        {
            var result = await _levelService.GetById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost("CreateLevel")]
        public async Task<ActionResult<ApiResult<bool>>> CreatLevel([FromBody] LevelDtoForCreate model)
        {
            var validationResult = await new CreateLevelCommandValidator().ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _levelService.Create(model);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("UpdateLevel")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateLevel([FromBody] LevelDtoForUpdate model)
        {
            var validationResult = await new UpdateLevelCommandValidator().ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }
            var result = await _levelService.Update(model);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("UpdateStatusLevel/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusLevel(int id)
        {
            var result = await _levelService.UpdateStatus(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("DeleteLevel/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteLevel(int id)
        {
            var result = await _levelService.Delete(id);
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("SearchLevel")]
        public async Task<ActionResult<ApiResult<IEnumerable<LevelDto>>>> SearchLevel(
            [FromQuery] SearchLevelDto searchLevelDto)
        {
            var result = await _levelService.SearchByLevelPagination(searchLevelDto);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(result.Data.GetMetaData()));
            return Ok(result);
        }
    }
}