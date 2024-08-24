using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.CommentDocumentRepositories;
using LW.Data.Repositories.DocumentRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Data.Repositories.GradeRepositories;
using LW.Data.Repositories.LessonRepositories;
using LW.Data.Repositories.ProblemRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.SolutionRepositories;
using LW.Data.Repositories.TopicRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.Constant;
using LW.Shared.DTOs.Document;
using LW.Shared.DTOs.Exam;
using LW.Shared.DTOs.File;
using LW.Shared.DTOs.Lesson;
using LW.Shared.DTOs.Problem;
using LW.Shared.DTOs.Quiz;
using LW.Shared.DTOs.Solution;
using LW.Shared.DTOs.Topic;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.OpenApi.Extensions;
using MockQueryable.Moq;

namespace LW.Services.DocumentServices;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IGradeRepository _gradeRepository;
    private readonly ICommentDocumentRepository _commentDocumentRepository;
    private readonly IElasticSearchService<DocumentDto, int> _elasticSearchService;
    private readonly IElasticSearchService<TopicDto, int> _elasticSearchTopicService;
    private readonly IElasticSearchService<LessonDto, int> _elasticSearchLessonService;
    private readonly IElasticSearchService<ProblemDto, int> _elasticSearchProblemService;
    private readonly IElasticSearchService<QuizDto, int> _elasticSearchQuizService;
    private readonly IElasticSearchService<SolutionDto, int> _elasticSearchSolutionService;
    private readonly IMapper _mapper;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly ITopicRepository _topicRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IProblemRepository _problemRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly ISolutionRepository _solutionRepository;

    public DocumentService(IDocumentRepository documentRepository, IMapper mapper, IGradeRepository gradeRepository,
        IElasticSearchService<DocumentDto, int> elasticSearchService,
        ICommentDocumentRepository commentDocumentRepository, ITopicRepository topicRepository,
        IElasticSearchService<TopicDto, int> elasticSearchTopicService,
        IElasticSearchService<LessonDto, int> elasticSearchLessonService, ILessonRepository lessonRepository,
        IElasticSearchService<ProblemDto, int> elasticSearchProblemService,
        IElasticSearchService<QuizDto, int> elasticSearchQuizService,
        IElasticSearchService<SolutionDto, int> elasticSearchSolutionService, IProblemRepository problemRepository,
        IQuizRepository quizRepository, ISolutionRepository solutionRepository, ICloudinaryService cloudinaryService)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
        _gradeRepository = gradeRepository;
        _elasticSearchService = elasticSearchService;
        _commentDocumentRepository = commentDocumentRepository;
        _topicRepository = topicRepository;
        _elasticSearchTopicService = elasticSearchTopicService;
        _elasticSearchLessonService = elasticSearchLessonService;
        _lessonRepository = lessonRepository;
        _elasticSearchProblemService = elasticSearchProblemService;
        _elasticSearchQuizService = elasticSearchQuizService;
        _elasticSearchSolutionService = elasticSearchSolutionService;
        _problemRepository = problemRepository;
        _quizRepository = quizRepository;
        _solutionRepository = solutionRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResult<IEnumerable<DocumentDto>>> GetAllDocument(bool? status)
    {
        var documentList = await _documentRepository.GetAllDocument();
        if (!documentList.Any())
        {
            return new ApiResult<IEnumerable<DocumentDto>>(false, "Document is null !!!");
        }

        foreach (var item in documentList)
        {
            var averageRating = item.CommentDocuments.Any()
                ? Math.Round(item.CommentDocuments.Average(c => c.Rating), 2)
                : 0;
            item.AverageRating = averageRating;
            item.TotalReviewer = item.CommentDocuments.Count;
        }

        if (status != null)
        {
            documentList = documentList.Where(d => d.IsActive == status);
        }

        var result = _mapper.Map<IEnumerable<DocumentDto>>(documentList);
        return new ApiSuccessResult<IEnumerable<DocumentDto>>(result);
    }

    public async Task<ApiResult<IEnumerable<DocumentDto>>> GetAllDocumentByGrade(int id, bool? status)
    {
        var documentList = await _documentRepository.GetAllDocumentByGrade(id);
        if (!documentList.Any())
        {
            return new ApiResult<IEnumerable<DocumentDto>>(false, "Document is null !!!");
        }

        foreach (var item in documentList)
        {
            var averageRating = item.CommentDocuments.Any()
                ? Math.Round(item.CommentDocuments.Average(c => c.Rating), 2)
                : 0;
            item.AverageRating = averageRating;
            item.TotalReviewer = item.CommentDocuments.Count;
        }

        if (status != null)
        {
            documentList = documentList.Where(d => d.IsActive == status);
        }

        var result = _mapper.Map<IEnumerable<DocumentDto>>(documentList);
        return new ApiSuccessResult<IEnumerable<DocumentDto>>(result);
    }

    public async Task<ApiResult<PagedList<DocumentDto>>> GetAllDocumentPagination(SearchDocumentDto searchDocumentDto)
    {
        IEnumerable<DocumentDto> documentList;
        if (!string.IsNullOrEmpty(searchDocumentDto.Value))
        {
            var documentListSearch = await _elasticSearchService.SearchDocumentFieldAsync(
                ElasticConstant.ElasticDocuments,
                new SearchRequestValue
                {
                    Value = searchDocumentDto.Value,
                    Size = searchDocumentDto.Size,
                });
            if (documentListSearch is null)
            {
                return new ApiResult<PagedList<DocumentDto>>(false, "Document not found !!!");
            }

            documentList = documentListSearch.ToList();
        }
        else
        {
            var documentListAll = await _documentRepository.GetAllDocumentPagination();
            if (!documentListAll.Any())
            {
                return new ApiResult<PagedList<DocumentDto>>(false, "Document is null !!!");
            }

            documentList = _mapper.Map<IEnumerable<DocumentDto>>(documentListAll);
        }

        if (searchDocumentDto.Status != null)
        {
            documentList = documentList.Where(d => d.IsActive == searchDocumentDto.Status);
        }

        if (searchDocumentDto.GradeId > 0)
        {
            documentList = documentList.Where(d => d.GradeId == searchDocumentDto.GradeId);
        }

        foreach (var item in documentList)
        {
            var comment = await _commentDocumentRepository.GetAllCommentByDocumentId(item.Id);
            var averageRating = comment.Any() ? Math.Round(comment.Average(c => c.Rating), 2) : 0;
            item.AverageRating = averageRating;
            item.TotalReviewer = comment.Count();
        }

        var pagedResult = await PagedList<DocumentDto>.ToPageListAsync(documentList.AsQueryable().BuildMock(),
            searchDocumentDto.PageIndex, searchDocumentDto.PageSize, searchDocumentDto.OrderBy,
            searchDocumentDto.IsAscending);
        return new ApiSuccessResult<PagedList<DocumentDto>>(pagedResult);
    }

    public async Task<ApiResult<DocumentDto>> GetDocumentById(int id)
    {
        var documentEntity = await _documentRepository.GetDocumentById(id);
        if (documentEntity == null)
        {
            return new ApiResult<DocumentDto>(false, "Document is null !!!");
        }

        var gradeEntity = await _gradeRepository.GetGradeById(documentEntity.GradeId, false);
        if (gradeEntity != null)
        {
            documentEntity.Grade = gradeEntity;
        }

        var averageRating = documentEntity.CommentDocuments.Any()
            ? Math.Round(documentEntity.CommentDocuments.Average(c => c.Rating), 2)
            : 0;
        documentEntity.AverageRating = averageRating;
        documentEntity.TotalReviewer = documentEntity.CommentDocuments.Count;

        var result = _mapper.Map<DocumentDto>(documentEntity);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<DocumentDto>> CreateDocument(DocumentCreateDto documentCreateDto)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(documentCreateDto.GradeId, false);
        if (gradeEntity == null)
        {
            return new ApiResult<DocumentDto>(false, "GradeID not found !!!");
        }

        var imagePath = new FileImageDto();
        if (documentCreateDto.Image.Length > 0)
        {
            imagePath = await _cloudinaryService.CreateImageAsync(documentCreateDto.Image, CloudinaryConstant.FolderDocumentImage);
        }

        var documentEntity = _mapper.Map<Document>(documentCreateDto);
        documentEntity.Image = imagePath.Url;
        documentEntity.PublicId = imagePath.PublicId;
        // mã hóa code field
        documentEntity.Code = EncodeHelperExtensions.EncodeDocument(documentEntity.BookCollection.GetDisplayName().ToUpper(), documentEntity.TypeOfBook.GetDisplayName().ToUpper(), documentEntity.PublicationYear, documentEntity.Edition);
        documentEntity.KeyWord = (documentCreateDto.TagValues is not null) ? documentCreateDto.TagValues.ConvertToTagString() : documentCreateDto.Title.RemoveDiacritics();
        await _documentRepository.CreateDocument(documentEntity);
        await CreateOrUpdateElasticDocument(documentEntity.Id, true);
        var result = _mapper.Map<DocumentDto>(documentEntity);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<DocumentDto>> UpdateDocument(DocumentUpdateDto documentUpdateDto)
    {
        var gradeEntity = await _gradeRepository.GetGradeById(documentUpdateDto.GradeId, false);
        if (gradeEntity == null)
        {
            return new ApiResult<DocumentDto>(false, "GradeID not found !!!");
        }

        var documentEntity = await _documentRepository.GetDocumentById(documentUpdateDto.Id);
        if (documentEntity is null)
        {
            return new ApiResult<DocumentDto>(false, "Document not found !!!");
        }

        var imagePath = new FileImageDto();
        if (documentUpdateDto.Image != null && documentUpdateDto.Image.Length > 0)
        {
            imagePath = await _cloudinaryService.UpdateImageAsync(documentEntity.PublicId!, documentUpdateDto.Image);
        }
        else
        {
            imagePath.PublicId = documentEntity.PublicId;
            imagePath.Url = documentEntity.Image;
        }

        var model = _mapper.Map(documentUpdateDto, documentEntity);
        model.Image = imagePath.Url;
        model.PublicId = imagePath.PublicId;
        // encode code field
        model.Code = EncodeHelperExtensions.EncodeDocument(model.BookCollection.GetDisplayName().ToUpper(), model.TypeOfBook.GetDisplayName().ToUpper(), model.PublicationYear, model.Edition);
        model.KeyWord = (documentUpdateDto.TagValues is not null) ? documentUpdateDto.TagValues.ConvertToTagString() : documentUpdateDto.Title.RemoveDiacritics();
        var updateDocument = await _documentRepository.UpdateDocument(model);
        await CreateOrUpdateElasticDocument(documentUpdateDto.Id, false);
        var result = _mapper.Map<DocumentDto>(updateDocument);
        return new ApiSuccessResult<DocumentDto>(result);
    }

    public async Task<ApiResult<bool>> UpdateStatusDocument(int id)
    {
        var documentEntity = await _documentRepository.GetDocumentById(id);
        if (documentEntity is null)
        {
            return new ApiResult<bool>(false, "Document not found !!!");
        }

        var gradeEntity = await _gradeRepository.GetGradeById(documentEntity.GradeId, false);

        documentEntity.IsActive = !documentEntity.IsActive;
        await _documentRepository.UpdateDocument(documentEntity);
        await _documentRepository.SaveChangesAsync();
        documentEntity.Grade = gradeEntity;
        var result = _mapper.Map<DocumentDto>(documentEntity);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticDocuments, result, id);
        return new ApiSuccessResult<bool>(true, "Grade update successfully !!!");
    }

    public async Task<ApiResult<bool>> DeleteDocument(int id)
    {
        var documentEntity = await _documentRepository.GetDocumentById(id);
        if (documentEntity is null)
        {
            return new ApiResult<bool>(false, "Document not found !!!");
        }

        // xóa  topic - lesson
        var listTopic = await _topicRepository.GetAllTopicByDocumentAll(id);
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
                    await _elasticSearchLessonService.DeleteDocumentRangeAsync(ElasticConstant.ElasticLessons,
                        listLessonId);
                    foreach (var lesson in listLesson)
                    {
                        var listProblemInLesson = await _problemRepository.GetAllProblemByLesson(lesson.Id);
                        foreach (var problem in listProblemInLesson)
                        {
                            var listSolutionInLesson = await _solutionRepository.GetAllSolutionByProblemId(problem.Id);
                            if (listSolutionInLesson.Any())
                            {
                                var listSolutionInLessonId = listSolutionInLesson.Select(s => s.Id);
                                await _elasticSearchSolutionService.DeleteDocumentRangeAsync(
                                    ElasticConstant.ElasticSolutions, listSolutionInLessonId);
                            }
                        }

                        var listQuizInLesson = await _quizRepository.GetAllQuizByLessonId(lesson.Id);
                        if (listProblemInLesson.Any())
                        {
                            var listProblemInLessonId = listProblemInLesson.Select(p => p.Id);
                            await _elasticSearchProblemService.DeleteDocumentRangeAsync(ElasticConstant.ElasticProblems,
                                listProblemInLessonId);
                        }

                        if (listQuizInLesson.Any())
                        {
                            var listQuizInLessonId = listQuizInLesson.Select(q => q.Id);
                            await _elasticSearchQuizService.DeleteDocumentRangeAsync(ElasticConstant.ElasticQuizzes,
                                listQuizInLessonId);
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
                        await _elasticSearchSolutionService.DeleteDocumentRangeAsync(ElasticConstant.ElasticSolutions,
                            listSolutionInLessonId);
                    }
                }

                var listQuiz = await _quizRepository.GetAllQuizByTopicId(topic.Id);
                if (listQuiz.Any())
                {
                    var listQuizId = listQuiz.Select(q => q.Id);
                    await _elasticSearchQuizService.DeleteDocumentRangeAsync(ElasticConstant.ElasticQuizzes,
                        listQuizId);
                }

                if (listProblem.Any())
                {
                    var listProblemId = listProblem.Select(p => p.Id);
                    await _elasticSearchProblemService.DeleteDocumentRangeAsync(ElasticConstant.ElasticProblems,
                        listProblemId);
                }
            }
        }


        var document = await _documentRepository.DeleteDocument(id);
        if (!document)
        {
            return new ApiResult<bool>(false, "Failed Delete Document not found !!!");
        }

        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticDocuments, id);
        return new ApiSuccessResult<bool>(true, "Delete Document Successfully !!!");
    }

    private async Task CreateOrUpdateElasticDocument(int id, bool isCreateOrUpdate)
    {
        var document = await _documentRepository.GetDocumentById(id);
        var result = _mapper.Map<DocumentDto>(document);
        if (isCreateOrUpdate)
        {
            await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticDocuments, result, d => d.Id);
        }
        else
        {
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticDocuments, result, result.Id);
            var topics = await _topicRepository.GetAllTopicByDocument(id);
            var resultTopic = _mapper.Map<IEnumerable<TopicDto>>(topics);
            await _elasticSearchTopicService.UpdateDocumentRangeAsync(ElasticConstant.ElasticTopics, resultTopic,
                d => d.Id);
        }
    }
}