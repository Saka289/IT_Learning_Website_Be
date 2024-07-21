using LW.Shared.Enums;

namespace LW.Shared.DTOs.UserQuiz;

public class QuestionAnswerDto
{
    public ETypeQuestion Type { get; set; }
    public int QuestionId { get; set; }
    public IEnumerable<int>? AnswerId { get; set; }
}