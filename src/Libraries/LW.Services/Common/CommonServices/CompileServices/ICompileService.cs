using LW.Shared.DTOs.Compile;

namespace LW.Services.Common.CommonServices.CompileServices;

public interface ICompileService
{
    Task<CompileDto> SubmitCompile(CompileCreateDto compileCreateDto);
}