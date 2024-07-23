using System.Collections;
using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.QuizAnswerRepositories;
using LW.Data.Repositories.QuizQuestionRelationRepositories;
using LW.Data.Repositories.QuizQuestionRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.QuizAnswer;
using LW.Shared.DTOs.QuizQuestion;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;

namespace LW.Services.QuizQuestionServices;

public class QuizQuestionService : IQuizQuestionService
{
    private readonly IQuizAnswerRepository _quizAnswerRepository;
    private readonly IQuizQuestionRepository _quizQuestionRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IElasticSearchService<QuizQuestionDto, int> _elasticSearchService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IMapper _mapper;

    public QuizQuestionService(IQuizAnswerRepository quizAnswerRepository,
        IQuizQuestionRepository quizQuestionRepository, IMapper mapper, IQuizRepository quizRepository,
        IElasticSearchService<QuizQuestionDto, int> elasticSearchService, ICloudinaryService cloudinaryService)
    {
        _quizAnswerRepository = quizAnswerRepository;
        _quizQuestionRepository = quizQuestionRepository;
        _mapper = mapper;
        _quizRepository = quizRepository;
        _elasticSearchService = elasticSearchService;
        _cloudinaryService = cloudinaryService;
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

    public async Task<ApiResult<IEnumerable<QuizQuestionDto>>> GetAllQuizQuestionByQuizIdPractice(int quizId,
        int? size = 0)
    {
        var quizQuestionList = await _quizQuestionRepository.GetAllQuizQuestionByQuizId(quizId);
        if (!quizQuestionList.Any())
        {
            return new ApiResult<IEnumerable<QuizQuestionDto>>(false, "Quiz Question is null !!!");
        }

        var quiz = await _quizRepository.GetQuizById(quizId);

        if (quiz.IsShuffle)
        {
            quizQuestionList = quizQuestionList.ToList().OrderBy(x => Random.Shared.Next()).AsQueryable();
        }

        foreach (var item in quizQuestionList)
        {
            if (item.IsShuffle)
            {
                item.QuizAnswers = item.QuizAnswers.OrderBy(x => Random.Shared.Next()).ToList();
            }
        }

        if (size > 0)
        {
            quizQuestionList = quizQuestionList.Take(Convert.ToInt32(size));
        }

        var result = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionList);
        return new ApiSuccessResult<IEnumerable<QuizQuestionDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<QuizQuestionTestDto>>> GetAllQuizQuestionByQuizIdTest(int quizId,
        int? size = 0)
    {
        var quizQuestionList = await _quizQuestionRepository.GetAllQuizQuestionByQuizId(quizId);
        if (!quizQuestionList.Any())
        {
            return new ApiResult<IEnumerable<QuizQuestionTestDto>>(false, "Quiz Question is null !!!");
        }

        var quiz = await _quizRepository.GetQuizById(quizId);

        if (quiz.IsShuffle)
        {
            quizQuestionList = quizQuestionList.ToList().OrderBy(x => Random.Shared.Next()).AsQueryable();
        }

        foreach (var item in quizQuestionList)
        {
            if (item.IsShuffle)
            {
                item.QuizAnswers = item.QuizAnswers.OrderBy(x => Random.Shared.Next()).ToList();
            }
        }

        if (size > 0)
        {
            quizQuestionList = quizQuestionList.Take(Convert.ToInt32(size));
        }

        var result = _mapper.Map<IEnumerable<QuizQuestionTestDto>>(quizQuestionList);
        return new ApiSuccessResult<IEnumerable<QuizQuestionTestDto>>(result);
    }

    public async Task<ApiResult<PagedList<QuizQuestionDto>>> SearchQuizQuestion(
        SearchQuizQuestionDto searchQuizQuestionDto)
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
            quizQuestionEntity = quizQuestionEntity
                .Where(t => t.QuizQuestionRelations.Any(t => t.QuizId == searchQuizQuestionDto.QuizId)).ToList();
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
        var countAnswer = quizQuestionCreateDto.QuizAnswers.Count();
        var countAnswerTrue = quizQuestionCreateDto.QuizAnswers.Count(x => x.IsCorrect);
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
            case ETypeQuestion.QuestionMultiChoice:
                if (countAnswer != 6 || countAnswerTrue < 1)
                {
                    return new ApiResult<QuizQuestionDto>(false,
                        "Question is six answer and more than 1 correct answer !!!");
                }

                break;
        }

        var quizQuestionEntity = _mapper.Map<QuizQuestion>(quizQuestionCreateDto);
        if (quizQuestionCreateDto.QuizId > 0)
        {
            quizQuestionEntity.QuizQuestionRelations = new List<QuizQuestionRelation>
            {
                new() { QuizId = quizQuestionCreateDto.QuizId }
            };
        }

        quizQuestionEntity.KeyWord = quizQuestionCreateDto.Content.RemoveDiacritics();
        if (quizQuestionCreateDto.Image != null && quizQuestionCreateDto.Image.Length > 0)
        {
            var filePath = await _cloudinaryService.CreateImageAsync(quizQuestionCreateDto.Image,
                CloudinaryConstant.FolderQuestionImage);
            quizQuestionEntity.Image = filePath.Url;
            quizQuestionEntity.PublicId = filePath.PublicId;
        }

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

        var countAnswer = quizQuestionUpdateDto.QuizAnswers.Count();
        var countAnswerTrue = quizQuestionUpdateDto.QuizAnswers.Count(x => x.IsCorrect);
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
            case ETypeQuestion.QuestionMultiChoice:
                if (countAnswer != 6 || countAnswerTrue < 1)
                {
                    return new ApiResult<QuizQuestionDto>(false,
                        "Question is six answer and more than 1 correct answer !!!");
                }

                break;
        }

        var modelQuestion = _mapper.Map(quizQuestionUpdateDto, quizQuestionEntity);
        var quizQuestion = await _quizQuestionRepository.GetQuizQuestionById(quizQuestionUpdateDto.Id);
        if (quizQuestionUpdateDto.Image != null && quizQuestionUpdateDto.Image.Length > 0)
        {
            var filePath =
                await _cloudinaryService.UpdateImageAsync(quizQuestionEntity.PublicId, quizQuestionUpdateDto.Image);
            quizQuestionEntity.Image = filePath.Url;
            quizQuestionEntity.PublicId = filePath.PublicId;
        }

        modelQuestion.QuizQuestionRelations = quizQuestion.QuizQuestionRelations;
        modelQuestion.KeyWord = quizQuestionUpdateDto.Content.RemoveDiacritics();
        modelQuestion.Image = quizQuestion.Image;
        modelQuestion.PublicId = quizQuestion.PublicId;
        var quizQuestionUpdate = await _quizQuestionRepository.UpdateQuizQuestion(modelQuestion);
        if (quizQuestion.Type.Equals(quizQuestionUpdateDto.Type))
        {
            foreach (var item in quizQuestionUpdateDto.QuizAnswers)
            {
                var quizAnswer = await _quizAnswerRepository.GetQuizAnswerById(item.Id);
                if (quizAnswer is null)
                {
                    return new ApiResult<QuizQuestionDto>(false, "Quiz answer not found !!!");
                }
                var modelAnswer = _mapper.Map(item, quizAnswer);
                await _quizAnswerRepository.UpdateQuizAnswer(modelAnswer);
            }
        }
        else
        {
            var quizAnswers = await _quizAnswerRepository.GetAllQuizAnswerByQuizQuestionId(quizQuestionUpdateDto.Id);
            if (quizAnswers.Any())
            {
                await _quizAnswerRepository.DeleteRangeAnswer(quizAnswers);
                var quizAnswer = _mapper.Map<IEnumerable<QuizAnswer>>(quizQuestionUpdateDto.QuizAnswers);
                await _quizAnswerRepository.CreateRangeQuizAnswer(quizAnswer);
                quizQuestionUpdate.QuizAnswers = quizAnswer.ToList();
            }
        }

        var result = _mapper.Map<QuizQuestionDto>(quizQuestionUpdate);
        _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticQuizQuestion, result, quizQuestionUpdateDto.Id);
        return new ApiSuccessResult<QuizQuestionDto>(result);
    }

    public async Task<ApiResult<bool>> CreateRangeQuizQuestion(
        IEnumerable<QuizQuestionCreateDto> quizQuestionsCreateDto)
    {
        foreach (var item in quizQuestionsCreateDto)
        {
            var countAnswer = item.QuizAnswers.Count();
            var countAnswerTrue = item.QuizAnswers.Count(x => x.IsCorrect);
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
                case ETypeQuestion.QuestionMultiChoice:
                    if (countAnswer != 6 || countAnswerTrue < 1)
                    {
                        return new ApiResult<bool>(false, "Question is six answer and more than 1 correct answer !!!");
                    }

                    break;
            }

            var quizQuestionEntity = _mapper.Map<QuizQuestion>(item);
            quizQuestionEntity.QuizQuestionRelations = new List<QuizQuestionRelation>
            {
                new() { QuizId = item.QuizId }
            };
            quizQuestionEntity.KeyWord = item.Content.RemoveDiacritics();
            if (item.Image != null && item.Image.Length > 0)
            {
                var filePath =
                    await _cloudinaryService.CreateImageAsync(item.Image, CloudinaryConstant.FolderQuestionImage);
                quizQuestionEntity.Image = filePath.Url;
                quizQuestionEntity.PublicId = filePath.PublicId;
            }

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

            var countAnswer = item.QuizAnswers.Count();
            var countAnswerTrue = item.QuizAnswers.Count(x => x.IsCorrect);
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
                case ETypeQuestion.QuestionMultiChoice:
                    if (countAnswer != 6 || countAnswerTrue < 1)
                    {
                        return new ApiResult<bool>(false, "Question is six answer and more than 1 correct answer !!!");
                    }

                    break;
            }

            var modelQuestion = _mapper.Map(item, quizQuestionEntity);
            var quizQuestion = await _quizQuestionRepository.GetQuizQuestionById(item.Id);
            if (item.Image != null && item.Image.Length > 0)
            {
                var filePath = await _cloudinaryService.UpdateImageAsync(quizQuestionEntity.PublicId, item.Image);
                quizQuestionEntity.Image = filePath.Url;
                quizQuestionEntity.PublicId = filePath.PublicId;
            }

            modelQuestion.QuizQuestionRelations = quizQuestion.QuizQuestionRelations;
            modelQuestion.KeyWord = item.Content.RemoveDiacritics();
            modelQuestion.Image = quizQuestion.Image;
            modelQuestion.PublicId = quizQuestion.PublicId;
            var quizQuestionUpdate = await _quizQuestionRepository.UpdateQuizQuestion(modelQuestion);
            if (quizQuestion.Type.Equals(item.Type))
            {
                foreach (var itemAnswer in item.QuizAnswers)
                {
                    var quizAnswer = await _quizAnswerRepository.GetQuizAnswerById(itemAnswer.Id);
                    if (quizAnswer is null)
                    {
                        return new ApiResult<bool>(false, "Quiz answer not found !!!");
                    }
                    var modelAnswer = _mapper.Map(itemAnswer, quizAnswer);
                    await _quizAnswerRepository.UpdateQuizAnswer(modelAnswer);
                }
            }
            else
            {
                var quizAnswers = await _quizAnswerRepository.GetAllQuizAnswerByQuizQuestionId(item.Id);
                if (quizAnswers.Any())
                {
                    await _quizAnswerRepository.DeleteRangeAnswer(quizAnswers);
                    var quizAnswer = _mapper.Map<IEnumerable<QuizAnswer>>(item.QuizAnswers);
                    await _quizAnswerRepository.CreateRangeQuizAnswer(quizAnswer);
                    quizQuestionUpdate.QuizAnswers = quizAnswer.ToList();
                }
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