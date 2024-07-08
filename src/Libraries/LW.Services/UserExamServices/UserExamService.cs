using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.ExamAnswerRepositories;
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
    public UserExamService(IExamAnswerRepository answerRepository, IExamRepository examRepository, UserManager<ApplicationUser> userManager, IMapper mapper, IUserExamRepository userExamRepository)
    {
        _answerRepository = answerRepository;
        _examRepository = examRepository;
        _userManager = userManager;
        _mapper = mapper;
        _userExamRepository = userExamRepository;
    }
    public async Task<ApiResult<bool>> CreateRangeUserExam(ExamFormSubmitDto examFormSubmitDto)
    {
        var user = await _userManager.FindByIdAsync(examFormSubmitDto.UserId);
        if (user == null)
        {
            return new ApiResult<bool>(false, $"Not found user with id = {examFormSubmitDto.UserId}");
        }
        var exam = await _examRepository.GetExamById(examFormSubmitDto.ExamId);
        if (exam == null)
        {
            return new ApiResult<bool>(false, $"Not found exam with id = {examFormSubmitDto.ExamId}");
        }

        var scoreOfEachQuestion = (decimal) 10 / exam.NumberQuestion;
        var listAnswerOfExam = await _answerRepository.GetAllExamAnswerByExamId(exam.Id);
        if (listAnswerOfExam.Count() == 0)
        {
            return new ApiResult<bool>(false, $"Not found list answer of exam with id = {examFormSubmitDto.ExamId}");
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
            ExamId = examFormSubmitDto.ExamId,
            Score = totalScore,
            HistoryExam = JsonConvert.SerializeObject(historyAnswers)
        };
        await _userExamRepository.CreateUserExam(userExam);
        return new ApiResult<bool>(true, "Create result of user successfully");
    }

    public async Task<ApiResult<UserExamDto>> GetExamResultById(int id)
    {
        var userExam = await _userExamRepository.GetUserExamById(id);
        if (userExam == null)
        {
            return new ApiResult<UserExamDto>(false,"Not found");
        }

        var result = _mapper.Map<UserExamDto>(userExam);
        return new ApiResult<UserExamDto>(true, result, "Get UserExam successfully");
    } 
}