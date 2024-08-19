using LW.Shared.Enums;

namespace LW.Shared.DTOs.UserQuiz;

public class UserQuizSubmitDto
{
    public IEnumerable<QuestionAnswerDto> QuestionAnswerDto { get; set; }
    public int QuizId { get; set; }
    public string UserId { get; set; }
}