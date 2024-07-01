using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Enum;
using LW.Shared.SeedWork;

namespace LW.Services.EnumServices;

public interface IEnumService
{
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllBookCollection();
    Task<ApiResult<IEnumerable<EnumDto>>> GetAllBookType();

}