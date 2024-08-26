using Aspose.Pdf;
using AutoMapper;
using LW.Cache;
using LW.Cache.Interfaces;
using System.Collections;
using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.QuizAnswerRepositories;
using LW.Data.Repositories.QuizQuestionRelationRepositories;
using LW.Data.Repositories.QuizQuestionRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.DTOs.Email;
using LW.Shared.DTOs.Enum;
using LW.Shared.Constant;
using LW.Shared.DTOs.QuizAnswer;
using LW.Shared.DTOs.QuizQuestion;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using static Aspose.Pdf.CollectionItem;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using System.Xml.Linq;
using System.Data;
using Moq;

namespace LW.Services.QuizQuestionServices;

public class QuizQuestionService : IQuizQuestionService
{
    private readonly IQuizAnswerRepository _quizAnswerRepository;
    private readonly IQuizQuestionRepository _quizQuestionRepository;
    private readonly IQuizQuestionRelationRepository _quizQuestionRelationRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IElasticSearchService<QuizQuestionDto, int> _elasticSearchService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IMapper _mapper;
    private readonly IRedisCache<string> _redisCacheService;

    public QuizQuestionService(IQuizAnswerRepository quizAnswerRepository,
        IQuizQuestionRepository quizQuestionRepository, IRedisCache<string> redisCacheService, IMapper mapper,
        IQuizRepository quizRepository,
        IElasticSearchService<QuizQuestionDto, int> elasticSearchService, ICloudinaryService cloudinaryService,
        IQuizQuestionRelationRepository quizQuestionRelationRepository)
    {
        _quizAnswerRepository = quizAnswerRepository;
        _quizQuestionRepository = quizQuestionRepository;
        _mapper = mapper;
        _quizRepository = quizRepository;
        _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        _elasticSearchService = elasticSearchService;
        _cloudinaryService = cloudinaryService;
        _quizQuestionRelationRepository = quizQuestionRelationRepository;
    }

    public async Task<ApiResult<IEnumerable<QuizQuestionDto>>> GetAllQuizQuestion(bool? status)
    {
        var quizQuestionList = await _quizQuestionRepository.GetAllQuizQuestion();
        if (!quizQuestionList.Any())
        {
            return new ApiResult<IEnumerable<QuizQuestionDto>>(false, "Quiz Question is null !!!");
        }

        if (status != null)
        {
            quizQuestionList = quizQuestionList.Where(q => q.IsActive == status);
        }

        var result = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionList);
        return new ApiSuccessResult<IEnumerable<QuizQuestionDto>>(result);
    }

    public async Task<ApiResult<PagedList<QuizQuestionDto>>> GetAllQuizQuestionPagination(
        SearchAllQuizQuestionDto searchAllQuizQuestionDto)
    {
        IEnumerable<QuizQuestionDto> quizQuestionList;
        if (!string.IsNullOrEmpty(searchAllQuizQuestionDto.Value))
        {
            var quizQuestionListSearch = await _elasticSearchService.SearchDocumentFieldAsync(
                ElasticConstant.ElasticQuizQuestion, new SearchRequestValue
                {
                    Value = searchAllQuizQuestionDto.Value,
                    Size = searchAllQuizQuestionDto.Size,
                });
            if (quizQuestionListSearch is null)
            {
                return new ApiResult<PagedList<QuizQuestionDto>>(false, "Quiz Question not found !!!");
            }

            quizQuestionList = quizQuestionListSearch.ToList();
        }
        else
        {
            var quizQuestionListAll = await _quizQuestionRepository.GetAllQuizQuestionPagination();
            if (!quizQuestionListAll.Any())
            {
                return new ApiResult<PagedList<QuizQuestionDto>>(false, "Quiz Question is null !!!");
            }

            quizQuestionList = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionListAll);
        }

        if (searchAllQuizQuestionDto.Status != null)
        {
            quizQuestionList = quizQuestionList.Where(q => q.IsActive == searchAllQuizQuestionDto.Status);
        }

        if (searchAllQuizQuestionDto.QuizId > 0)
        {
            quizQuestionList = quizQuestionList.Where(q =>
                q.QuizQuestionRelations.Any(q => q.QuizId == searchAllQuizQuestionDto.QuizId));
        }

        if (searchAllQuizQuestionDto.Level > 0)
        {
            quizQuestionList = quizQuestionList.Where(q => q.QuestionLevel == searchAllQuizQuestionDto.Level);
        }

        var result = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionList);
        var pagedResult = await PagedList<QuizQuestionDto>.ToPageListAsync(result.AsQueryable().BuildMock(),
            searchAllQuizQuestionDto.PageIndex, searchAllQuizQuestionDto.PageSize, searchAllQuizQuestionDto.OrderBy,
            searchAllQuizQuestionDto.IsAscending);
        return new ApiSuccessResult<PagedList<QuizQuestionDto>>(pagedResult);
    }

    public async Task<ApiResult<IEnumerable<object>>> GetAllQuizQuestionByQuizId(
        SearchQuizQuestionDto searchQuizQuestionDto)
    {
        var quiz = await _quizRepository.GetQuizById(Convert.ToInt32(searchQuizQuestionDto.QuizId));
        if (quiz is null)
        {
            return new ApiResult<IEnumerable<object>>(false, "Quiz is null !!!");
        }

        IEnumerable<QuizQuestionDto> quizQuestionList;
        if (!string.IsNullOrEmpty(searchQuizQuestionDto.Value))
        {
            var quizQuestionListSearch = await _elasticSearchService.SearchDocumentFieldAsync(
                ElasticConstant.ElasticQuizQuestion, new SearchRequestValue
                {
                    Value = searchQuizQuestionDto.Value,
                    Size = searchQuizQuestionDto.Size,
                });
            if (quizQuestionListSearch is null)
            {
                return new ApiResult<IEnumerable<object>>(false, "Quiz Question not found !!!");
            }

            quizQuestionList = quizQuestionListSearch.ToList();
        }
        else
        {
            var quizQuestionListAll =
                await _quizQuestionRepository.GetAllQuizQuestionByQuizId(Convert.ToInt32(searchQuizQuestionDto.QuizId));
            if (!quizQuestionListAll.Any())
            {
                return new ApiResult<IEnumerable<object>>(false, "Quiz Question is null !!!");
            }

            quizQuestionList = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionListAll);
        }

        if (searchQuizQuestionDto.Status != null)
        {
            quizQuestionList = quizQuestionList.Where(q => q.IsActive == searchQuizQuestionDto.Status);
        }

        if (quiz.IsShuffle)
        {
            quizQuestionList = quizQuestionList.OrderBy(x => Random.Shared.Next());
        }

        foreach (var item in quizQuestionList)
        {
            if (item.IsShuffle)
            {
                item.QuizAnswers = item.QuizAnswers.OrderBy(x => Random.Shared.Next()).ToList();
            }
        }

        if (searchQuizQuestionDto.NumberOfQuestion > 0)
        {
            quizQuestionList = quizQuestionList.Take(searchQuizQuestionDto.NumberOfQuestion);
        }

        if (quiz.Type == ETypeQuiz.Test)
        {
            var resultTest = _mapper.Map<IEnumerable<QuizQuestionTestDto>>(quizQuestionList);
            return new ApiSuccessResult<IEnumerable<object>>(resultTest);
        }

        var result = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionList);
        return new ApiSuccessResult<IEnumerable<object>>(result);
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
            case ETypeQuestion.QuestionMultiChoice:
                if (countAnswer != 6 || countAnswerTrue < 1)
                {
                    return new ApiResult<QuizQuestionDto>(false,
                        "Question is six answer and more than 1 correct answer !!!");
                }

                break;
        }

        var quizQuestionEntity = _mapper.Map<QuizQuestion>(quizQuestionCreateDto);

        var listQuizQuestion = (quizQuestionCreateDto.QuizId > 0) ? await _quizQuestionRepository.GetAllQuizQuestionByQuizId(quizQuestionCreateDto.QuizId) : await _quizQuestionRepository.GetAllQuizQuestion();
        var numberHash = FindDuplicateQuestion(listQuizQuestion, quizQuestionEntity);
        if (numberHash == 0)
        {
            return new ApiResult<QuizQuestionDto>(false, "Question is duplicate !!!");
        }

        if (quizQuestionCreateDto.QuizId > 0)
        {
            quizQuestionEntity.QuizQuestionRelations = new List<QuizQuestionRelation>
            {
                new() { QuizId = quizQuestionCreateDto.QuizId }
            };
        }

        quizQuestionEntity.KeyWord = quizQuestionCreateDto.Content.RemoveDiacritics();
        quizQuestionEntity.HashQuestion = numberHash;
        if (quizQuestionCreateDto.Image != null && quizQuestionCreateDto.Image.Length > 0)
        {
            var filePath = await _cloudinaryService.CreateImageAsync(quizQuestionCreateDto.Image, CloudinaryConstant.FolderQuestionImage);
            quizQuestionEntity.Image = filePath.Url;
            quizQuestionEntity.PublicId = filePath.PublicId;
        }

        var quizQuestionCreate = await _quizQuestionRepository.CreateQuizQuestion(quizQuestionEntity);
        var result = _mapper.Map<QuizQuestionDto>(quizQuestionCreate);
        await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticQuizQuestion, result, q => q.Id);
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
            case ETypeQuestion.QuestionMultiChoice:
                if (countAnswer < 6 || countAnswerTrue < 1)
                {
                    return new ApiResult<QuizQuestionDto>(false,
                        "Question is six answer and more than 1 correct answer !!!");
                }

                break;
        }

        var modelQuestion = _mapper.Map(quizQuestionUpdateDto, quizQuestionEntity);

        var listQuizQuestion = await _quizQuestionRepository.GetAllQuizQuestion();
        var numberHash = FindDuplicateQuestion(listQuizQuestion.Where(q => q.Id != quizQuestionUpdateDto.Id), quizQuestionEntity);
        if (numberHash == 0)
        {
            return new ApiResult<QuizQuestionDto>(false, "Question is duplicate !!!");
        }

        var quizQuestion = await _quizQuestionRepository.GetQuizQuestionById(quizQuestionUpdateDto.Id);
        if (quizQuestionUpdateDto.Image != null && quizQuestionUpdateDto.Image.Length > 0)
        {
            var filePath = await _cloudinaryService.UpdateImageAsync(quizQuestionEntity.PublicId, quizQuestionUpdateDto.Image);
            quizQuestionEntity.Image = filePath.Url;
            quizQuestionEntity.PublicId = filePath.PublicId;
        }

        modelQuestion.HashQuestion = numberHash;
        modelQuestion.QuizQuestionRelations = quizQuestion!.QuizQuestionRelations;
        modelQuestion.KeyWord = quizQuestionUpdateDto.Content.RemoveDiacritics();
        modelQuestion.Image = quizQuestion.Image;
        modelQuestion.PublicId = quizQuestion.PublicId;
        if (quizQuestion.Type.Equals(quizQuestionUpdateDto.Type) && countAnswer == quizQuestion.QuizAnswers.Count)
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
                foreach (var answer in quizAnswer)
                {
                    answer.QuizQuestionId = quizQuestionUpdateDto.Id;
                }

                await _quizAnswerRepository.CreateRangeQuizAnswer(quizAnswer);
            }
        }

        var quizQuestionUpdate = await _quizQuestionRepository.UpdateQuizQuestion(modelQuestion);
        quizQuestionUpdate = await _quizQuestionRepository.GetQuizQuestionById(quizQuestionUpdateDto.Id);
        var result = _mapper.Map<QuizQuestionDto>(quizQuestionUpdate);
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticQuizQuestion, result, quizQuestionUpdateDto.Id);
        return new ApiSuccessResult<QuizQuestionDto>(result);
    }

    public async Task<ApiResult<bool>> CreateRangeQuizQuestion(IEnumerable<QuizQuestionCreateDto> quizQuestionsCreateDto)
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
                case ETypeQuestion.QuestionMultiChoice:
                    if (countAnswer != 6 || countAnswerTrue < 1)
                    {
                        return new ApiResult<bool>(false,
                            "Question is six answer and more than 1 correct answer !!!");
                    }

                    break;
            }

            var quizQuestionEntity = _mapper.Map<QuizQuestion>(item);

            var listQuizQuestion = (item.QuizId > 0) ? await _quizQuestionRepository.GetAllQuizQuestionByQuizId(item.QuizId) : await _quizQuestionRepository.GetAllQuizQuestion();
            var numberHash = FindDuplicateQuestion(listQuizQuestion, quizQuestionEntity);
            if (numberHash == 0)
            {
                return new ApiResult<bool>(false, "Question is duplicate !!!");
            }

            if (item.QuizId > 0)
            {
                quizQuestionEntity.QuizQuestionRelations = new List<QuizQuestionRelation>
                {
                    new() { QuizId = item.QuizId }
                };
            }

            quizQuestionEntity.KeyWord = item.Content.RemoveDiacritics();
            quizQuestionEntity.HashQuestion = numberHash;
            if (item.Image != null && item.Image.Length > 0)
            {
                var filePath = await _cloudinaryService.CreateImageAsync(item.Image, CloudinaryConstant.FolderQuestionImage);
                quizQuestionEntity.Image = filePath.Url;
                quizQuestionEntity.PublicId = filePath.PublicId;
            }

            var quizQuestionCreate = await _quizQuestionRepository.CreateQuizQuestion(quizQuestionEntity);
            var result = _mapper.Map<QuizQuestionDto>(quizQuestionCreate);
            await _elasticSearchService.CreateDocumentAsync(ElasticConstant.ElasticQuizQuestion, result, q => q.Id);
        }

        return new ApiSuccessResult<bool>(true);
    }

    public async Task<ApiResult<bool>> UpdateRangeQuizQuestion(IEnumerable<QuizQuestionUpdateDto> quizQuestionsUpdateDto)
    {
        foreach (var itemUpdate in quizQuestionsUpdateDto)
        {
            var quizQuestionEntity = await _quizQuestionRepository.GetQuizQuestionById(itemUpdate.Id);
            if (quizQuestionEntity is null)
            {
                return new ApiResult<bool>(false, "Quiz Question not found !!!");
            }

            var countAnswer = itemUpdate.QuizAnswers.Count();
            var countAnswerTrue = itemUpdate.QuizAnswers.Count(x => x.IsCorrect);
            switch (itemUpdate.Type)
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
                case ETypeQuestion.QuestionMultiChoice:
                    if (countAnswer < 6 || countAnswerTrue < 1)
                    {
                        return new ApiResult<bool>(false, "Question is six answer and more than 1 correct answer !!!");
                    }

                    break;
            }

            var modelQuestion = _mapper.Map(itemUpdate, quizQuestionEntity);

            var listQuizQuestion = await _quizQuestionRepository.GetAllQuizQuestion();
            var numberHash = FindDuplicateQuestion(listQuizQuestion.Where(q => q.Id != itemUpdate.Id), quizQuestionEntity);
            if (numberHash == 0)
            {
                return new ApiResult<bool>(false, "Question is duplicate !!!");
            }

            var quizQuestion = await _quizQuestionRepository.GetQuizQuestionById(itemUpdate.Id);
            if (itemUpdate.Image != null && itemUpdate.Image.Length > 0)
            {
                var filePath = await _cloudinaryService.UpdateImageAsync(quizQuestionEntity.PublicId, itemUpdate.Image);
                quizQuestionEntity.Image = filePath.Url;
                quizQuestionEntity.PublicId = filePath.PublicId;
            }

            modelQuestion.HashQuestion = numberHash;
            modelQuestion.QuizQuestionRelations = quizQuestion!.QuizQuestionRelations;
            modelQuestion.KeyWord = itemUpdate.Content.RemoveDiacritics();
            modelQuestion.Image = quizQuestion.Image;
            modelQuestion.PublicId = quizQuestion.PublicId;
            if (quizQuestion.Type.Equals(itemUpdate.Type) && countAnswer == quizQuestion.QuizAnswers.Count)
            {
                foreach (var item in itemUpdate.QuizAnswers)
                {
                    var quizAnswer = await _quizAnswerRepository.GetQuizAnswerById(item.Id);
                    if (quizAnswer is null)
                    {
                        return new ApiResult<bool>(false, "Quiz answer not found !!!");
                    }

                    var modelAnswer = _mapper.Map(item, quizAnswer);
                    await _quizAnswerRepository.UpdateQuizAnswer(modelAnswer);
                }
            }
            else
            {
                var quizAnswers = await _quizAnswerRepository.GetAllQuizAnswerByQuizQuestionId(itemUpdate.Id);
                if (quizAnswers.Any())
                {
                    await _quizAnswerRepository.DeleteRangeAnswer(quizAnswers);
                    var quizAnswer = _mapper.Map<IEnumerable<QuizAnswer>>(itemUpdate.QuizAnswers);
                    foreach (var answer in quizAnswer)
                    {
                        answer.QuizQuestionId = itemUpdate.Id;
                    }

                    await _quizAnswerRepository.CreateRangeQuizAnswer(quizAnswer);
                }
            }

            var quizQuestionUpdate = await _quizQuestionRepository.UpdateQuizQuestion(modelQuestion);
            quizQuestionUpdate = await _quizQuestionRepository.GetQuizQuestionById(itemUpdate.Id);
            var result = _mapper.Map<QuizQuestionDto>(quizQuestionUpdate);
            await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticQuizQuestion, result, itemUpdate.Id);
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
        await _elasticSearchService.UpdateDocumentAsync(ElasticConstant.ElasticQuizQuestion, result, id);
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

        await _elasticSearchService.DeleteDocumentAsync(ElasticConstant.ElasticQuizQuestion, id);
        return new ApiSuccessResult<bool>(true);
    }


    private static int FindDuplicateQuestion(IEnumerable<QuizQuestion> dataArray, QuizQuestion quizQuestion)
    {
        var answerHash = (quizQuestion.QuizAnswers.Select(x => x.Content).ToArray()).HashArray();
        foreach (var item in dataArray)
        {
            if (item.Content.Trim().Equals(quizQuestion.Content.Trim()) && item.HashQuestion == answerHash)
            {
                return 0;
            }
        }

        return answerHash;
    }


    public async Task<byte[]> ExportExcel(int checkData = 1, string? Ids = null)
    {
        IEnumerable<QuizQuestionExcelDto> data = new List<QuizQuestionExcelDto>();
        //kiểm tra xem một ICollection
        if (Ids != null)
        {
            var dataImport = await _redisCacheService.GetStringKey(Ids);

            var quizQuestions = JsonConvert.DeserializeObject<List<QuizQuestionImportDto>>(dataImport);
            data = quizQuestions.Select(e => _mapper.Map<QuizQuestionExcelDto>(e)).ToList();
        }

        if (data.Any())
        {
            return await GenerateExcelFile(data, Ids);
        }

        return await GenerateExcelFile(data, Ids);
    }


    private async Task<byte[]> GenerateExcelFile(IEnumerable<QuizQuestionExcelDto> data, string Ids)
    {
        ETypeQuestion[] typeQuestions = (ETypeQuestion[])Enum.GetValues(typeof(ETypeQuestion));
        EQuestionLevel[] levels = (EQuestionLevel[])Enum.GetValues(typeof(EQuestionLevel));
        var arrayShuffle = new[]
        {
            new { label = "Có", value = true },
            new { label = "Không", value = false }
        };
        using (var package = new ExcelPackage())
        {
            // Add a worksheet named "Data Validation"
            var worksheet = CreateWorkSheet(package, Ids);
            // Define column headers and widths
            string[] columnHeaders =
            {
                "STT",
                "Loại câu hỏi",
                "Câu hỏi - max 200 ký tự ",
                "Đáp án 1 - max 200 ký tự",
                "Đáp án 2 - max 200 ký tự",
                "Đáp án 3 - max 200 ký tự",
                "Đáp án 4 - max 200 ký tự",
                "Đáp án 5 - max 200 ký tự",
                "Đáp án 6 - max 200 ký tự", // thay đổi cột import 
                "Đáp án đúng - Chọn một câu trả lời đúng theo số (1;2;3;4;5;6) ",
                "Mức độ câu hỏi",
                "Trộn câu hỏi",
                "Gợi ý câu trả lời",
            };
            if (Ids != null)
            {
                string[] extendedColumnHeaders = new string[columnHeaders.Length + 1];
                for (int i = 0; i < columnHeaders.Length; i++)
                {
                    extendedColumnHeaders[i] = columnHeaders[i];
                }

                // Add the new column header
                extendedColumnHeaders[columnHeaders.Length] = "Tình trạng";
                // Replace the old array with the new one
                columnHeaders = extendedColumnHeaders;
            }

            int dataStartRow = 3;
            int endRow = 1000;
            ; // Calculate end row dynamically
            StyleColumn(columnHeaders, worksheet, dataStartRow, endRow, Ids);
            int startRow = dataStartRow + 1; // Assuming data starts from row (dataStartRow + 1)
            AddDataValidation(worksheet, columnHeaders, startRow, endRow, typeQuestions, levels, arrayShuffle);
            // Convert data to DataTable and populate the worksheet
            ToConvertDataTable(data, worksheet);
            // Save the workbook to a memory stream and return the stream as a byte array
            return SavePackageToStream(package);
        }
    }

    public DataTable ToConvertDataTable<T>(IEnumerable<T> items, ExcelWorksheet worksheet)
    {
        DataTable dt = CreateDataTableStructure<T>();
        PropertyInfo[] propInfo = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        int ordinalNumber = 1;
        int rowIndex = 4;

        foreach (T item in items)
        {
            DataRow row = dt.NewRow();
            row["OrdinalNumber"] = ordinalNumber;
            worksheet.Cells[rowIndex, 1].Value = ordinalNumber;

            int excelColumnIndex = 2;

            // Set properties separately
            SetPropertyValueInWorksheet(item, "TypeName", worksheet, rowIndex, ref excelColumnIndex);
            SetPropertyValueInWorksheet(item, "Content", worksheet, rowIndex, ref excelColumnIndex);

            string correctAnswer = SetQuizAnswersInWorksheet(item, worksheet, rowIndex, ref excelColumnIndex);

            SetRemainingPropertiesInWorksheet(item, propInfo, worksheet, rowIndex, ref excelColumnIndex, correctAnswer);

            dt.Rows.Add(row);
            rowIndex++;
            ordinalNumber++;
        }

        return dt;
    }

    private DataTable CreateDataTableStructure<T>()
    {
        DataTable dt = new DataTable(typeof(T).Name);
        PropertyInfo[] propInfo = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        dt.Columns.Add("OrdinalNumber", typeof(int));
        AddColumnIfPropertyExists(dt, propInfo, "TypeName");
        AddColumnIfPropertyExists(dt, propInfo, "Content");

        for (int i = 1; i <= 6; i++)
        {
            dt.Columns.Add($"QuizAnswers_Content{i}");
        }

        foreach (PropertyInfo prop in propInfo)
        {
            if (prop.Name != "QuizAnswers" && prop.Name != "TypeName" && prop.Name != "Content")
            {
                dt.Columns.Add(prop.Name);
            }
        }

        return dt;
    }

    private void AddColumnIfPropertyExists(DataTable dt, PropertyInfo[] propInfo, string propertyName)
    {
        if (propInfo.Any(p => p.Name == propertyName))
        {
            dt.Columns.Add(propertyName);
        }
    }

    private void SetPropertyValueInWorksheet<T>(T item, string propertyName, ExcelWorksheet worksheet, int rowIndex,
        ref int excelColumnIndex)
    {
        var prop = item.GetType().GetProperty(propertyName);
        if (prop != null)
        {
            var value = prop.GetValue(item, null);
            string cellValue = value != null ? value.ToString() : "";
            worksheet.Cells[rowIndex, excelColumnIndex].Value = cellValue;
            excelColumnIndex++;
        }
    }

    private string SetQuizAnswersInWorksheet<T>(T item, ExcelWorksheet worksheet, int rowIndex,
        ref int excelColumnIndex)
    {
        var quizAnswers = item.GetType().GetProperty("QuizAnswers").GetValue(item) as IEnumerable<QuizAnswerDto>;
        string correctAnswer = "";
        if (quizAnswers != null)
        {
            int answerIndex = 1;
            foreach (var answer in quizAnswers)
            {
                worksheet.Cells[rowIndex, excelColumnIndex].Value = answer.Content;
                if (answer.IsCorrect)
                {
                    correctAnswer += $"{answerIndex};";
                }

                excelColumnIndex++;
                answerIndex++;
            }
        }

        if (correctAnswer.EndsWith(";"))
        {
            correctAnswer = correctAnswer.TrimEnd(';');
        }

        return correctAnswer;
    }

    private void SetErrorQuizInWorksheet<T>(T item, ExcelWorksheet worksheet, int rowIndex, ref int excelColumnIndex)
    {
        var errors = item.GetType().GetProperty("Errors").GetValue(item) as List<string>;
        if (errors != null && errors.Any())
        {
            string combinedErrors = string.Join("\n", errors.Select(e => $" + {e}"));
            worksheet.Cells[rowIndex, excelColumnIndex].Value = combinedErrors;
            worksheet.Cells[rowIndex, excelColumnIndex].Style.WrapText = true; // Ensure text wraps within the cell
            excelColumnIndex++;
        }
        else
        {
            worksheet.Cells[rowIndex, excelColumnIndex].Value = " + Hợp Lệ";
        }
    }

    private void SetRemainingPropertiesInWorksheet<T>(T item, PropertyInfo[] propInfo, ExcelWorksheet worksheet,
        int rowIndex, ref int excelColumnIndex, string correctAnswer)
    {
        excelColumnIndex = 10;
        foreach (PropertyInfo prop in propInfo)
        {
            if (prop.Name != "QuizAnswers" && prop.Name != "TypeName" && prop.Name != "Content")
            {
                if (prop.Name == "Correct")
                {
                    string cellValue = !string.IsNullOrEmpty(correctAnswer) ? correctAnswer : "";
                    worksheet.Cells[rowIndex, excelColumnIndex].Value = cellValue;
                    excelColumnIndex++;
                }
                else if (prop.Name == "Errors")
                {
                    SetErrorQuizInWorksheet(item, worksheet, rowIndex, ref excelColumnIndex);
                }
                else
                {
                    var propValue = prop.GetValue(item, null);
                    string cellValue = propValue is bool boolValue
                        ? (boolValue ? "Có" : "Không")
                        : propValue?.ToString() ?? "";
                    worksheet.Cells[rowIndex, excelColumnIndex].Value = cellValue;
                    excelColumnIndex++;
                }
            }
        }
    }


    // create worksheet and merge cell 
    public ExcelWorksheet CreateWorkSheet(ExcelPackage package, string Ids)
    {
        // Add a worksheet named "Data Validation"
        var worksheet = package.Workbook.Worksheets.Add("Danh sách câu hỏi");
        // Merge cells for title and set title formatting

        if (Ids != null)
        {
            worksheet.Cells["A1:N2"].Merge = true;
        }
        else
        {
            worksheet.Cells["A1:M2"].Merge = true;
        }

        worksheet.Cells["A1"].Value = "Danh Sách Câu Hỏi"; // Replace with your actual title
        worksheet.Row(1).Height = 25;
        worksheet.Row(1).Style.Font.Bold = true;
        worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Row(1).Style.Font.Size = 25;
        return worksheet;
    }

    // style for column 
    public void StyleColumn(string[] columnHeaders, ExcelWorksheet worksheet, int dataStartRow, int endrow, string? Ids)
    {
        int[] columnWidths = { 7, 20, 30, 30, 30, 30, 30, 30, 50, 30, 30, 30, 30 };
        if (Ids != null)
        {
            int[] extendedColumnWidths = new int[columnWidths.Length + 1];
            for (int i = 0; i < columnWidths.Length; i++)
            {
                extendedColumnWidths[i] = columnWidths[i];
            }

            // Add the new column header
            extendedColumnWidths[columnWidths.Length] = 50;
            // Replace the old array with the new one
            columnWidths = extendedColumnWidths;
        }

        // Add column headers and set column widths
        for (int i = 0; i < columnHeaders.Length; i++)
        {
            worksheet.Cells[dataStartRow, i + 1].Value = columnHeaders[i];
            worksheet.Cells[dataStartRow, i + 1].Style.WrapText = true;
            worksheet.Column(i + 1).Width = columnWidths[i];
            worksheet.Row(dataStartRow).Style.Font.Size = 12; // Set font size for headers row
            worksheet.Row(dataStartRow).Style.Font.Bold = true;
            // Apply border style to the entire column
            using (var range = worksheet.Cells[dataStartRow, i + 1, endrow, i + 1])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }
        }
    }

    // create combobox 
    private void AddDataValidation(ExcelWorksheet worksheet, string[] columnHeaders, int startRow, int endRow,
        IEnumerable<ETypeQuestion> comboBoxValues, IEnumerable<EQuestionLevel> levels, IEnumerable<Object> shuffles)
    {
        int departmentColumnIndex = Array.IndexOf(columnHeaders, "Loại câu hỏi");
        if (departmentColumnIndex == -1)
        {
            throw new ArgumentException("Column 'Loại câu hỏi' not found in columnHeaders.");
        }

        var validationRange = worksheet.Cells[startRow, departmentColumnIndex + 1, endRow, departmentColumnIndex + 1];

        var validation = validationRange.DataValidation.AddListDataValidation();

        foreach (var value in comboBoxValues)
        {
            validation.Formula.Values.Add(value.GetDisplayNameEnum());
        }

        int levelsColumnIndex = Array.IndexOf(columnHeaders, "Mức độ câu hỏi");
        var validationRangelevels = worksheet.Cells[startRow, levelsColumnIndex + 1, endRow, levelsColumnIndex + 1];

        var validationLevels = validationRangelevels.DataValidation.AddListDataValidation();

        foreach (var value in levels)
        {
            validationLevels.Formula.Values.Add(value.GetDisplayNameEnum());
        }

        int shuffleColumnIndex = Array.IndexOf(columnHeaders, "Trộn câu hỏi");
        var validationRangeShuffle = worksheet.Cells[startRow, shuffleColumnIndex + 1, endRow, shuffleColumnIndex + 1];

        var validationShuffle = validationRangeShuffle.DataValidation.AddListDataValidation();

        foreach (var item in shuffles)
        {
            var label = item.GetType().GetProperty("label").GetValue(item).ToString();
            validationShuffle.Formula.Values.Add(label);
        }
    }

    // save package by stream 
    private byte[] SavePackageToStream(ExcelPackage package)
    {
        using (var stream = new MemoryStream())
        {
            package.SaveAs(stream);
            return stream.ToArray();
        }
    }

    public ApiResult<QuizQuestionImportParentDto> CheckFileImport(IFormFile fileImport)
    {
        if (fileImport == null || fileImport.Length == 0)
        {
            return new ApiResult<QuizQuestionImportParentDto>(false, "File Import isn't empty");
        }

        if (!Path.GetExtension(fileImport.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return new ApiResult<QuizQuestionImportParentDto>(false, "File upload must be format.xlxs");
        }

        // If the file is valid
        return new ApiResult<QuizQuestionImportParentDto>(true, "File valid");
    }

    // support error in list
    private void AddImportError(QuizQuestionImportDto dto, string error)
    {
        dto.Errors.Add(error);
        dto.IsImported = false;
    }


    public async Task<ApiResult<QuizQuestionImportParentDto>> ImportExcel(IFormFile fileImport, int quizId)
    {
        var quizQuestionImportParentDto = new QuizQuestionImportParentDto();
        var isExcel = CheckFileImport(fileImport);
        if (!isExcel.IsSucceeded)
        {
            return isExcel;
        }

        var quizQuestionImportDtos = new List<QuizQuestionImportDto>();
        var quizQuestionImportSuccess = new List<QuizQuestion>();
        var quizQuestionImportFail = new List<QuizQuestionImportDto>();
        var listQuizQuestion = await _quizQuestionRepository.GetAllQuizQuestionByQuizId(quizId);
        var processHashNum = new HashSet<(int hashNum, string title)>();
        using (var stream = new MemoryStream())
        {
            await fileImport.CopyToAsync(stream);

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                if (workSheet != null)
                {
                    ProcessWorksheet(workSheet, quizQuestionImportDtos, quizQuestionImportSuccess,
                        quizQuestionImportFail, out int countSuccess, out int countFail, listQuizQuestion,
                        processHashNum);

                    quizQuestionImportParentDto.CountSuccess = countSuccess;
                    quizQuestionImportParentDto.CountFail = countFail;
                    quizQuestionImportParentDto.QuizQuestionImportDtos = quizQuestionImportDtos;
                    var (cacheKeySuccess, cacheKeyFail, cacheKeyResult) =
                        await CacheQuizQuestions(quizQuestionImportSuccess, quizQuestionImportFail,
                            quizQuestionImportDtos);
                    quizQuestionImportParentDto.IdImport = cacheKeySuccess;
                    quizQuestionImportParentDto.IdImportFail = cacheKeyFail;
                    quizQuestionImportParentDto.IdImportResult = cacheKeyResult;
                }
            }
        }

        return new ApiResult<QuizQuestionImportParentDto>(true, quizQuestionImportParentDto);
    }

    private static int FindLastRowWithData(ExcelWorksheet worksheet, int headerRow)
    {
        int lastRow = headerRow;
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            for (int row = headerRow + 1; row <= worksheet.Dimension.End.Row; row++)
            {
                if (worksheet.Cells[row, col].Value != null && worksheet.Cells[row, col].Value.ToString().Trim() != "")
                {
                    lastRow = Math.Max(lastRow, row);
                }
            }
        }

        return lastRow;
    }

    private void ProcessWorksheet(ExcelWorksheet workSheet, List<QuizQuestionImportDto> quizQuestionImportDtos,
        List<QuizQuestion> quizQuestionImportSuccess, List<QuizQuestionImportDto> quizQuestionImportFail,
        out int countSuccess, out int countFail, IEnumerable<QuizQuestion> listQuizQuestion,
        HashSet<(int hashNum, string title)> processHashNum)
    {
        countSuccess = 0;
        countFail = 0;
        int headerRow = 3;
        int lastRowWithData = FindLastRowWithData(workSheet, headerRow);

        for (int row = 4; row <= lastRowWithData; row++)
        {
            var quizQuestionImportDto = CreateQuizQuestionImportDto(workSheet, row);
            var quizQuestion = _mapper.Map<QuizQuestion>(quizQuestionImportDto);
            quizQuestion.HashQuestion = FindDuplicateQuestion(listQuizQuestion, quizQuestion);
            quizQuestionImportDto.HashQuestion = quizQuestion.HashQuestion;

            if (ValidateQuizQuestionImportDto(quizQuestionImportDto, processHashNum))

            {
                quizQuestionImportDto.IsImported = true;
                quizQuestionImportSuccess.Add(quizQuestion);
                countSuccess++;
            }
            else
            {
                countFail++;
                quizQuestionImportFail.Add(quizQuestionImportDto);
            }

            processHashNum.Add((quizQuestion.HashQuestion, quizQuestion.Content));
            quizQuestionImportDtos.Add(quizQuestionImportDto);
        }
    }

    private QuizQuestionImportDto CreateQuizQuestionImportDto(ExcelWorksheet workSheet, int row)
    {
        var arrayShuffle = new[]
        {
            new { label = "Có", value = true },
            new { label = "Không", value = false }
        };
        var typeQuestionName = workSheet.Cells[row, 2].Value?.ToString()?.Trim();
        var answers = CreateArrayAnswer(workSheet, row);
        var answerCorrect = workSheet.Cells[row, 10].Value?.ToString()?.Trim();
        var level = workSheet.Cells[row, 11].Value?.ToString()?.Trim();
        var isShuffle = workSheet.Cells[row, 12].Value?.ToString()?.Trim();
        var hint = workSheet.Cells[row, 13].Value?.ToString()?.Trim();
        int result = EnumHelperExtensions.GetEnumIntValueFromDisplayName<ETypeQuestion>(typeQuestionName);
        int resultLevel = EnumHelperExtensions.GetEnumIntValueFromDisplayName<EQuestionLevel>(level);
        string[] correctAnswersArray = answerCorrect?.Split(';', ',') ?? Array.Empty<string>();
        List<QuizAnswerDto> quizAnswers = CreateQuizAnswer(answers, correctAnswersArray);
        var shuffle = GetShuffleValue(arrayShuffle, isShuffle);

        return new QuizQuestionImportDto
        {
            Content = workSheet.Cells[row, 3].Value?.ToString()?.Trim(),
            QuestionLevel = (EQuestionLevel)resultLevel,
            QuestionLevelName = level,
            QuizAnswers = quizAnswers,
            Type = (ETypeQuestion)result,
            TypeName = typeQuestionName,
            IsActive = true,
            IsShuffle = shuffle,
            Hint = hint,
            KeyWord = workSheet.Cells[row, 3].Value?.ToString()?.Trim().RemoveDiacritics(),
        };
    }

    private List<QuizAnswerDto> CreateQuizAnswer(List<string> answers, string[] correctAnswersArray)
    {
        List<QuizAnswerDto> quizAnswers = new List<QuizAnswerDto> { };
        for (int i = 0; i < answers.Count; i++)
        {
            if (!string.IsNullOrEmpty(answers[i]))
            {
                quizAnswers.Add(new QuizAnswerDto(correctAnswersArray.Contains((i + 1).ToString()), answers[i]));
            }
        }

        return quizAnswers;
    }

    private List<string> CreateArrayAnswer(ExcelWorksheet workSheet, int row)
    {
        var answers = new List<string>();

        for (int i = 4; i <= 9; i++)
        {
            answers.Add(workSheet.Cells[row, i].Value?.ToString()?.Trim());
        }

        return answers;
    }

    private bool GetShuffleValue(dynamic[] arrayShuffle, string isShuffle)
    {
        var shuffle = arrayShuffle.FirstOrDefault(e => e.label == isShuffle);
        return shuffle?.value ?? false;
    }

    public bool ValidateQuizQuestionImportDto(QuizQuestionImportDto dto,
        HashSet<(int hashNum, string title)> processHashNum)
    {
        bool isValid = true;

        if (dto.Type == 0)
        {
            AddImportError(dto, $"Không tìm thấy loại câu hỏi {dto.TypeName}");
            isValid = false;
        }
        if (dto.Type != 0 &&
     (((int)dto.Type == 1 || (int)dto.Type == 2) && dto.QuizAnswers.Count() != 1) ||
     ((int)dto.Type == 3 && dto.QuizAnswers.Count() >= 6))
        {

            AddImportError(dto, $"Số lượng câu trả lời chưa phù hợp với loại câu hỏi");
            isValid = false;
        }

        if (dto.HashQuestion == 0 ||
            processHashNum.Contains((dto.HashQuestion, dto.Content))) // you has just compare the old answers 
        {
            AddImportError(dto, $"Câu hỏi '{dto.Content}' bị trùng với câu hỏi đã có ");
            isValid = false;
        }

        if (dto.QuestionLevel == 0)
        {
            AddImportError(dto, $"Không tìm thấy cấp độ câu hỏi {dto.QuestionLevelName}");
            isValid = false;
        }

        if (string.IsNullOrEmpty(dto.Content))
        {
            AddImportError(dto, $"Không tìm thấy nội dung câu hỏi");
            isValid = false;
        }


        return isValid;
    }

    private async Task<(string cacheKeySuccess, string cacheKeyFail, string cacheKeyResult)> CacheQuizQuestions(
        List<QuizQuestion> quizQuestionImportSuccess, List<QuizQuestionImportDto> quizQuestionImportFail,
        List<QuizQuestionImportDto> quizQuestionImportResult)
    {
        // Generate unique cache keys
        string cacheKeySuccess = $"excel-import-success-{Guid.NewGuid()}";
        string cacheKeyFail = $"excel-import-fail-{Guid.NewGuid()}";
        string cacheKeyResult = $"excel-import-Result-{Guid.NewGuid()}";

        // Set cache options
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTimeOffset.Now.AddDays(1))
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));

        // Serialize and cache quizQuestionImportSuccess
        await _redisCacheService.SetStringKey(cacheKeySuccess, JsonConvert.SerializeObject(quizQuestionImportSuccess),
            options);

        // Serialize and cache quizQuestionImportFail
        await _redisCacheService.SetStringKey(cacheKeyFail, JsonConvert.SerializeObject(quizQuestionImportFail),
            options);

        // Serialize and cache quizQuestionImportResult
        await _redisCacheService.SetStringKey(cacheKeyResult, JsonConvert.SerializeObject(quizQuestionImportResult),
            options);

        // Return cache keys
        return (cacheKeySuccess, cacheKeyFail, cacheKeyResult);
    }


    public async Task<ApiResult<bool>> ImportDatabase(string idImport, int quizId)
    {
        if (idImport == null)
        {
            return new ApiResult<bool>(false, "ID Import Not Found");
        }

        // ở bước 3 thực hiện xong thì mới có ID Import Result 
        // how to take the id result. so, we take value ID by parameter 
        var dataImport = await _redisCacheService.GetStringKey(idImport);

        if (string.IsNullOrEmpty(dataImport))
        {
            return new ApiResult<bool>(false, "Data for import not found or invalid");
        }

        try
        {
            // Assuming CreateRangeQuizQuestion accepts dataImport in the expected format
            var quizQuestions = JsonConvert.DeserializeObject<List<QuizQuestion>>(dataImport);
            var quizQuestionCreate = await _quizQuestionRepository.CreateRangeQuizQuestion(quizQuestions);
            var createElastic = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionCreate);
            await _elasticSearchService.CreateDocumentRangeAsync(ElasticConstant.ElasticQuizQuestion, createElastic);
            List<QuizQuestion> quizQuestionList = quizQuestionCreate.ToList();
            // Map to an array of tuples
            var resultArray = quizQuestionList
                .Select(q => new QuizQuestionRelation
                {
                    QuizId = quizId,
                    QuizQuestionId = q.Id
                })
                .ToArray();
            // map insert những thằng này vào db xong mình phải lấy ra được id của nó vừa insert rồi mới thực hiện được 
            //var quizQuestionRelations = 
            var quizQuestionRelationCreate =
                await _quizQuestionRelationRepository.CreateRangeQuizQuestionRelation(resultArray);
            return new ApiResult<bool>(true, $"Import Success: {quizQuestionCreate}");
        }
        catch (Exception ex)
        {
            return new ApiResult<bool>(false, $"Error importing data: {ex.Message}");
        }
    }
}