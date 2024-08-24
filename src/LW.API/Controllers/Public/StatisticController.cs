using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.StatisticServices;
using LW.Shared.DTOs.Statistic;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpGet("CountUser")]
        public async Task<ActionResult<ApiResult<int>>> CountUser()
        {
            var result = await _statisticService.CountUser();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("CountRole")]
        public async Task<ActionResult<ApiResult<int>>> CountRole()
        {
            var result = await _statisticService.CountRole();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("DocumentStatistic/{year}")]
        public async Task<ActionResult<ApiResult<IEnumerable<StatisticDto>>>> DocumentStatistic([Required] int year)
        {
            var result = await _statisticService.DocumentStatistic(year);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("TopicStatistic/{year}")]
        public async Task<ActionResult<ApiResult<IEnumerable<StatisticDto>>>> TopicStatistic([Required] int year)
        {
            var result = await _statisticService.TopicStatistic(year);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("LessonStatistic/{year}")]
        public async Task<ActionResult<ApiResult<IEnumerable<StatisticDto>>>> LessonStatistic([Required] int year)
        {
            var result = await _statisticService.LessonStatistic(year);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}