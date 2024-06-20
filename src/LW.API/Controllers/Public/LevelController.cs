using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.LevelServices;
using LW.Shared.DTOs.Level;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<IEnumerable<LevelDto>>> GetAllLevel()
        {
            var result = await _levelService.GetAll();
            return Ok(result);
        }
        [HttpGet("GetLevelById")]
        public async Task<ActionResult<ApiResult<LevelDto>>> GetAllLevelById(int id)
        {
            var result = await _levelService.GetById(id);
            return Ok(result);
        }
        [HttpPost("CreateLevel")]
        public async Task<ActionResult<ApiResult<bool>>> CreatLevel([FromBody] LevelDtoForCreate model)
        {
            var result = await _levelService.Create(model);
            return Ok(result);
        }
        [HttpPut("UpdateLevel")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateLevel([FromBody] LevelDtoForUpdate model)
        {
            var result = await _levelService.Update(model);
            return Ok(result);
        }
        [HttpPut("UpdateStatusLevel/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateStatusLevel(int id)
        {
            var result = await _levelService.UpdateStatus(id);
            return Ok(result);
        }
        [HttpDelete("DeleteLevel/{id}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteLevel(int id)
        {
            var result = await _levelService.Delete(id);
            return Ok(result);
        }
        [HttpGet("SearchLevel")]
        public async Task<ActionResult<IEnumerable<LevelDto>>> SearchLevel([FromQuery]SearchLevelDto searchLevelDto)
        {
            var result = await _levelService.SearchLevel(searchLevelDto);
            return Ok(result);
        }
    }
}
