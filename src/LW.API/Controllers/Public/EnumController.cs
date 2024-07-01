using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.Services.EnumServices;
using LW.Shared.DTOs.Enum;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumController : ControllerBase
    {
        private readonly IEnumService _enumService;
        public EnumController(IEnumService enumService)
        {
            _enumService = enumService;
        }
        [HttpGet("GetAllBookCollection")]
        public async Task<ActionResult<ApiResult<IEnumerable<EnumDto>>>> GetBookCollection()
        {
            var result = await _enumService.GetAllBookCollection();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("GetAllBookType")]
        public async Task<ActionResult<ApiResult<IEnumerable<EnumDto>>>> GetBookType()
        {
            var result = await _enumService.GetAllBookType();
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);        }
    }
}