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
        var result = typeof(EBookCollection).ToEnumDto();
        if (result is null)
        {
            return new ApiResult<IEnumerable<EnumDto>>(false, "Get all book collection not found !!!");
        }

        return new ApiSuccessResult<IEnumerable<EnumDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<EnumDto>>> GetAllBookType()
    {
        var result = typeof(EBookType).ToEnumDto();
        if (result is null)
        {
            return new ApiResult<IEnumerable<EnumDto>>(false, "Get all book type not found !!!");
        }

        return new ApiSuccessResult<IEnumerable<EnumDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<EnumDto>>> GetAllTypeQuestion()
    {
        var result = typeof(ETypeQuestion).ToEnumDto();
        if (result is null)
        {
            return new ApiResult<IEnumerable<EnumDto>>(false, "Get all enum type not found !!!");
        }

        return new ApiSuccessResult<IEnumerable<EnumDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<EnumDto>>> GetAllLevelQuestion()
    {
        var result = typeof(EQuestionLevel).ToEnumDto();
        if (result is null)
        {
            return new ApiResult<IEnumerable<EnumDto>>(false, "Get all question level not found !!!");
        }

        return new ApiSuccessResult<IEnumerable<EnumDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<EnumDto>>> GetAllQuizType()
    {
        var result = typeof(ETypeQuiz).ToEnumDto();
        if (result is null)
        {
            return new ApiResult<IEnumerable<EnumDto>>(false, "Get all quiz type not found !!!");
        }

        return new ApiSuccessResult<IEnumerable<EnumDto>>(result);
    }
}