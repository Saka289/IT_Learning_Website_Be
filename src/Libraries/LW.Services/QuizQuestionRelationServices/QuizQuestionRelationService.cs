using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.QuizQuestionRelationRepositories;
using LW.Data.Repositories.QuizQuestionRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Shared.DTOs.QuizQuestionRelation;
using LW.Shared.SeedWork;
using Serilog;

namespace LW.Services.QuizQuestionRelationServices;

public class QuizQuestionRelationService : IQuizQuestionRelationService
{
    private readonly IQuizQuestionRelationRepository _quizQuestionRelationRepository;
    private readonly IQuizQuestionRepository _quizQuestionRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public QuizQuestionRelationService(IQuizQuestionRelationRepository quizQuestionRelationRepository, IMapper mapper,
        IQuizRepository quizRepository, ILogger logger, IQuizQuestionRepository quizQuestionRepository)
    {
        _quizQuestionRelationRepository = quizQuestionRelationRepository;
        _mapper = mapper;
        _quizRepository = quizRepository;
        _logger = logger;
        _quizQuestionRepository = quizQuestionRepository;
    }

    public async Task<ApiResult<bool>> CreateQuizQuestionRelationByQuizCustom(QuizQuestionRelationCustomCreateDto quizQuestionRelationCustomCreateDto)
    {
        var quiz = await _quizRepository.GetQuizById(quizQuestionRelationCustomCreateDto.QuizId);
        if (quiz is null)
        {
            return new ApiResult<bool>(false, "Quiz not found !!!");
        }

        var quizQuestionRelation = new List<QuizQuestionRelation>();
        var quizQuestionOld = (await _quizQuestionRepository.GetAllQuizQuestionByQuizId(quizQuestionRelationCustomCreateDto.QuizId)).Select(q => q.Id);
        foreach (var item in quizQuestionRelationCustomCreateDto.QuiQuestionRelationCustomCreate)
        {
            var quizQuestion = await _quizQuestionRepository.GetAllQuizQuestionByQuizId(item.QuizChildId);
            if (!quizQuestion.Any())
            {
                _logger.Information($"Quiz Question: Get all quiz question by quizId {item.QuizChildId} not found !!!");
            }
            if (item.Shuffle)
            {
                quizQuestion = quizQuestion.ToList().OrderBy(x => Random.Shared.Next()).AsQueryable();
            }
            quizQuestion = quizQuestion.Take(item.NumberOfQuestion);
            
            foreach (var itemBoth in quizQuestion.Select(q => q.Id).Intersect(quizQuestionOld))
            {
                _logger.Information($"Quiz Relation: Quiz Question ID {itemBoth} is exits !!!");
            }
            foreach (var itemQuestion in quizQuestion.Select(q => q.Id).Except(quizQuestionOld))
            {
                var quizQuestionCheck = quizQuestionRelation.Any(x => x.QuizQuestionId == itemQuestion);
                if (!quizQuestionCheck)
                {
                    quizQuestionRelation.Add(new()
                    {
                        QuizId = quizQuestionRelationCustomCreateDto.QuizId,
                        QuizQuestionId = itemQuestion
                    });
                }
                else
                {
                    _logger.Information($"Quiz Relation: Quiz Question ID {itemQuestion} is exits !!!");
                }
            }
        }
        
        var create = await _quizQuestionRelationRepository.CreateRangeQuizQuestionRelation(quizQuestionRelation);
        if (!create)
        {
            return new ApiResult<bool>(false, "Failed to create QuizQuestionRelation !!!");
        }

        return new ApiSuccessResult<bool>(create);
    }

    public async Task<ApiResult<bool>> CreateQuizQuestionRelation(QuizQuestionRelationCreateDto quizQuestionRelationCreateDto)
    {
        var quizEntity = await _quizRepository.GetQuizById(quizQuestionRelationCreateDto.QuizId);
        if (quizEntity is null)
        {
            return new ApiResult<bool>(false, "Quiz not found !!!");
        }

        var quizQuestionOld = (await _quizQuestionRepository.GetAllQuizQuestionByQuizId(quizQuestionRelationCreateDto.QuizId)).Select(q => q.Id);

        var quizQuestionRelationBoth = quizQuestionRelationCreateDto.QuizQuestionIds.Intersect(quizQuestionOld).ToList();
        foreach (var item in quizQuestionRelationBoth)
        {
            _logger.Information($"Quiz Relation: Quiz Question ID {item} is exits !!!");
        }
        
        var quizQuestionRelation = quizQuestionRelationCreateDto.QuizQuestionIds.Except(quizQuestionOld).Select(quizQuestionId =>
            new QuizQuestionRelation
            {
                QuizId = quizQuestionRelationCreateDto.QuizId,
                QuizQuestionId = quizQuestionId
            }).ToList();
        var create = await _quizQuestionRelationRepository.CreateRangeQuizQuestionRelation(quizQuestionRelation);
        if (!create)
        {
            return new ApiResult<bool>(false, "Failed to create QuizQuestionRelation !!!");
        }

        return new ApiSuccessResult<bool>(create);
    }

    public async Task<ApiResult<bool>> UpdateQuizQuestionRelation(QuizQuestionRelationUpdateDto quizQuestionRelationUpdateDto)
    {
        var quizEntity = await _quizRepository.GetQuizById(quizQuestionRelationUpdateDto.QuizId);
        if (quizEntity is null)
        {
            return new ApiResult<bool>(false, "Quiz not found !!!");
        }

        var listRelation = await _quizQuestionRelationRepository.GetAllQuizQuestionRelationByQuizId(quizQuestionRelationUpdateDto.QuizId);
        var deleteRelation = await _quizQuestionRelationRepository.DeleteRangeQuizQuestionRelation(listRelation);
        if (!deleteRelation)
        {
            return new ApiResult<bool>(false, "Failed to update QuizQuestionRelation !!!");
        }
        var quizQuestionRelation = quizQuestionRelationUpdateDto.QuizQuestionIds.Select(quizQuestionId =>
            new QuizQuestionRelation
            {
                QuizId = quizQuestionRelationUpdateDto.QuizId,
                QuizQuestionId = quizQuestionId
            }).ToList();
        var update = await _quizQuestionRelationRepository.CreateRangeQuizQuestionRelation(quizQuestionRelation);
        if (!update)
        {
            return new ApiResult<bool>(false, "Failed to update QuizQuestionRelation !!!");
        }

        return new ApiSuccessResult<bool>(update);
    }

    public async Task<ApiResult<bool>> DeleteQuizQuestionRelation(int id)
    {
        var quizQuestionRelation = await _quizQuestionRelationRepository.GetQuizQuestionRelationById(id);
        if (quizQuestionRelation is null)
        {
            return new ApiResult<bool>(false, "Quiz Question Relation not found !!!");
        }

        var delete = await _quizQuestionRelationRepository.DeleteQuizQuestionRelation(id);
        if (!delete)
        {
            return new ApiResult<bool>(false, "Failed to delete QuizQuestionRelation !!!");
        }
        return new ApiSuccessResult<bool>(delete);
    }

    public async Task<ApiResult<bool>> DeleteRangeQuizQuestionRelation(IEnumerable<int> ids)
    {
        var listRelations = new List<QuizQuestionRelation>();
        foreach (var item in ids)
        {
            var quizQuestionRelation = await _quizQuestionRelationRepository.GetQuizQuestionRelationById(item);
            if (quizQuestionRelation is not null)
            {
                listRelations.Add(quizQuestionRelation);
            }
            _logger.Information($"Not found with id {item}");
        }
        
        var delete = await _quizQuestionRelationRepository.DeleteRangeQuizQuestionRelation(listRelations);
        if (!delete)
        {
            return new ApiResult<bool>(false, "Failed to delete QuizQuestionRelation !!!");
        }
        return new ApiSuccessResult<bool>(delete);
    }
}