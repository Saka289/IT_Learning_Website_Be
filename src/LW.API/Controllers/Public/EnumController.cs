using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LW.Shared.DTOs.Enum;
using LW.Shared.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using EnumExtensions = LW.Infrastructure.Extensions.EnumExtensions;

namespace LW.API.Controllers.Public
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumController : ControllerBase
    {
        [HttpGet("GetAllBookCollection")]
        public ActionResult<IEnumerable<EnumDto>> GetBookCollection()
        {
            var enumValues = Enum.GetValues(typeof(EBookCollection)).Cast<EBookCollection>();
            var result = enumValues.Select(e => new EnumDto
            {
                Value = (int)e,
                Name = EnumExtensions.GetDisplayName(e) ?? e.ToString()
            }).ToList();
            return Ok(result);
        }

        [HttpGet("GetAllBookType")]
        public ActionResult<IEnumerable<EnumDto>> GetAllBookType()
        {
            var enumValues = Enum.GetValues(typeof(EBookType)).Cast<EBookType>();
            var result = enumValues.Select(e => new EnumDto
            {
                Value = (int)e,
                Name = EnumExtensions.GetDisplayName(e) ?? e.ToString()
            }).ToList();
            return Ok(result);
        }
    }
}