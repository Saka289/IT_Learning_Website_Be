using LW.Shared.DTOs.Quiz;
using LW.Shared.DTOs.QuizAnswer;
using LW.Shared.Enums;

namespace LW.Shared.DTOs.QuizQuestion;

public class QuizQuestionCreateDto
{
    public ETypeQuestion Type { get; set; }
    public string Content { get; set; }
    public bool IsActive { get; set; }
    public EQuestionLevel QuestionLevel { get; set; }
    public int QuizId { get; set; }
    public IEnumerable<QuizAnswerCreateDto> QuizAnswers { get; set; }
}