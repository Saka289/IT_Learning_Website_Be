using Aspose.Pdf;
using AutoMapper;
using LW.Cache;
using LW.Cache.Interfaces;
using LW.Data.Entities;
using LW.Data.Repositories.QuizAnswerRepositories;
using LW.Data.Repositories.QuizQuestionRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Infrastructure.Extensions;
using LW.Shared.DTOs.Email;
using LW.Shared.DTOs.Enum;
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

namespace LW.Services.QuizQuestionServices;

public class QuizQuestionService : IQuizQuestionService
{
    private readonly IQuizAnswerRepository _quizAnswerRepository;
    private readonly IQuizQuestionRepository _quizQuestionRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IMapper _mapper;
    private readonly IRedisCache<string> _redisCacheService;
    public QuizQuestionService(IQuizAnswerRepository quizAnswerRepository,
        IQuizQuestionRepository quizQuestionRepository, IRedisCache<string> redisCacheService, IMapper mapper, IQuizRepository quizRepository)
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

        ETypeQuestion[] typeQuestions = (ETypeQuestion[])Enum.GetValues(typeof(ETypeQuestion));
        EQuestionLevel[] levels = (EQuestionLevel[])Enum.GetValues(typeof(EQuestionLevel));
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
                "Đáp án đúng - Chọn một câu trả lời đúng theo số (1,2,3,4) ",
                "Mức độ câu hỏi",
                };
            int dataStartRow = 3;
            StyleColumn(columnHeaders, worksheet, dataStartRow);
            int startRow = dataStartRow + 1; // Assuming data starts from row (dataStartRow + 1)
            int endRow = 1000; ;  // Calculate end row dynamically
            AddDataValidation(worksheet, columnHeaders, startRow, endRow, typeQuestions, levels);
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
        worksheet.Cells["A1:I2"].Merge = true;
        worksheet.Cells["A1"].Value = "Danh Sách Câu Hỏi"; // Replace with your actual title
        worksheet.Row(1).Height = 25;
        worksheet.Row(1).Style.Font.Bold = true;
        worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Row(1).Style.Font.Size = 25;
        return worksheet;
    }
    // style for column 
    public void StyleColumn(string[] columnHeaders, ExcelWorksheet worksheet, int dataStartRow)
    {
        int[] columnWidths = { 7, 20, 30, 30, 30, 30, 30, 30, 30 };
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
    // create combobox 
    private void AddDataValidation(ExcelWorksheet worksheet, string[] columnHeaders, int startRow, int endRow, IEnumerable<ETypeQuestion> comboBoxValues, IEnumerable<EQuestionLevel> levels)
    {
        int departmentColumnIndex = Array.IndexOf(columnHeaders, "Loại câu hỏi");
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
    //public async Task<QuizQuestionImportParentDto> ImportExcel(IFormFile fileImport, int quizId)
    //{
    //    var isExcel = CheckFileImport(fileImport);
    //    var quizQuestionImportParentDtos = new QuizQuestionImportParentDto();
    //    if (isExcel.IsSucceeded)
    //    {
    //        int countSuccess = 0, countFail = 0;
    //        var quizQuestionImportDtos = new List<QuizQuestionImportDto>();
    //        var quizQuestionImportSuccess = new List<QuizQuestion>();
    //        using (var stream = new MemoryStream())
    //        {
    //            // copy vào tệp stream 
    //            fileImport.CopyTo(stream);
    //            // thực hiện đọc dữ liệu trong file
    //            using (var package = new ExcelPackage(stream))
    //            {
    //                // Đọc worksheet đầu 
    //                ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
    //                if (workSheet != null)
    //                {
    //                    var rowCount = workSheet.Dimension.Rows;
    //                    for (int row = 4; row <= rowCount; row++)
    //                    {
    //                        var quizQuestionImportDto = new QuizQuestionImportDto();
    //                        var typeQuestionName = workSheet?.Cells[row, 2]?.Value?.ToString()?.Trim();
    //                        var answer1 = workSheet?.Cells[row, 4]?.Value?.ToString()?.Trim();
    //                        var answer2 = workSheet?.Cells[row, 5]?.Value?.ToString()?.Trim();
    //                        var answer3 = workSheet?.Cells[row, 6]?.Value?.ToString()?.Trim();
    //                        var answer4 = workSheet?.Cells[row, 7]?.Value?.ToString()?.Trim();
    //                        var answerCorrect = workSheet?.Cells[row, 8]?.Value?.ToString()?.Trim();
    //                        var level = workSheet?.Cells[row, 9]?.Value?.ToString()?.Trim(); 
    //                        int result = EnumExtensions.GetEnumIntValueFromDisplayName<ETypeQuestion>(typeQuestionName);
    //                        int resultLevel = EnumExtensions.GetEnumIntValueFromDisplayName<EQuestionLevel>(level);
    //                        var resultTypeQuestion = result != 0 ? result : 0;
    //                        var resultLevelQuestion = resultLevel != 0 ? resultLevel : 0;
    //                        IEnumerable<QuizAnswerDto> QuizAnswerExcel = new[]
    //                            {
    //                              new QuizAnswerDto(answerCorrect.Equals("1"), answer1),  // Correct answer for question 1
    //                              new QuizAnswerDto(answerCorrect.Equals("2"), answer2), // Incorrect answer for question 1
    //                              new QuizAnswerDto(answerCorrect.Equals("3"), answer3), // Incorrect answer for question 1
    //                              new QuizAnswerDto(answerCorrect.Equals("4"), answer4), // Incorrect answer for question 1
    //                            };
    //                        quizQuestionImportDto = new QuizQuestionImportDto
    //                        {
    //                            QuizId = quizId,
    //                            Content = workSheet?.Cells[row, 3]?.Value?.ToString()?.Trim(),
    //                            QuestionLevel = resultLevel.ToString(),
    //                            QuizAnswers = QuizAnswerExcel,
    //                            Type = result.ToString(),
    //                            IsActive = true,
    //                        };
    //                        bool check = true;
    //                        // không tìm thấy 
    //                        if (resultTypeQuestion == 0)
    //                        {
    //                            AddImportError(quizQuestionImportDto, "Không tìm thấy loại câu hỏi ");
    //                            check = false;
    //                        }if (resultLevelQuestion == 0)
    //                        {
    //                            AddImportError(quizQuestionImportDto, "Không tìm thấy cấp độ câu hỏi ");
    //                            check = false;
    //                        }


    //                        if (check == true)
    //                        {
    //                            var quizQuestion = _mapper.Map<QuizQuestion>(quizQuestionImportDto);
    //                            countSuccess++;
    //                            quizQuestionImportDto.IsImported = true;
    //                            quizQuestionImportSuccess.Add(quizQuestion);

    //                        }
    //                        if (resultTypeQuestion == 0 || resultLevelQuestion == 0)
    //                        {
    //                            countFail++;
    //                        }

    //                        quizQuestionImportDtos.Add(quizQuestionImportDto);

    //                    }
    //                }
    //                quizQuestionImportParentDtos.CountSuccess = countSuccess;
    //                quizQuestionImportParentDtos.CountFail = countFail;
    //                quizQuestionImportParentDtos.QuizQuestionImportDtos = quizQuestionImportDtos;
    //                var cacheKey = $"excel-import-data-{Guid.NewGuid()}"; // Use a unique key
    //                quizQuestionImportParentDtos.IdImport = cacheKey;
    //                DateTimeOffset expiryTime = DateTimeOffset.Now.AddDays(1);
    //                // Define cache entry options with absolute and sliding expiration
    //                var options = new DistributedCacheEntryOptions()
    //                    .SetAbsoluteExpiration(DateTime.Now.AddDays(1))  // Set absolute expiration to one day
    //                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));  // Set sliding expiration to 5 minutes
    //                await _redisCacheService.SetStringKey(cacheKey, JsonConvert.SerializeObject(quizQuestionImportSuccess), options);

    //            }
    //        }

    //        return quizQuestionImportParentDtos;
    //    }
    //    return quizQuestionImportParentDtos;
    //}

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
                    quizQuestionImportParentDto.IdImport = await CacheQuizQuestions(quizQuestionImportSuccess);
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
        var typeQuestionName = workSheet.Cells[row, 2].Value?.ToString()?.Trim();
        var answer1 = workSheet.Cells[row, 4].Value?.ToString()?.Trim();
        var answer2 = workSheet.Cells[row, 5].Value?.ToString()?.Trim();
        var answer3 = workSheet.Cells[row, 6].Value?.ToString()?.Trim();
        var answer4 = workSheet.Cells[row, 7].Value?.ToString()?.Trim();
        var answerCorrect = workSheet.Cells[row, 8].Value?.ToString()?.Trim();
        var level = workSheet.Cells[row, 9].Value?.ToString()?.Trim();

        int result = EnumExtensions.GetEnumIntValueFromDisplayName<ETypeQuestion>(typeQuestionName);
        int resultLevel = EnumExtensions.GetEnumIntValueFromDisplayName<EQuestionLevel>(level);

        IEnumerable<QuizAnswerDto> quizAnswers = new[]
        {
        new QuizAnswerDto(answerCorrect.Equals("1"), answer1),
        new QuizAnswerDto(answerCorrect.Equals("2"), answer2),
        new QuizAnswerDto(answerCorrect.Equals("3"), answer3),
        new QuizAnswerDto(answerCorrect.Equals("4"), answer4),
    };

        return new QuizQuestionImportDto
        {
            QuizId = quizId,
            Content = workSheet.Cells[row, 3].Value?.ToString()?.Trim(),
            QuestionLevel = resultLevel.ToString(),
            QuizAnswers = quizAnswers,
            Type = result.ToString(),
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