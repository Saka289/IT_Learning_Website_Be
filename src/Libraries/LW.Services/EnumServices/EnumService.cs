using System.Collections;
using LW.Infrastructure.Extensions;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Enum;
using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Services.EnumServices;

public class EnumService : IEnumService
{
    public async Task<ApiResult<IEnumerable<EnumDto>>> GetAllBookCollection()
    {
        var enumValues = Enum.GetValues(typeof(EBookCollection)).Cast<EBookCollection>();
        var result = enumValues.Select(e => new EnumDto
        {
            Value = (int)e,
            Name = EnumHelperExtensions.GetDisplayName(e) ?? e.ToString()
        }).ToList();
        return new ApiResult<IEnumerable<EnumDto>>(true, result, "Get all book collection successfully");
    }

    public async Task<ApiResult<IEnumerable<EnumDto>>> GetAllBookType()
    {
        var enumValues = Enum.GetValues(typeof(EBookType)).Cast<EBookType>();
        var result = enumValues.Select(e => new EnumDto
        {
            Value = (int)e,
            Name = EnumHelperExtensions.GetDisplayName(e) ?? e.ToString()
        }).ToList();
        return new ApiResult<IEnumerable<EnumDto>>(true, result, "Get all book type successfully");
    }

    public async Task<ApiResult<IEnumerable<EnumDto>>> GetAllTypeQuestion()
    {
        var result =  typeof(ETypeQuestion).ToEnumDto();
        if (result is null)
        {
            return new ApiResult<IEnumerable<EnumDto>>(false, "Get all enum type not found !!!");
        }

        return new ApiSuccessResult<IEnumerable<EnumDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<EnumDto>>> GetAllLevelQuestion()
    {
        var result =  typeof(EQuestionLevel).ToEnumDto();
        if (result is null)
        {
            return new ApiResult<IEnumerable<EnumDto>>(false, "Get all question level not found !!!");
        }

        return new ApiSuccessResult<IEnumerable<EnumDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<EnumDto>>> GetAllTypeOfExam()
    {
        var result = typeof(EExamType).ToEnumDto();
        if (result is null)
        {
            return new ApiResult<IEnumerable<EnumDto>>(false, "Get all type of exam not found !!!");
        }
        return new ApiSuccessResult<IEnumerable<EnumDto>>(result);
    }
    
}