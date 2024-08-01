using LW.Contracts.Common;
using LW.Shared.Configurations;
using LW.Shared.DTOs.Compile;
using LW.Shared.DTOs.Request;
using LW.Shared.Enums;
using Microsoft.Extensions.Options;

namespace LW.Services.Common.Services.CompileService;

public class CompileService : ICompileService
{
    private readonly IBaseService<CompileCreateDto> _baseService;
    private readonly UrlBase _urlBase;

    public CompileService(IBaseService<CompileCreateDto> baseService, IOptions<UrlBase> urlBase)
    {
        _baseService = baseService;
        _urlBase = urlBase.Value;
    }
    public async Task<CompileDto> SubmitCompile(CompileCreateDto compileCreateDto)
    {
        var response = await _baseService.SendAsync<CompileDto>(new RequestDto()
        {
            ApiType = EApiType.Post,
            Data = compileCreateDto,
            Url = _urlBase.Judge0Url + _urlBase.Judge0RequestUrl
        });

        if (response is null)
        {
            return null;
        }

        return response;
    }
}