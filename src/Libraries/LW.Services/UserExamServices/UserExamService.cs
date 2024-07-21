using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.ExamAnswerRepositories;
using LW.Data.Repositories.ExamCodeRepositories;
using LW.Data.Repositories.ExamRepositories;
using LW.Data.Repositories.UserExamRepositories;
using LW.Shared.DTOs.ExamAnswer;
using LW.Shared.DTOs.UserExam;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace LW.Services.UserExamServices;

public class UserExamService : IUserExamService
{
    private readonly IExamAnswerRepository _answerRepository;
    private readonly IExamRepository _examRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IUserExamRepository _userExamRepository;
    private readonly IExamCodeRepository _examCodeRepository;

    public UserExamService(IExamAnswerRepository answerRepository, IExamRepository examRepository,
        UserManager<ApplicationUser> userManager, IMapper mapper, IUserExamRepository userExamRepository, IExamCodeRepository examCodeRepository)
    {
        _answerRepository = answerRepository;
        _examRepository = examRepository;
        _userManager = userManager;
        _mapper = mapper;
        _userExamRepository = userExamRepository;
        _examCodeRepository = examCodeRepository;
    }

    public async Task<ApiResult<int>> CreateRangeUserExam(ExamFormSubmitDto examFormSubmitDto)
    {
        var user = await _userManager.FindByIdAsync(examFormSubmitDto.UserId);
        if (user == null)
        {
            return new ApiResult<int>(false, $"Not found user with id = {examFormSubmitDto.UserId}");
        }

        var examCode = await _examCodeRepository.GetExamCodeById(examFormSubmitDto.ExamCodeId);
        if (examCode == null)
        {
            return new ApiResult<int>(false, $"Not found exam code with id = {examFormSubmitDto.ExamCodeId}");
        }

        var scoreOfEachQuestion = (decimal)10 / examCode.Exam.NumberQuestion;
        
        var listAnswerOfExam = await _answerRepository.GetAllExamAnswerByExamCodeId(examCode.Id);
        if (listAnswerOfExam.Count() == 0)
        {
            return new ApiResult<int>(false, $"Not found list answer of examcode with id = {examFormSubmitDto.ExamCodeId}");
        }

        decimal totalScore = 0;
        var historyAnswers = new List<HistoryAnswer>();
        foreach (var userAnswer in examFormSubmitDto.UserAnswerDtos)
        {
            var historyAnswer = new HistoryAnswer();
            var correctAnswer = listAnswerOfExam.FirstOrDefault(a => a.NumberOfQuestion == userAnswer.NumberOfQuestion);
            var isCorrect = correctAnswer != null && correctAnswer.Answer == userAnswer.Answer;
            historyAnswer.NumberOfQuestion = userAnswer.NumberOfQuestion;
            historyAnswer.UserAnswer = userAnswer.Answer.ToString();

            if (isCorrect)
            {
                totalScore += scoreOfEachQuestion;
                historyAnswer.IsCorrect = true;
                historyAnswer.CorrectAnswer = null;
            }
            else
            {
                historyAnswer.IsCorrect = false;
                historyAnswer.CorrectAnswer = correctAnswer.Answer.ToString();
            }

            historyAnswers.Add(historyAnswer);
        }

        var userExam = new UserExam()
        {
            UserId = examFormSubmitDto.UserId,
            ExamCodeId =  examFormSubmitDto.ExamCodeId,
            Score = totalScore,
            HistoryExam = JsonConvert.SerializeObject(historyAnswers)
        };
        var userExamResult =  await _userExamRepository.CreateUserExam(userExam);
        return new ApiResult<int>(true,userExamResult.Id, "Create result of user successfully");
    }

    public async Task<ApiResult<UserExamDto>> GetExamResultById(int id)
    {
        var userExam = await _userExamRepository.GetUserExamById(id);
        if (userExam == null)
        {
            return new ApiResult<UserExamDto>(false, "Not found");
        }

        var result = _mapper.Map<UserExamDto>(userExam);
        return new ApiResult<UserExamDto>(true, result, "Get UserExam successfully");
    }

    public async Task<ApiResult<IEnumerable<UserExamDto>>> GetListResultByUserId(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ApiResult<IEnumerable<UserExamDto>>(false, "User not found");
        }

        var listResult = await _userExamRepository.GetAllUserExamByUserId(userId);
        if (!listResult.Any())
        {
            return new ApiResult<IEnumerable<UserExamDto>>(false,"List not found");
        }

        var result = _mapper.Map<IEnumerable<UserExamDto>>(listResult);
        return new ApiResult<IEnumerable<UserExamDto>>(true, result, "Get list result of user successfully");
    }
}