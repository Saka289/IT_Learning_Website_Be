using Aspose.Pdf;
using AutoMapper;
using LW.Cache;
using LW.Cache.Interfaces;
using System.Collections;
using AutoMapper;
using LW.Contracts.Common;
using LW.Data.Entities;
using LW.Data.Repositories.QuizAnswerRepositories;
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
<<<<<<< HEAD
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
=======
using Microsoft.EntityFrameworkCore;
>>>>>>> 4267fc22ea8e16fc2b9dbeea30b598b79d5afcb6
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
    private readonly IRedisCache<string> _redisCacheService;
    public QuizQuestionService(IQuizAnswerRepository quizAnswerRepository,
        IQuizQuestionRepository quizQuestionRepository, IRedisCache<string> redisCacheService, IMapper mapper, IQuizRepository quizRepository,
        IElasticSearchService<QuizQuestionDto, int> elasticSearchService, ICloudinaryService cloudinaryService)
    {
        _quizAnswerRepository = quizAnswerRepository;
        _quizQuestionRepository = quizQuestionRepository;
        _mapper = mapper;
        _quizRepository = quizRepository;
        _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
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

    public async Task<ApiResult<IEnumerable<QuizQuestionDto>>> GetAllQuizQuestionByQuizIdPractice(int quizId)
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

        var result = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionList);
        return new ApiSuccessResult<IEnumerable<QuizQuestionDto>>(result);
    }

    public async Task<ApiResult<PagedList<QuizQuestionTestDto>>> GetAllQuizQuestionByQuizIdPaginationTest(int quizId,
        PagingRequestParameters pagingRequestParameters)
    {
        var quizQuestionList = await _quizQuestionRepository.GetAllQuizQuestionByQuizId(quizId);
        if (!quizQuestionList.Any())
        {
            return new ApiResult<PagedList<QuizQuestionTestDto>>(false, "Quiz Question is null !!!");
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

        var result = _mapper.ProjectTo<QuizQuestionTestDto>(quizQuestionList);
        var pagedResult = await PagedList<QuizQuestionTestDto>.ToPageListAsync(result.BuildMock(),
            pagingRequestParameters.PageIndex,
            pagingRequestParameters.PageSize, pagingRequestParameters.OrderBy, pagingRequestParameters.IsAscending);
        return new ApiSuccessResult<PagedList<QuizQuestionTestDto>>(pagedResult);
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

        var quizEntity = await _quizRepository.GetQuizById(quizQuestionUpdateDto.QuizId);
        if (quizEntity is null)
        {
            return new ApiResult<QuizQuestionDto>(false, "Quiz not found !!!");
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

        modelQuestion.KeyWord = quizQuestionUpdateDto.Content.RemoveDiacritics();
        modelQuestion.Image = quizQuestion.Image;
        modelQuestion.PublicId = quizQuestion.PublicId;
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

    public async Task<ApiResult<bool>> CreateRangeQuizQuestion(
        IEnumerable<QuizQuestionCreateDto> quizQuestionsCreateDto)
    {
        foreach (var item in quizQuestionsCreateDto)
        {
            var quizEntity = await _quizRepository.GetQuizById(item.QuizId);
            if (quizEntity is null)
            {
                return new ApiResult<bool>(false, "Quiz not found !!!");
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

            var quizQuestionEntity = _mapper.Map<QuizQuestion>(item);
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

    public async Task<ApiResult<bool>> UpdateRangeQuizQuestion(
        IEnumerable<QuizQuestionUpdateDto> quizQuestionsUpdateDto)
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

            modelQuestion.KeyWord = item.Content.RemoveDiacritics();
            modelQuestion.Image = quizQuestion.Image;
            modelQuestion.PublicId = quizQuestion.PublicId;
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
    public async Task<byte[]> ExportExcel(int checkData = 1, List<Guid>? Ids = null)
    {
        return await GenerateExcelFile();
    }


    private async Task<byte[]> GenerateExcelFile()
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
            var worksheet = CreateWorkSheet(package);
            // Define column headers and widths
            string[] columnHeaders = {
                "STT",
                "Loại câu hỏi",
                "Câu hỏi - max 200 ký tự ",
                "Đáp án 1 - max 200 ký tự",
                "Đáp án 2 - max 200 ký tự",
                "Đáp án 3 - max 200 ký tự",
                "Đáp án 4 - max 200 ký tự",
                "Đáp án 5 - max 200 ký tự",
                "Đáp án 6 - max 200 ký tự", // thay đổi cột import 
                "Đáp án đúng - Chọn một câu trả lời đúng theo số (1,2,3,4,5,6) ",
                "Mức độ câu hỏi",
                "Trộn câu hỏi",
                "Gợi ý câu hỏi",

                };
            int dataStartRow = 3;
            int endRow = 1000; ;  // Calculate end row dynamically
            StyleColumn(columnHeaders, worksheet, dataStartRow,endRow);
            int startRow = dataStartRow + 1; // Assuming data starts from row (dataStartRow + 1)
            AddDataValidation(worksheet, columnHeaders, startRow, endRow, typeQuestions, levels, arrayShuffle);
            // Save the workbook to a memory stream and return the stream as a byte array
            return SavePackageToStream(package);
        }

    }
    // create worksheet and merge cell 
    public ExcelWorksheet CreateWorkSheet(ExcelPackage package)
    {
        // Add a worksheet named "Data Validation"
        var worksheet = package.Workbook.Worksheets.Add("Danh sách câu hỏi");
        // Merge cells for title and set title formatting
        worksheet.Cells["A1:M2"].Merge = true;
        worksheet.Cells["A1"].Value = "Danh Sách Câu Hỏi"; // Replace with your actual title
        worksheet.Row(1).Height = 25;
        worksheet.Row(1).Style.Font.Bold = true;
        worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Row(1).Style.Font.Size = 25;
        return worksheet;
    }
    // style for column 
    public void StyleColumn(string[] columnHeaders, ExcelWorksheet worksheet, int dataStartRow, int endrow)
    {
        int[] columnWidths = { 7, 20, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
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
    private void AddDataValidation(ExcelWorksheet worksheet, string[] columnHeaders, int startRow, int endRow, IEnumerable<ETypeQuestion> comboBoxValues, IEnumerable<EQuestionLevel> levels, IEnumerable<Object> shuffles)
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
    public ApiResult<bool> CheckFileImport(IFormFile fileImport)
    {
        if (fileImport == null || fileImport.Length == 0)
        {
            return new ApiResult<bool>(false, "File Import isn't empty");
        }
        if (!Path.GetExtension(fileImport.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return new ApiResult<bool>(false, "File upload must be format.xlxs");
        }
        // If the file is valid
        return new ApiResult<bool>(true, "File valid");
    }
    public object? CheckCoincidence(IEnumerable<object> items, string name, string nameCompare)
    {
        if (items == null || name == null || nameCompare == null)
        {
            return null; // Handle null inputs gracefully
        }

        // Use case-insensitive comparison (optional)
        var find = items.FirstOrDefault(item =>
        {
            // Kiểm tra xem đối tượng có thuộc tính của nameCompare không
            var nameProperty = item.GetType().GetProperty(nameCompare);
            if (nameProperty == null)
            {
                // Không tìm thấy thuộc tính "Name", trả về false
                return false;
            }

            // Lấy giá trị của thuộc tính "Name" và so sánh với 'name'
            var itemName = nameProperty.GetValue(item) as string;
            return itemName != null && itemName.Equals(name, StringComparison.OrdinalIgnoreCase);
        });

        return find;
    }
    // support error in list
    private void AddImportError(QuizQuestionImportDto dto, string error)
    {
        dto.Errors.Add(error);
        dto.IsImported = false;
    }


    public async Task<QuizQuestionImportParentDto> ImportExcel(IFormFile fileImport, int quizId)
    {
        var quizQuestionImportParentDto = new QuizQuestionImportParentDto();
        var isExcel = CheckFileImport(fileImport);

        if (!isExcel.IsSucceeded)
        {
            return quizQuestionImportParentDto;
        }

        var quizQuestionImportDtos = new List<QuizQuestionImportDto>();
        var quizQuestionImportSuccess = new List<QuizQuestion>();

        using (var stream = new MemoryStream())
        {
            await fileImport.CopyToAsync(stream);

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.FirstOrDefault();

                if (workSheet != null)
                {
                    ProcessWorksheet(workSheet, quizId, quizQuestionImportDtos, quizQuestionImportSuccess, out int countSuccess, out int countFail);

                    quizQuestionImportParentDto.CountSuccess = countSuccess;
                    quizQuestionImportParentDto.CountFail = countFail;
                    quizQuestionImportParentDto.QuizQuestionImportDtos = quizQuestionImportDtos;
                    //quizQuestionImportParentDto.IdImport = await CacheQuizQuestions(quizQuestionImportSuccess);
                }
            }
        }

        return quizQuestionImportParentDto;
    }

    private void ProcessWorksheet(ExcelWorksheet workSheet, int quizId, List<QuizQuestionImportDto> quizQuestionImportDtos, List<QuizQuestion> quizQuestionImportSuccess, out int countSuccess, out int countFail)
    {
        countSuccess = 0;
        countFail = 0;

        for (int row = 4; row <= workSheet.Dimension.Rows; row++)
        {
            var quizQuestionImportDto = CreateQuizQuestionImportDto(workSheet, row, quizId);

            if (ValidateQuizQuestionImportDto(quizQuestionImportDto))
            {
                var quizQuestion = _mapper.Map<QuizQuestion>(quizQuestionImportDto);
                quizQuestionImportDto.IsImported = true;
                quizQuestionImportSuccess.Add(quizQuestion);
                countSuccess++;
            }
            else
            {
                countFail++;
            }

            quizQuestionImportDtos.Add(quizQuestionImportDto);
        }
    }

    private QuizQuestionImportDto CreateQuizQuestionImportDto(ExcelWorksheet workSheet, int row, int quizId)
    {
        var arrayShuffle = new[]
{
    new { label = "Có", value = true },
    new { label = "Không", value = false }
};
        var typeQuestionName = workSheet.Cells[row, 2].Value?.ToString()?.Trim();
        var answer1 = workSheet.Cells[row, 4].Value?.ToString()?.Trim();
        var answer2 = workSheet.Cells[row, 5].Value?.ToString()?.Trim();
        var answer3 = workSheet.Cells[row, 6].Value?.ToString()?.Trim();
        var answer4 = workSheet.Cells[row, 7].Value?.ToString()?.Trim();
        var answer5 = workSheet.Cells[row, 8].Value?.ToString()?.Trim();
        var answer6 = workSheet.Cells[row, 9].Value?.ToString()?.Trim();
        var answerCorrect = workSheet.Cells[row, 10].Value?.ToString()?.Trim();
        var level = workSheet.Cells[row, 11].Value?.ToString()?.Trim();
        var isShuffle = workSheet.Cells[row, 12].Value?.ToString()?.Trim();
        var hint = workSheet.Cells[row, 13].Value?.ToString()?.Trim();

        int result = EnumHelperExtensions
            .GetEnumIntValueFromDisplayName<ETypeQuestion>(typeQuestionName);
        int resultLevel = EnumHelperExtensions.GetEnumIntValueFromDisplayName<EQuestionLevel>(level);
        string[] correctAnswersArray = answerCorrect?.Split(',') ?? Array.Empty<string>();
        IEnumerable<QuizAnswerDto> quizAnswers = new[]
        {
        new QuizAnswerDto(correctAnswersArray.Contains("1"), answer1),
        new QuizAnswerDto(correctAnswersArray.Contains("2"), answer2),
        new QuizAnswerDto(correctAnswersArray.Contains("3"), answer3),
        new QuizAnswerDto(correctAnswersArray.Contains("4"), answer4),
        new QuizAnswerDto(correctAnswersArray.Contains("5"), answer5),
        new QuizAnswerDto(correctAnswersArray.Contains("6"), answer6),
    };
        var shuffle = CheckCoincidence(arrayShuffle, isShuffle, "label");
        return new QuizQuestionImportDto
        {
            QuizId = quizId,
            Content = workSheet.Cells[row, 3].Value?.ToString()?.Trim(),
            QuestionLevel = resultLevel.ToString(),
            QuestionLevelName = level,
            QuizAnswers = quizAnswers,
            Type = result.ToString(),
            TypeName = typeQuestionName,
            IsActive = true,
        };
    }

    private bool ValidateQuizQuestionImportDto(QuizQuestionImportDto dto)
    {
        bool isValid = true;

        if (int.Parse(dto.Type) == 0)
        {
            AddImportError(dto, "Không tìm thấy loại câu hỏi");
            isValid = false;
        }

        if (int.Parse(dto.QuestionLevel) == 0)
        {
            AddImportError(dto, "Không tìm thấy cấp độ câu hỏi");
            isValid = false;
        }
        if (string.IsNullOrEmpty(dto.Content))
        {
            AddImportError(dto, "Không tìm thấy câu hỏi");
            isValid = false;
        }

        return isValid;
    }

    private async Task<string> CacheQuizQuestions(List<QuizQuestion> quizQuestionImportSuccess)
    {
        var cacheKey = $"excel-import-data-{Guid.NewGuid()}";
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.Now.AddDays(1))
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));

        await _redisCacheService.SetStringKey(cacheKey, JsonConvert.SerializeObject(quizQuestionImportSuccess), options);

        return cacheKey;
    }

    public async Task<ApiResult<bool>> ImportDatabase(string idImport)
    {
        if (idImport == null)
        {
            return new ApiResult<bool>(false, "ID Import Not Found");
        }

        var dataImport = await _redisCacheService.GetStringKey(idImport);

        if (string.IsNullOrEmpty(dataImport))
        {
            return new ApiResult<bool>(false, "Data for import not found or invalid");
        }

        try
        {
            // Assuming CreateRangeQuizQuestion accepts dataImport in the expected format
            var quizQuestions = JsonConvert.DeserializeObject<List<QuizQuestion>>(dataImport);
            int create = 0;
            foreach (var item in quizQuestions)
            {
                var quizQuestionCreate = await _quizQuestionRepository.CreateQuizQuestion(item);
                await Console.Out.WriteLineAsync(quizQuestionCreate.ToString());
                create++;
            }
            return new ApiResult<bool>(true, $"Import Success: {create}");
        }
        catch (Exception ex)
        {
            return new ApiResult<bool>(false, $"Error importing data: {ex.Message}");
        }
    }


}