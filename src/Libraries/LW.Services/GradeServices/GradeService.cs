using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.CompetitionRepositories;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Data.Repositories.GradeRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.SolutionRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Services.SolutionServices;
using LW.Shared.Constant;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.Grade;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Problem;
using LW.Shared.DTOs.Quiz;
using LW.Shared.DTOs.Solution;
using LW.Shared.DTOs.Topic;
using LW.Shared.SeedWork;
using MockQueryable.Moq;

namespace LW.Services.GradeServices;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IElasticSearchService<GradeDto, int> _elasticSearchService;
    private readonly IElasticSearchService<DocumentDto, int> _elasticSearchDocumentService;
    private readonly IElasticSearchService<TopicDto, int> _elasticSearchTopicService;
    private readonly IElasticSearchService<LessonDto, int> _elasticSearchLessonService;
    private readonly IElasticSearchService<ProblemDto, int> _elasticSearchProblemService;
    private readonly IElasticSearchService<QuizDto, int> _elasticSearchQuizService;
    private readonly IElasticSearchService<SolutionDto, int> _elasticSearchSolutionService;
    private readonly IElasticSearchService<ExamDto, int> _elasticSearchExamService;
    private readonly IMapper _mapper;
    private readonly IDocumentRepository _documentRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IProblemRepository _problemRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IExamRepository _examRepository;
    private readonly ISolutionRepository _solutionRepository;


    public GradeService(IGradeRepository gradeRepository, IMapper mapper,
        IElasticSearchService<GradeDto, int> elasticSearchService,
        IElasticSearchService<DocumentDto, int> elasticSearchDocumentService,
        IElasticSearchService<TopicDto, int> elasticSearchTopicService,
        IElasticSearchService<LessonDto, int> elasticSearchLessonService, IDocumentRepository documentRepository,
        ITopicRepository topicRepository, ILessonRepository lessonRepository, IProblemRepository problemRepository, IQuizRepository quizRepository, IExamRepository examRepository, IElasticSearchService<ProblemDto, int> elasticSearchProblemService, IElasticSearchService<QuizDto, int> elasticSearchQuizService, ISolutionRepository solutionRepository, IElasticSearchService<SolutionDto, int> elasticSearchSolutionService, IElasticSearchService<ExamDto, int> elasticSearchExamService)
    {
        _gradeRepository = gradeRepository;
        _mapper = mapper;
        _elasticSearchService = elasticSearchService;
        _elasticSearchDocumentService = elasticSearchDocumentService;
        _elasticSearchTopicService = elasticSearchTopicService;
        _elasticSearchLessonService = elasticSearchLessonService;
        _documentRepository = documentRepository;
        _topicRepository = topicRepository;
        _lessonRepository = lessonRepository;
        _problemRepository = problemRepository;
        _quizRepository = quizRepository;
        _examRepository = examRepository;
        _elasticSearchProblemService = elasticSearchProblemService;
        _elasticSearchQuizService = elasticSearchQuizService;
        _solutionRepository = solutionRepository;
        _elasticSearchSolutionService = elasticSearchSolutionService;
        _elasticSearchExamService = elasticSearchExamService;
    }

    public async Task<ApiResult<IEnumerable<GradeDto>>> GetAllGrade()
    {
        var gradeList = await _gradeRepository.GetAllGrade();
        if (!gradeList.Any())
        {
            return new ApiResult<IEnumerable<GradeDto>>(false, "Grade is null !!!");
        }

        var result = _mapper.Map<IEnumerable<GradeDto>>(gradeList);
        return new ApiSuccessResult<IEnumerable<GradeDto>>(result);
    }

    public async Task<ApiResult<PagedList<GradeDto>>> GetAllGradePagination(SearchGradeDto searchGradeDto)
    {
        IEnumerable<GradeDto> gradeList;
        if (!string.IsNullOrEmpty(searchGradeDto.Value))
        {
            var gradeListSearch = await _elasticSearchService.SearchDocumentFieldAsync(ElasticConstant.ElasticGrades,
                new SearchRequestValue
                {
                    Value = searchGradeDto.Value,
                    Size = searchGradeDto.Size,
                });
            if (gradeListSearch is null)
            {
                return new ApiResult<PagedList<GradeDto>>(false, "Grade not found !!!");
            }

            gradeList = gradeListSearch.ToList();
        }
        else
        {
            var gradeListAll = await _gradeRepository.GetAllGradePagination();
            if (!gradeListAll.Any())
            {
                return new ApiResult<PagedList<GradeDto>>(false, "Grade is null !!!");
            }

            gradeList = _mapper.Map<IEnumerable<GradeDto>>(gradeListAll);
        }

        var result = _mapper.Map<IEnumerable<GradeDto>>(gradeList);
        var pagedResult = await PagedList<GradeDto>.ToPageListAsync(result.AsQueryable().BuildMock(),
            searchGradeDto.PageIndex, searchGradeDto.PageSize, searchGradeDto.OrderBy, searchGradeDto.IsAscending);
        return new ApiSuccessResult<PagedList<GradeDto>>(pagedResult);
    }

    public async Task<ApiResult<GradeDto>> GetGradeById(int id)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(id, true);
        if (gradeEntity == null)
        {
            return new ApiResult<GradeDto>(false, "Grade is null !!!");
        }

        var result = _mapper.Map<GradeDto>(gradeEntity);
        return new ApiSuccessResult<GradeDto>(result);
    }

    public async Task<ApiResult<GradeDto>> CreateGrade(GradeCreateDto gradeCreateDto)
    {
        var gradeEntity = _mapper.Map<Grade>(gradeCreateDto);
        gradeEntity.KeyWord = gradeCreateDto.Title.RemoveDiacritics();
        await _gradeRepository.CreateGrade(gradeEntity);
        await _gradeRepository.SaveChangesAsync();
        await CreateOrUpdateElasticGrade(gradeEntity.Id, true);
        var result = _mapper.Map<GradeDto>(gradeEntity);
        return new ApiSuccessResult<GradeDto>(result);
    }

    public async Task<ApiResult<GradeDto>> UpdateGrade(GradeUpdateDto gradeUpdateDto)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(gradeUpdateDto.Id, false);
        if (gradeEntity is null)
        {
            return new ApiResult<GradeDto>(false, "Grade not found !!!");
        }

        var model = _mapper.Map(gradeUpdateDto, gradeEntity);
        model.KeyWord = gradeUpdateDto.Title.RemoveDiacritics();
        var updateGrade = await _gradeRepository.UpdateGrade(model);
        await _gradeRepository.SaveChangesAsync();
        await CreateOrUpdateElasticGrade(gradeUpdateDto.Id, false);
        var result = _mapper.Map<GradeDto>(updateGrade);
        return new ApiSuccessResult<GradeDto>(result);
    }

    public async Task<ApiResult<bool>> UpdateGradeStatus(int id)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(id, false);
        if (gradeEntity is null)
        {
            return new ApiResult<bool>(false, "Grade not found !!!");
        }

        gradeEntity.IsActive = !gradeEntity.IsActive;
        await _gradeRepository.UpdateGrade(gradeEntity);
        await _gradeRepository.SaveChangesAsync();
        var result = _mapper.Map<GradeDto>(gradeEntity);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticGrades, result, id);
        return new ApiSuccessResult<bool>(true, "Grade update successfully !!!");
    }

    public async Task<ApiResult<bool>> DeleteGrade(int id)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(id, false);
        if (gradeEntity is null)
        {
            return new ApiResult<bool>(false, "Grade not found !!!");
        }

        // xóa document - topic - lesson
        var documents = await _documentRepository.GetAllDocumentByGrade(id);
        if (documents.Any())
        {
            var listDocumentDtoId = documents.Select(x => x.Id).ToList();
            await _elasticSearchDocumentService.DeleteDocumentRangeAsync(ElasticConstant.ElasticDocuments, listDocumentDtoId);
            foreach (var document in documents)
            {
                var listTopic = await _topicRepository.GetAllTopicByDocumentAll(document.Id);
                if (listTopic.Any())
                {
                    var listTopicDtoId = listTopic.Select(x => x.Id).ToList();
                    await _elasticSearchTopicService.DeleteDocumentRangeAsync(ElasticConstant.ElasticTopics, listTopicDtoId);
                    foreach (var topic in listTopic)
                    {
                        var listLesson = await _lessonRepository.GetAllLessonByTopic(topic.Id);
                        if (listLesson.Any())
                        {
                            var listLessonId = listLesson.Select(l => l.Id);
                            await _elasticSearchLessonService.DeleteDocumentRangeAsync(ElasticConstant.ElasticLessons, listLessonId);
                            foreach (var lesson in listLesson)
                            {
                                var listProblemInLesson = await _problemRepository.GetAllProblemByLesson(lesson.Id);
                                foreach (var problem in listProblemInLesson)
                                {
                                    var listSolutionInLesson = await _solutionRepository.GetAllSolutionByProblemId(problem.Id);
                                    if (listSolutionInLesson.Any())
                                    {
                                        var listSolutionInLessonId = listSolutionInLesson.Select(s => s.Id);
                                        await _elasticSearchSolutionService.DeleteDocumentRangeAsync(ElasticConstant.ElasticSolutions, listSolutionInLessonId);
                                    }
                                }
                                var listQuizInLesson = await _quizRepository.GetAllQuizByLessonId(lesson.Id);
                                if (listProblemInLesson.Any())
                                {
                                    var listProblemInLessonId = listProblemInLesson.Select(p => p.Id);
                                    await _elasticSearchProblemService.DeleteDocumentRangeAsync(ElasticConstant.ElasticProblems, listProblemInLessonId);
                                }
                                if (listQuizInLesson.Any())
                                {
                                    var listQuizInLessonId = listQuizInLesson.Select(q => q.Id);
                                    await _elasticSearchQuizService.DeleteDocumentRangeAsync(ElasticConstant.ElasticQuizzes, listQuizInLessonId);
                                }
                            }
                        }
                        var listProblem = await _problemRepository.GetAllProblemByTopic(topic.Id);
                        foreach (var problem in listProblem)
                        {
                            var listSolution = await _solutionRepository.GetAllSolutionByProblemId(problem.Id);
                            if (listSolution.Any())
                            {
                                var listSolutionInLessonId = listSolution.Select(s => s.Id);
                                await _elasticSearchSolutionService.DeleteDocumentRangeAsync(ElasticConstant.ElasticSolutions, listSolutionInLessonId);
                            }
                        }
                        var listQuiz = await _quizRepository.GetAllQuizByTopicId(topic.Id);
                        if (listQuiz.Any())
                        {
                            var listQuizId = listQuiz.Select(q => q.Id);   
                            await _elasticSearchQuizService.DeleteDocumentRangeAsync(ElasticConstant.ElasticQuizzes, listQuizId);
                        }

                        if (listProblem.Any())
                        {
                            var listProblemId = listProblem.Select(p => p.Id);
                            await _elasticSearchProblemService.DeleteDocumentRangeAsync(ElasticConstant.ElasticProblems, listProblemId);
                        }
                    }
                }
            }
        }

        var exams = await _examRepository.GetAllExamByGrade(id);
        if (exams.Any())
        {
            var listExamId = exams.Select(e => e.Id);
            await _elasticSearchExamService.DeleteDocumentRangeAsync(ElasticConstant.ElasticExams, listExamId);
        }

        var grade = await _gradeRepository.DeleteGrade(id);
        if (!grade)
        {
            return new ApiResult<bool>(false, "Failed Delete Grade not found !!!");
        }

        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticGrades, id);
        return new ApiSuccessResult<bool>(true, "Delete Grade Successfully !!!");
    }

    private async Task CreateOrUpdateElasticGrade(int id, bool isCreateOrUpdate)
    {
        var grade = await _gradeRepository.GetGradeById(id, true);
        var result = _mapper.Map<GradeDto>(grade);
        if (isCreateOrUpdate)
        {
            await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticGrades, result, g => g.Id);
        }
        else
        {
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticGrades, result, result.Id);
            var documents = await _documentRepository.GetAllDocumentByGrade(id);
            var resultDocument = _mapper.Map<IEnumerable<DocumentDto>>(documents);
            await _elasticSearchDocumentService.UpdateDocumentRangeAsync(ElasticConstant.ElasticDocuments,
                resultDocument, d => d.Id);
        }
    }
}