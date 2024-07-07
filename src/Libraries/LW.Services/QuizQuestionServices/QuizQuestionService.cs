using Aspose.Pdf;
using AutoMapper;
using LW.Cache;
using LW.Cache.Interfaces;
using LW.Data.Entities;
using LW.Data.Repositories.QuizAnswerRepositories;
using LW.Data.Repositories.QuizQuestionRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Shared.DTOs.Email;
using LW.Shared.DTOs.QuizAnswer;
using LW.Shared.DTOs.QuizQuestion;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace LW.Services.QuizQuestionServices;

public class QuizQuestionService : IQuizQuestionService
{
    private readonly IQuizAnswerRepository _quizAnswerRepository;
    private readonly IQuizQuestionRepository _quizQuestionRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IMapper _mapper;
    private readonly IRedisCache<IEnumerable<QuizQuestion>> _redisCacheService;
    public QuizQuestionService(IQuizAnswerRepository quizAnswerRepository,
        IQuizQuestionRepository quizQuestionRepository, IRedisCache<IEnumerable<QuizQuestion>> redisCacheService, IMapper mapper, IQuizRepository quizRepository)
    {
        _quizAnswerRepository = quizAnswerRepository;
        _quizQuestionRepository = quizQuestionRepository;
        _mapper = mapper;
        _quizRepository = quizRepository;
        _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
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

    public async Task<ApiResult<IEnumerable<QuizQuestionDto>>> GetAllQuizQuestionByQuizId(int quizId)
    {
        var quizQuestionList = await _quizQuestionRepository.GetAllQuizQuestionByQuizId(quizId);
        if (!quizQuestionList.Any())
        {
            return new ApiResult<IEnumerable<QuizQuestionDto>>(false, "Quiz Question is null !!!");
        }

        var result = _mapper.Map<IEnumerable<QuizQuestionDto>>(quizQuestionList);
        return new ApiSuccessResult<IEnumerable<QuizQuestionDto>>(result);
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

        var quizQuestionEntity = _mapper.Map<QuizQuestion>(quizQuestionCreateDto);
        var quizQuestionCreate = await _quizQuestionRepository.CreateQuizQuestion(quizQuestionEntity);
        var result = _mapper.Map<QuizQuestionDto>(quizQuestionCreate);
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

        var modelQuestion = _mapper.Map(quizQuestionUpdateDto, quizQuestionEntity);
        var quizQuestionUpdate = await _quizQuestionRepository.UpdateQuizQuestion(modelQuestion);
        foreach (var item in quizQuestionUpdateDto.QuizAnswers)
        {
            var quizAnswer = await _quizAnswerRepository.GetQuizAnswerById(item.Id);
            var modelAnswer = _mapper.Map(item, quizAnswer);
            await _quizAnswerRepository.UpdateQuizAnswer(modelAnswer);
        }

        var result = _mapper.Map<QuizQuestionDto>(quizQuestionUpdate);
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

            var quizQuestionEntity = _mapper.Map<QuizQuestion>(item);
            var quizQuestionCreate = await _quizQuestionRepository.CreateQuizQuestion(quizQuestionEntity);
            var result = _mapper.Map<QuizQuestionDto>(quizQuestionCreate);
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

            var modelQuestion = _mapper.Map(item, quizQuestionEntity);
            var quizQuestionUpdate = await _quizQuestionRepository.UpdateQuizQuestion(modelQuestion);
            foreach (var itemAnswer in item.QuizAnswers)
            {
                var quizAnswer = await _quizAnswerRepository.GetQuizAnswerById(itemAnswer.Id);
                var modelAnswer = _mapper.Map(itemAnswer, quizAnswer);
                await _quizAnswerRepository.UpdateQuizAnswer(modelAnswer);
            }
            // var result = _mapper.Map<QuizQuestionDto>(quizQuestionUpdate);
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

        return new ApiSuccessResult<bool>(true);
    }
    public async Task<byte[]> ExportExcel(int checkData = 1, List<Guid>? Ids = null)
    {
        return await GenerateExcelFile();
    }


    private async Task<byte[]> GenerateExcelFile()
    {
        var comboBoxValues = await _quizRepository.GetAllQuiz();

        using (var package = new ExcelPackage())
        {
            // Add a worksheet named "Data Validation"
            var worksheet = CreateWorkSheet(package);
            // Define column headers and widths
            string[] columnHeaders = {
                "STT",
                "Tiêu đề quiz",
                "Mô tả nội dung",
                "Câu hỏi - max 200 ký tự ",
                "Đáp án 1 - max 200 ký tự",
                "Đáp án 2 - max 200 ký tự",
                "Đáp án 3 - max 200 ký tự",
                "Đáp án 4 - max 200 ký tự",
                "Đáp án đúng - Chọn một câu trả lời đúng theo số (1,2,3,4) ",
                "Mức độ câu hỏi (khó vừa dễ)",
                };
            int dataStartRow = 3;
            StyleColumn(columnHeaders, worksheet, dataStartRow);
            int startRow = dataStartRow + 1; // Assuming data starts from row (dataStartRow + 1)
            int endRow = 1000; ;  // Calculate end row dynamically
            AddDataValidation(worksheet, columnHeaders, startRow, endRow, comboBoxValues);
            // Save the workbook to a memory stream and return the stream as a byte array
            return SavePackageToStream(package);
        }

    }
    // tạo worksheet và merge cell 
    public ExcelWorksheet CreateWorkSheet(ExcelPackage package)
    {
        // Add a worksheet named "Data Validation"
        var worksheet = package.Workbook.Worksheets.Add("Danh sách câu hỏi");
        // Merge cells for title and set title formatting
        worksheet.Cells["A1:I2"].Merge = true;
        worksheet.Cells["A1"].Value = "Danh Sách Câu Hỏi"; // Replace with your actual title
        worksheet.Row(1).Height = 25;
        worksheet.Row(1).Style.Font.Bold = true;
        worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Row(1).Style.Font.Size = 25;
        return worksheet;
    }
    // style cho các column 
    public void StyleColumn(string[] columnHeaders, ExcelWorksheet worksheet, int dataStartRow)
    {
        int[] columnWidths = { 7, 20, 30, 30, 30, 30, 30, 30, 30, 30 };
        // Add column headers and set column widths
        for (int i = 0; i < columnHeaders.Length; i++)
        {

            worksheet.Cells[dataStartRow, i + 1].Value = columnHeaders[i];
            worksheet.Cells[dataStartRow, i + 1].Style.WrapText = true;
            worksheet.Column(i + 1).Width = columnWidths[i];
            worksheet.Row(dataStartRow).Style.Font.Size = 12; // Set font size for headers row
            worksheet.Row(dataStartRow).Style.Font.Bold = true;
        }
    }
    // tạo combobox 
    private void AddDataValidation(ExcelWorksheet worksheet, string[] columnHeaders, int startRow, int endRow, IEnumerable<Quiz> comboBoxValues)
    {
        int departmentColumnIndex = Array.IndexOf(columnHeaders, "Tiêu đề quiz");
        var validationRange = worksheet.Cells[startRow, departmentColumnIndex + 1, endRow, departmentColumnIndex + 1];

        var validation = validationRange.DataValidation.AddListDataValidation();

        foreach (var value in comboBoxValues)
        {
            validation.Formula.Values.Add(value.Title);
        }
    }
    // Lưu package by stream 
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
    // Hàm hỗ trợ thêm lỗi import vào danh sách
    private void AddImportError(QuizQuestionImportDto dto, string error)
    {
        dto.Errors.Add(error);
        dto.IsImported = false;
    }
    public async Task<QuizQuestionImportParentDto> ImportExcel(IFormFile fileImport)
    {
        var isExcel = CheckFileImport(fileImport);
        var quizQuestionImportParentDtos = new QuizQuestionImportParentDto();
        if (isExcel.IsSucceeded)
        {
            int countSuccess = 0, countFail = 0;
            var quizQuestionImportDtos = new List<QuizQuestionImportDto>();
            var quizQuestionImportSuccess = new List<QuizQuestion>();
            var quizQuestions = await _quizRepository.GetAllQuiz();
            using (var stream = new MemoryStream())
            {
                // copy vào tệp stream 
                fileImport.CopyTo(stream);
                // thực hiện đọc dữ liệu trong file
                using (var package = new ExcelPackage(stream))
                {
                    // Đọc worksheet đầu 
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    if (workSheet != null)
                    {
                        var rowCount = workSheet.Dimension.Rows;
                        for (int row = 4; row <= rowCount; row++)
                        {
                            var quizQuestionImportDto = new QuizQuestionImportDto();
                            var quizName = workSheet?.Cells[row, 2]?.Value?.ToString()?.Trim();
                            var answer1 = workSheet?.Cells[row, 5]?.Value?.ToString()?.Trim();
                            var answer2 = workSheet?.Cells[row, 6]?.Value?.ToString()?.Trim();
                            var answer3 = workSheet?.Cells[row, 7]?.Value?.ToString()?.Trim();
                            var answer4 = workSheet?.Cells[row, 8]?.Value?.ToString()?.Trim();
                            var answerCorrect = workSheet?.Cells[row, 9]?.Value?.ToString()?.Trim();
                            var checkQuizName = CheckCoincidence(quizQuestions, quizName, "Title");
                            IEnumerable<QuizAnswerDto> QuizAnswerExcel = new[]
                                {
                                  new QuizAnswerDto(answerCorrect.Equals("1"), answer1),  // Correct answer for question 1
                                  new QuizAnswerDto(answerCorrect.Equals("2"), answer2), // Incorrect answer for question 1
                                  new QuizAnswerDto(answerCorrect.Equals("3"), answer3), // Incorrect answer for question 1
                                  new QuizAnswerDto(answerCorrect.Equals("4"), answer4), // Incorrect answer for question 1
                                };
                            quizQuestionImportDto = new QuizQuestionImportDto
                            {
                                QuizId = checkQuizName != null ? (int)checkQuizName.GetType().GetProperty("Id")?.GetValue(checkQuizName, null) : 0,
                                Content = workSheet?.Cells[row, 3]?.Value?.ToString()?.Trim(),
                                QuestionLevel = workSheet?.Cells[row, 10]?.Value?.ToString()?.Trim(),
                                QuizAnswers = QuizAnswerExcel,
                            };
                            bool check = true;
                            // không tìm thấy 
                            if (checkQuizName == null)
                            {
                                AddImportError(quizQuestionImportDto, "Không tìm thấy ");
                                check = false;
                            }
                            var quizQuestion = _mapper.Map<QuizQuestion>(quizQuestionImportDto);

                            if (check == true)
                            {
                                countSuccess++;
                                quizQuestionImportDto.IsImported = true;
                                quizQuestionImportSuccess.Add(quizQuestion);
                            }
                            if (checkQuizName != null)
                            {
                                countFail++;
                            }

                            quizQuestionImportDtos.Add(quizQuestionImportDto);

                        }
                    }
                    quizQuestionImportParentDtos.CountSuccess = countSuccess;
                    quizQuestionImportParentDtos.CountFail = countFail;
                    quizQuestionImportParentDtos.QuizQuestionImportDtos = quizQuestionImportDtos;

                    var cacheKey = $"excel-import-data-{Guid.NewGuid()}"; // Use a unique key
                    quizQuestionImportParentDtos.IdImport = cacheKey;
                    DateTimeOffset expiryTime = DateTimeOffset.Now.AddDays(1);
                    // Define cache entry options with absolute and sliding expiration
                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddDays(1))  // Set absolute expiration to one day
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5));  // Set sliding expiration to 5 minutes
                    await _redisCacheService.SetStringKey(cacheKey, quizQuestionImportSuccess, options);

                }
            }

            return quizQuestionImportParentDtos;
        }
        return quizQuestionImportParentDtos;
    }
    public async Task<ApiResult<bool>> ImportDatabase(string idImport)
    {
        if (idImport == null)
        {
            return new ApiResult<bool>(false, "ID Import Not Found ");
        }
        var dataImport = await _redisCacheService.GetStringKey(idImport);
        //var quizQuestionEntity = jArray?.ToObject<List<>>();

        var create = await _quizQuestionRepository.CreateRangeQuizQuestion(dataImport);
        return new ApiResult<bool>(true, $"Import Success {create}");
    }

}