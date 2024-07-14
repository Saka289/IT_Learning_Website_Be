using Aspose.Pdf.Forms;
using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.TagRepositories;
using LW.Shared.DTOs.Tag;
using LW.Shared.SeedWork;

namespace LW.Services.TagServices;

public class TagService : ITagService
{
    private readonly IMapper _mapper;
    private readonly ITagRepository _tagRepository;
    private readonly IElasticSearchService<TagDto, int> _elasticSearchService;

    public TagService(IMapper mapper, ITagRepository tagRepository,
        IElasticSearchService<TagDto, int> elasticSearchService)
    {
        _mapper = mapper;
        _tagRepository = tagRepository;
        _elasticSearchService = elasticSearchService;
    }

    public async Task<ApiResult<IEnumerable<TagDto>>> GetAllTag()
    {
        var tags = await _tagRepository.GetAllTag();
        if (!tags.Any())
        {
            return new ApiResult<IEnumerable<TagDto>>(false, "Not found list");
        }

        var tagDtos = _mapper.Map<IEnumerable<TagDto>>(tags);
        return new ApiResult<IEnumerable<TagDto>>(true, tagDtos, "Get all tag sucessfully");
    }

    public async Task<ApiResult<PagedList<TagDto>>> GetAllTagPagination(PagingRequestParameters pagingRequestParameters)
    {
        var tags = await _tagRepository.GetAllTagPagination();
        if (!tags.Any())
        {
            return new ApiResult<PagedList<TagDto>>(false, "List Tags is null !!!");
        }

        var result = _mapper.ProjectTo<TagDto>(tags);
        var pagedResult = await PagedList<TagDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);

        return new ApiSuccessResult<PagedList<TagDto>>(pagedResult);
    }

    public async Task<ApiResult<TagDto>> GetTagById(int id)
    {
        var tag = await _tagRepository.GetTagById(id);
        if (tag == null)
        {
            return new ApiResult<TagDto>(false, "Not found");
        }

        var result = _mapper.Map<TagDto>(tag);
        return new ApiResult<TagDto>(true, result, "Get by Id successfully");
    }

    public async Task<ApiResult<bool>> UpdateTagStatus(int id)
    {
        var tag = await _tagRepository.GetTagById(id);
        if (tag == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }

        tag.IsActive = !tag.IsActive;
        await _tagRepository.UpdateTag(tag);
        return new ApiResult<bool>(true, "Update status of tag successfully");
    }

    public async Task<ApiResult<TagDto>> CreateTag(TagCreateDto tagCreateDto)
    {
        var tag = _mapper.Map<Tag>(tagCreateDto);
        await _tagRepository.CreateTag(tag);
        var result = _mapper.Map<TagDto>(tag);
        return new ApiResult<TagDto>(true, result, "Create Tag Successfully");
    }

    public async Task<ApiResult<TagDto>> UpdateTag(TagUpdateDto tagUpdateDto)
    {
        var check = await _tagRepository.GetTagById(tagUpdateDto.Id);
        if (check == null)
        {
            return new ApiResult<TagDto>(false, "Not found tag to update");
        }
        var tag = _mapper.Map(tagUpdateDto,check);
        await _tagRepository.UpdateTag(tag);
        var result = _mapper.Map<TagDto>(tag);
        return new ApiResult<TagDto>(true, result, "Update tag successfully");
    }

    public async Task<ApiResult<bool>> DeleteTag(int id)
    {
        var tag = await _tagRepository.GetTagById(id);
        if (tag == null)
        {
            return new ApiResult<bool>(false, "Not found");
        }

        await _tagRepository.DeleteTag(id);
        return new ApiResult<bool>(true, "Delete Tag Successully");
    }
}