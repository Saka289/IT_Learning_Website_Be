using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public async Task<ActionResult<ApiResult<IEnumerable<LevelDto>>>> GetAllLevel()
        {
            var result = await _levelService.GetAllLevel();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        [HttpGet("GetLevelById")]
        public async Task<ActionResult<ApiResult<LevelDto>>> GetLevelById([Required] int id)
        {
            var result = await _levelService.GetLevelById(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
