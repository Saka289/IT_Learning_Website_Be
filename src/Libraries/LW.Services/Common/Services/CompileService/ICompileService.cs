using LW.Shared.DTOs.Compile;

namespace LW.Services.Common.Services.CompileService;

public interface ICompileService
{
    Task<CompileDto> SubmitCompile(CompileCreateDto compileCreateDto);
}