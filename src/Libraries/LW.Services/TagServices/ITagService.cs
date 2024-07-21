using LW.Shared.DTOs.Tag;
using LW.Shared.SeedWork;

namespace LW.Services.TagServices;

public interface ITagService
{
    Task<ApiResult<IEnumerable<TagDto>>> GetAllTag();
    Task<ApiResult<PagedList<TagDto>>> GetAllTagPagination(PagingRequestParameters pagingRequestParameters);
    //Task<ApiResult<PagedList<TagDto>>> SearchTagPagination(SearchTagDto searchTagDto);
    Task<ApiResult<TagDto>> GetTagById(int id);
    Task<ApiResult<bool>> UpdateTagStatus(int id);
    Task<ApiResult<TagDto>> CreateTag(TagCreateDto tagCreateDto);
    Task<ApiResult<TagDto>> UpdateTag(TagUpdateDto tagUpdateDto);
    Task<ApiResult<bool>> DeleteTag(int id);
}