using Aspose.Pdf.Forms;
using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.TagRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Services.ExamServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Tag;
using LW.Shared.SeedWork;
using MockQueryable.Moq;

namespace LW.Services.TagServices;

public class TagService : ITagService
{
    private readonly IMapper _mapper;
    private readonly ITagRepository _tagRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IExamRepository _examRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IProblemRepository _problemRepository;
    private readonly IElasticSearchService<TagDto, int> _elasticSearchService;

    public TagService(IMapper mapper, ITagRepository tagRepository,
        IElasticSearchService<TagDto, int> elasticSearchService, IDocumentRepository documentRepository,
        IExamRepository examRepository, ITopicRepository topicRepository, ILessonRepository lessonRepository,
        IQuizRepository quizRepository, IProblemRepository problemRepository)
    {
        _mapper = mapper;
        _tagRepository = tagRepository;
        _elasticSearchService = elasticSearchService;
        _documentRepository = documentRepository;
        _examRepository = examRepository;
        _topicRepository = topicRepository;
        _lessonRepository = lessonRepository;
        _quizRepository = quizRepository;
        _problemRepository = problemRepository;
    }

    public async Task<ApiResult<IEnumerable<TagDto>>> GetAllTag()
    {
        var tags = await _tagRepository.GetAllTag();
        if (!tags.Any())
        {
            return new ApiResult<IEnumerable<TagDto>>(false, "Not found list");
        }

        var tagDtos = _mapper.Map<IEnumerable<TagDto>>(tags);
        return new ApiResult<IEnumerable<TagDto>>(true, tagDtos, "Get all tag successfully");
    }

    public async Task<ApiResult<PagedList<TagDto>>> GetAllTagPagination(SearchTagDto searchTagDto)
    {
        IEnumerable<TagDto> tagList;
        if (!string.IsNullOrEmpty(searchTagDto.Value))
        {
            var tagListSearch = await _elasticSearchService.SearchDocumentFieldAsync(ElasticConstant.ElasticTags,
                new SearchRequestValue
                {
                    Value = searchTagDto.Value,
                    Size = searchTagDto.Size,
                });
            if (tagListSearch is null)
            {
                return new ApiResult<PagedList<TagDto>>(false, "Tag not found !!!");
            }

            tagList = tagListSearch.ToList();
        }
        else
        {
            var tagListAll = await _tagRepository.GetAllTag();
            if (!tagListAll.Any())
            {
                return new ApiResult<PagedList<TagDto>>(false, "List Tags is null !!!");
            }

            tagList = _mapper.Map<IEnumerable<TagDto>>(tagListAll);
        }

        var pagedResult = await PagedList<TagDto>.ToPageListAsync(tagList.AsQueryable().BuildMock(),
            searchTagDto.PageIndex, searchTagDto.PageSize, searchTagDto.OrderBy, searchTagDto.IsAscending);
        return new ApiSuccessResult<PagedList<TagDto>>(pagedResult);
    }

    public async Task<ApiResult<TagAllDto>> SearchTagPagination(SearchAllTagDto searchAllTagDto)
    {
        var examList = await _examRepository.SearchExamByTag(searchAllTagDto.TagValue.ConvertToTagString(), searchAllTagDto.OrderDate);
        var documentList = await _documentRepository.SearchDocumentByTag(searchAllTagDto.TagValue.ConvertToTagString(), searchAllTagDto.OrderDate);
        var topicList = await _topicRepository.SearchTopicByTag(searchAllTagDto.TagValue.ConvertToTagString(), searchAllTagDto.OrderDate);
        var lessonList = await _lessonRepository.SearchLessonByTag(searchAllTagDto.TagValue.ConvertToTagString(), searchAllTagDto.OrderDate);
        var quizList = await _quizRepository.SearchQuizByTag(searchAllTagDto.TagValue.ConvertToTagString(), searchAllTagDto.OrderDate);
        var problemList = await _problemRepository.SearchProblemByTag(searchAllTagDto.TagValue.ConvertToTagString(), searchAllTagDto.OrderDate);

        var exams = _mapper.Map<IEnumerable<TagExamDto>>(examList);
        var documents = _mapper.Map<IEnumerable<TagDocumentDto>>(documentList);
        var topics = _mapper.Map<IEnumerable<TagTopicDto>>(topicList);
        var lesson = _mapper.Map<IEnumerable<TagLessonDto>>(lessonList);
        var quizzes = _mapper.Map<IEnumerable<TagQuizDto>>(quizList);
        var problems = _mapper.Map<IEnumerable<TagProblemDto>>(problemList);

        var result = new TagAllDto()
        {
            Exams = exams,
            Documents = documents,
            Topics = topics,
            lessons = lesson,
            Quizzes = quizzes,
            Problems = problems,
        };

        return new ApiSuccessResult<TagAllDto>(result);
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
        var result = _mapper.Map<TagDto>(tag);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticTags, result, id);
        return new ApiResult<bool>(true, "Update status of tag successfully");
    }

    public async Task<ApiResult<TagDto>> CreateTag(TagCreateDto tagCreateDto)
    {
        var keyWord = tagCreateDto.Title.RemoveDiacritics();
        var tagExist = await _tagRepository.GetTagByKeyword(keyWord);
        if (tagExist != null)
        {
            return new ApiResult<TagDto>(false,
                "It seems that there is already a tag with the same value as the tag just created");
        }

        var tag = _mapper.Map<Tag>(tagCreateDto);
        tag.KeyWord = keyWord;
        await _tagRepository.CreateTag(tag);
        var result = _mapper.Map<TagDto>(tag);
        await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticTags, result, g => g.Id);
        return new ApiResult<TagDto>(true, result, "Create Tag Successfully");
    }

    public async Task<ApiResult<TagDto>> UpdateTag(TagUpdateDto tagUpdateDto)
    {
        var check = await _tagRepository.GetTagById(tagUpdateDto.Id);
        if (check == null)
        {
            return new ApiResult<TagDto>(false, "Not found tag to update");
        }
        var tagExist = await _tagRepository.GetTagByKeyword(tagUpdateDto.Title.RemoveDiacritics());
        if (tagExist != null)
        {
            return new ApiResult<TagDto>(false,
                "It seems that there is already a tag with the same value as the tag exist");
        }
        var tag = _mapper.Map(tagUpdateDto, check);
        tag.KeyWord = tagUpdateDto.Title.RemoveDiacritics();
        await _tagRepository.UpdateTag(tag);
        var result = _mapper.Map<TagDto>(tag);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticTags, result, tagUpdateDto.Id);
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
        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticTags, id);
        return new ApiResult<bool>(true, "Delete Tag successfully");
    }
}