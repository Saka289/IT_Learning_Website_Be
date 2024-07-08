using System.Collections;
using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.QuizAnswerRepositories;
using LW.Data.Repositories.QuizQuestionRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.QuizAnswer;
using LW.Shared.DTOs.QuizQuestion;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using MockQueryable.Moq;

namespace LW.Services.QuizQuestionServices;

public class QuizQuestionService : IQuizQuestionService
{
    private readonly IQuizAnswerRepository _quizAnswerRepository;
    private readonly IQuizQuestionRepository _quizQuestionRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IElasticSearchService<QuizQuestionDto, int> _elasticSearchService;
    private readonly IMapper _mapper;

    public QuizQuestionService(IQuizAnswerRepository quizAnswerRepository,
        IQuizQuestionRepository quizQuestionRepository, IMapper mapper, IQuizRepository quizRepository,
        IElasticSearchService<QuizQuestionDto, int> elasticSearchService)
    {
        _quizAnswerRepository = quizAnswerRepository;
        _quizQuestionRepository = quizQuestionRepository;
        _mapper = mapper;
        _quizRepository = quizRepository;
        _elasticSearchService = elasticSearchService;
    }

    public async Task<ApiResult<IEnumerable<QuizQuestionDto>>> GetAllQuizQuestion()
    {
        var quizQuestionList = await _quizQuestionRepository.GetAllQuizQuestion();
        if (!quizQuestionList.Any())
        {
            return new ApiResult<IEnumerable<QuizQuestionDto>>(false, "Quiz Question is null !!!");
        }

        var result = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionList);
        return new ApiSuccessResult<IEnumerable<QuizQuestionDto>>(result);
    }

    public async Task<ApiResult<PagedList<QuizQuestionDto>>> GetAllQuizQuestionPagination(
        PagingRequestParameters pagingRequestParameters)
    {
        var quizQuestionList = await _quizQuestionRepository.GetAllQuizQuestionPagination();
        if (!quizQuestionList.Any())
        {
            return new ApiResult<PagedList<QuizQuestionDto>>(false, "Quiz Question is null !!!");
        }

        var result = _mapper.ProjectTo<QuizQuestionDto>(quizQuestionList);
        var pagedResult = await PagedList<QuizQuestionDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);
        return new ApiSuccessResult<PagedList<QuizQuestionDto>>(pagedResult);
    }

    public async Task<ApiResult<IEnumerable<QuizQuestionDto>>> GetAllQuizQuestionByQuizIdPractice(int quizId)
    {
        var quizQuestionList = await _quizQuestionRepository.GetAllQuizQuestionByQuizId(quizId);
        if (!quizQuestionList.Any())
        {
            return new ApiResult<IEnumerable<QuizQuestionDto>>(false, "Quiz Question is null !!!");
        }

        var result = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionList);
        return new ApiSuccessResult<IEnumerable<QuizQuestionDto>>(result);
    }

    public async Task<ApiResult<PagedList<QuizQuestionTestDto>>> GetAllQuizQuestionByQuizIdPaginationTest(int quizId, PagingRequestParameters pagingRequestParameters)
    {
        var quizQuestionList = await _quizQuestionRepository.GetAllQuizQuestionByQuizId(quizId);
        if (!quizQuestionList.Any())
        {
            return new ApiResult<PagedList<QuizQuestionTestDto>>(false, "Quiz Question is null !!!");
        }

        var result = _mapper.ProjectTo<QuizQuestionTestDto>(quizQuestionList);
        var pagedResult = await PagedList<QuizQuestionTestDto>.ToPageListAsync(result, pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);
        return new ApiSuccessResult<PagedList<QuizQuestionTestDto>>(pagedResult);
    }

    public async Task<ApiResult<PagedList<QuizQuestionDto>>> SearchQuizQuestion(SearchQuizQuestionDto searchQuizQuestionDto)
    {
        var quizQuestionEntity =
            await _elasticSearchService.SearchDocumentAsync(ElasticConstant.ElasticQuizQuestion, searchQuizQuestionDto);
        if (quizQuestionEntity is null)
        {
            return new ApiResult<PagedList<QuizQuestionDto>>(false,
                $"Lesson not found by {searchQuizQuestionDto.Key} !!!");
        }

        if (searchQuizQuestionDto.QuizId > 0)
        {
            quizQuestionEntity = quizQuestionEntity.Where(t => t.QuizId == searchQuizQuestionDto.QuizId).ToList();
        }

        var result = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionEntity);
        var pagedResult = await PagedList<QuizQuestionDto>.ToPageListAsync(result.AsQueryable().BuildMock(),
            searchQuizQuestionDto.PageIndex, searchQuizQuestionDto.PageSize, searchQuizQuestionDto.OrderBy,
            searchQuizQuestionDto.IsAscending);
        return new ApiSuccessResult<PagedList<QuizQuestionDto>>(pagedResult);
    }

    public async Task<ApiResult<QuizQuestionDto>> GetQuizQuestionById(int id)
    {
        var quizQuestionEntity = await _quizQuestionRepository.GetQuizQuestionById(id);
        if (quizQuestionEntity is null)
        {
            return new ApiResult<QuizQuestionDto>(false, "Quiz Question is null !!!");
        }

        var result = _mapper.Map<QuizQuestionDto>(quizQuestionEntity);
        return new ApiSuccessResult<QuizQuestionDto>(result);
    }

    public async Task<ApiResult<QuizQuestionDto>> CreateQuizQuestion(QuizQuestionCreateDto quizQuestionCreateDto)
    {
        var quizEntity = await _quizRepository.GetQuizById(quizQuestionCreateDto.QuizId);
        if (quizEntity is null)
        {
            return new ApiResult<QuizQuestionDto>(false, "Quiz not found !!!");
        }

        var countAnswer = quizQuestionCreateDto.QuizAnswers.Count();
        switch (quizQuestionCreateDto.Type)
        {
            case ETypeQuestion.QuestionTrueFalse:
                if (countAnswer != 2)
                {
                    return new ApiResult<QuizQuestionDto>(false, "Question is true or false !!!");
                }

                break;
            case ETypeQuestion.QuestionFourAnswer:
                if (countAnswer != 4)
                {
                    return new ApiResult<QuizQuestionDto>(false, "Question is four answer !!!");
                }

                break;
            case ETypeQuestion.QuestionFiveAnswer:
                if (countAnswer != 5)
                {
                    return new ApiResult<QuizQuestionDto>(false, "Question is five answer !!!");
                }

                break;
        }

        var quizQuestionEntity = _mapper.Map<QuizQuestion>(quizQuestionCreateDto);
        quizQuestionEntity.KeyWord = quizQuestionCreateDto.Content.RemoveDiacritics();
        var quizQuestionCreate = await _quizQuestionRepository.CreateQuizQuestion(quizQuestionEntity);
        var result = _mapper.Map<QuizQuestionDto>(quizQuestionCreate);
        _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticQuizQuestion, result, q => q.Id);
        return new ApiSuccessResult<QuizQuestionDto>(result);
    }

    public async Task<ApiResult<QuizQuestionDto>> UpdateQuizQuestion(QuizQuestionUpdateDto quizQuestionUpdateDto)
    {
        var quizQuestionEntity = await _quizQuestionRepository.GetQuizQuestionById(quizQuestionUpdateDto.Id);
        if (quizQuestionEntity is null)
        {
            return new ApiResult<QuizQuestionDto>(false, "Quiz Question not found !!!");
        }

        var quizEntity = await _quizRepository.GetQuizById(quizQuestionUpdateDto.QuizId);
        if (quizEntity is null)
        {
            return new ApiResult<QuizQuestionDto>(false, "Quiz not found !!!");
        }

        var countAnswer = quizQuestionUpdateDto.QuizAnswers.Count();
        switch (quizQuestionUpdateDto.Type)
        {
            case ETypeQuestion.QuestionTrueFalse:
                if (countAnswer != 2)
                {
                    return new ApiResult<QuizQuestionDto>(false, "Question is true or false !!!");
                }

                break;
            case ETypeQuestion.QuestionFourAnswer:
                if (countAnswer != 4)
                {
                    return new ApiResult<QuizQuestionDto>(false, "Question is four answer !!!");
                }

                break;
            case ETypeQuestion.QuestionFiveAnswer:
                if (countAnswer != 5)
                {
                    return new ApiResult<QuizQuestionDto>(false, "Question is five answer !!!");
                }

                break;
        }

        var modelQuestion = _mapper.Map(quizQuestionUpdateDto, quizQuestionEntity);
        modelQuestion.KeyWord = quizQuestionUpdateDto.Content.RemoveDiacritics();
        var quizQuestionUpdate = await _quizQuestionRepository.UpdateQuizQuestion(modelQuestion);
        foreach (var item in quizQuestionUpdateDto.QuizAnswers)
        {
            var quizAnswer = await _quizAnswerRepository.GetQuizAnswerById(item.Id);
            var modelAnswer = _mapper.Map(item, quizAnswer);
            await _quizAnswerRepository.UpdateQuizAnswer(modelAnswer);
        }

        var result = _mapper.Map<QuizQuestionDto>(quizQuestionUpdate);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticQuizQuestion, result,
            quizQuestionUpdateDto.Id);
        return new ApiSuccessResult<QuizQuestionDto>(result);
    }

    public async Task<ApiResult<bool>> CreateRangeQuizQuestion(IEnumerable<QuizQuestionCreateDto> quizQuestionsCreateDto)
    {
        foreach (var item in quizQuestionsCreateDto)
        {
            var quizEntity = await _quizRepository.GetQuizById(item.QuizId);
            if (quizEntity is null)
            {
                return new ApiResult<bool>(false, "Quiz not found !!!");
            }

            var countAnswer = item.QuizAnswers.Count();
            switch (item.Type)
            {
                case ETypeQuestion.QuestionTrueFalse:
                    if (countAnswer != 2)
                    {
                        return new ApiResult<bool>(false, "Question is true or false !!!");
                    }

                    break;
                case ETypeQuestion.QuestionFourAnswer:
                    if (countAnswer != 4)
                    {
                        return new ApiResult<bool>(false, "Question is four answer !!!");
                    }

                    break;
                case ETypeQuestion.QuestionFiveAnswer:
                    if (countAnswer != 5)
                    {
                        return new ApiResult<bool>(false, "Question is five answer !!!");
                    }

                    break;
            }

            var quizQuestionEntity = _mapper.Map<QuizQuestion>(item);
            quizQuestionEntity.KeyWord = item.Content.RemoveDiacritics();
            var quizQuestionCreate = await _quizQuestionRepository.CreateQuizQuestion(quizQuestionEntity);
            var result = _mapper.Map<QuizQuestionDto>(quizQuestionCreate);
            _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticQuizQuestion, result, q => q.Id);
        }

        return new ApiSuccessResult<bool>(true);
    }

    public async Task<ApiResult<bool>> UpdateRangeQuizQuestion(IEnumerable<QuizQuestionUpdateDto> quizQuestionsUpdateDto)
    {
        foreach (var item in quizQuestionsUpdateDto)
        {
            var quizQuestionEntity = await _quizQuestionRepository.GetQuizQuestionById(item.Id);
            if (quizQuestionEntity is null)
            {
                return new ApiResult<bool>(false, "Quiz Question not found !!!");
            }

            var quizEntity = await _quizRepository.GetQuizById(item.QuizId);
            if (quizEntity is null)
            {
                return new ApiResult<bool>(false, "Quiz not found !!!");
            }

            var countAnswer = item.QuizAnswers.Count();
            switch (item.Type)
            {
                case ETypeQuestion.QuestionTrueFalse:
                    if (countAnswer != 2)
                    {
                        return new ApiResult<bool>(false, "Question is true or false !!!");
                    }

                    break;
                case ETypeQuestion.QuestionFourAnswer:
                    if (countAnswer != 4)
                    {
                        return new ApiResult<bool>(false, "Question is four answer !!!");
                    }

                    break;
                case ETypeQuestion.QuestionFiveAnswer:
                    if (countAnswer != 5)
                    {
                        return new ApiResult<bool>(false, "Question is five answer !!!");
                    }

                    break;
            }

            var modelQuestion = _mapper.Map(item, quizQuestionEntity);
            modelQuestion.KeyWord = item.Content.RemoveDiacritics();
            var quizQuestionUpdate = await _quizQuestionRepository.UpdateQuizQuestion(modelQuestion);
            foreach (var itemAnswer in item.QuizAnswers)
            {
                var quizAnswer = await _quizAnswerRepository.GetQuizAnswerById(itemAnswer.Id);
                var modelAnswer = _mapper.Map(itemAnswer, quizAnswer);
                await _quizAnswerRepository.UpdateQuizAnswer(modelAnswer);
            }
            var result = _mapper.Map<QuizQuestionDto>(quizQuestionUpdate);
            _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticQuizQuestion, result, item.Id);
        }

        return new ApiSuccessResult<bool>(true);
    }

    public async Task<ApiResult<bool>> UpdateStatusQuizQuestion(int id)
    {
        var quizQuestionEntity = await _quizQuestionRepository.GetQuizQuestionById(id);
        if (quizQuestionEntity is null)
        {
            return new ApiResult<bool>(false, "Quiz Question not found !!!");
        }

        quizQuestionEntity.IsActive = !quizQuestionEntity.IsActive;
        await _quizQuestionRepository.UpdateQuizQuestion(quizQuestionEntity);
        var result = _mapper.Map<QuizQuestionDto>(quizQuestionEntity);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticQuizQuestion, result, id);
        return new ApiSuccessResult<bool>(true, "Quiz Question update successfully !!!");
    }

    public async Task<ApiResult<bool>> DeleteQuizQuestion(int id)
    {
        var quizQuestion = await _quizQuestionRepository.GetQuizQuestionById(id);
        if (quizQuestion is null)
        {
            return new ApiResult<bool>(false, "Quiz Question not found !!!");
        }

        var quizQuestionDelete = await _quizQuestionRepository.DeleteQuizQuestion(id);
        if (!quizQuestionDelete)
        {
            return new ApiResult<bool>(false, "Delete Quiz Question Failed !!!");
        }
        _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticQuizQuestion, id);
        return new ApiSuccessResult<bool>(true);
    }
}