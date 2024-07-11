using AutoMapper;
using LW.Data.Entities;
using LW.Data.Repositories.QuizAnswerRepositories;
using LW.Data.Repositories.QuizQuestionRepositories;
using LW.Data.Repositories.QuizRepositories;
using LW.Data.Repositories.UserQuizRepositories;
using LW.Shared.DTOs.QuizQuestion;
using LW.Shared.DTOs.UserQuiz;
using LW.Shared.Enums;
using LW.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace LW.Services.UserQuizServices;

public class UserQuizService : IUserQuizService
{
    private readonly IUserQuizRepository _userQuizRepository;
    private readonly IQuizAnswerRepository _quizAnswerRepository;
    private readonly IQuizQuestionRepository _quizQuestionRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public UserQuizService(IUserQuizRepository userQuizRepository, IQuizAnswerRepository quizAnswerRepository,
        IMapper mapper, IQuizQuestionRepository quizQuestionRepository, UserManager<ApplicationUser> userManager,
        IQuizRepository quizRepository)
    {
        _userQuizRepository = userQuizRepository;
        _quizAnswerRepository = quizAnswerRepository;
        _mapper = mapper;
        _quizQuestionRepository = quizQuestionRepository;
        _userManager = userManager;
        _quizRepository = quizRepository;
    }

    public async Task<ApiResult<UserQuizDto>> SubmitQuiz(UserQuizSubmitDto userQuizSubmitDto)
    {
        var quiz = await _quizRepository.GetQuizById(userQuizSubmitDto.QuizId);
        if (quiz is null)
        {
            return new ApiResult<UserQuizDto>(false, "Quiz is null !!!");
        }

        var userCheck = await _userManager.FindByIdAsync(userQuizSubmitDto.UserId);
        if (userCheck == null)
        {
            return new ApiResult<UserQuizDto>(false, "User not found");
        }

        var listHistory = new List<HistoryQuizDto>();
        var quizAnswer = await _quizAnswerRepository.GetQuizAnswerByQuizIdCorrect(userQuizSubmitDto.QuizId);
        var numberScore = quiz.Score / userQuizSubmitDto.QuestionAnswerDto.Count();
        foreach (var itemSubmit in userQuizSubmitDto.QuestionAnswerDto)
        {
            var quizQuestion = await _quizQuestionRepository.GetQuizQuestionById(itemSubmit.QuestionId);
            if (itemSubmit.Type != ETypeQuestion.QuestionMultiChoice)
            {
                if (quizAnswer.Any(qa => qa.Id == itemSubmit.AnswerId.FirstOrDefault()))
                {
                    var history = new HistoryQuizDto()
                    {
                        QuestionId = itemSubmit.QuestionId,
                        AnswerId = itemSubmit.AnswerId ?? null,
                        Score = numberScore,
                        IsCorrect = true,
                        QuizQuestionDto = _mapper.Map<QuizQuestionDto>(quizQuestion)
                    };
                    listHistory.Add(history);
                }
                else
                {
                    var history = new HistoryQuizDto()
                    {
                        QuestionId = itemSubmit.QuestionId,
                        AnswerId = itemSubmit.AnswerId ?? null,
                        IsCorrect = false,
                        Score = 0,
                        QuizQuestionDto = _mapper.Map<QuizQuestionDto>(quizQuestion)
                    };
                    listHistory.Add(history);
                }
            }
            else
            {
                var listAnswer = quizAnswer.Where(qa => qa.QuizQuestionId == itemSubmit.QuestionId).Select(x => x.Id);
                var listAnswerUser = itemSubmit.AnswerId;
                var listBoth = listAnswer.Intersect(listAnswerUser).ToList();
                if (listBoth.Any())
                {
                    if (listBoth.Count() == listAnswer.Count())
                    {
                        var history = new HistoryQuizDto()
                        {
                            QuestionId = itemSubmit.QuestionId,
                            AnswerId = itemSubmit.AnswerId ?? null,
                            IsCorrect = true,
                            Score = numberScore,
                            QuizQuestionDto = _mapper.Map<QuizQuestionDto>(quizQuestion)
                        };
                        listHistory.Add(history);
                    }
                    else
                    {
                        var totalAnswer = await _quizAnswerRepository.GetAllQuizAnswerByQuizQuestionId(itemSubmit.QuestionId);
                        var scoreQuestion = numberScore / totalAnswer.Count();
                        var scoreResult = scoreQuestion * listBoth.Count();
                        var history = new HistoryQuizDto()
                        {
                            QuestionId = itemSubmit.QuestionId,
                            AnswerId = itemSubmit.AnswerId ?? null,
                            IsCorrect = true,
                            Score = scoreResult,
                            QuizQuestionDto = _mapper.Map<QuizQuestionDto>(quizQuestion)
                        };
                        listHistory.Add(history);
                    }
                }
                else
                {
                    var history = new HistoryQuizDto()
                    {
                        QuestionId = itemSubmit.QuestionId,
                        AnswerId = itemSubmit.AnswerId ?? null,
                        IsCorrect = false,
                        Score = 0,
                        QuizQuestionDto = _mapper.Map<QuizQuestionDto>(quizQuestion)
                    };
                    listHistory.Add(history);
                }
            }
        }

        int numberCorrect = Convert.ToInt32(listHistory.Count(h => h.IsCorrect));
        int totalQuestion = listHistory.Count();
        var Totalscore = listHistory.Sum(h => h.Score);
        var userQuiz = new UserQuiz()
        {
            Score = Totalscore,
            NumberCorrect = numberCorrect,
            TotalQuestion = totalQuestion,
            HistoryQuizzes = _mapper.Map<List<HistoryQuiz>>(listHistory),
            QuizId = userQuizSubmitDto.QuizId,
            UserId = userQuizSubmitDto.UserId,
        };
        var userQuizCreate = await _userQuizRepository.CreateUserQuiz(userQuiz);
        var result = _mapper.Map<UserQuizDto>(userQuizCreate);
        return new ApiSuccessResult<UserQuizDto>(result);
    }

    public async Task<ApiResult<IEnumerable<UserQuizDto>>> GetAllUserQuizByUserId(string userId)
    {
        var listUserQuizEntity = await _userQuizRepository.GetAllUserQuizByUserId(userId);
        if (!listUserQuizEntity.Any())
        {
            return new ApiResult<IEnumerable<UserQuizDto>>(false, "User Quiz not found !!!");
        }

        var result = _mapper.Map<IEnumerable<UserQuizDto>>(listUserQuizEntity);
        return new ApiSuccessResult<IEnumerable<UserQuizDto>>(result);
    }

    public async Task<ApiResult<bool>> DeleteUserQuizById(int id)
    {
        var userQuizEntity = await _userQuizRepository.GetUserQuizById(id);
        if (userQuizEntity is null)
        {
            return new ApiResult<bool>(false, "User Quiz not found !!!");
        }

        var userQuiz = await _userQuizRepository.DeleteUserQuiz(id);
        if (!userQuiz)
        {
            return new ApiResult<bool>(false, "Failed Delete User Quiz !!!");
        }

        return new ApiResult<bool>(true, "Delete user quiz successfully !!!");
    }
}