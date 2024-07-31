using LW.Shared.DTOs.Request;

namespace LW.Contracts.Common;

public interface IBaseService<T>  where T : class
{
    Task<T?> SendAsync<T>(RequestDto requestDto);
}