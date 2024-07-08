using LW.Shared.DTOs.QuizAnswer;
using LW.Shared.Enums;

namespace LW.Shared.DTOs.QuizQuestion;

public class QuizQuestionDto
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string? TypeName { get; set; }
    public string? Content { get; set; }
    public bool IsActive { get; set; }
    public string QuestionLevel { get; set; }
    public string? QuestionLevelName { get; set; }
    public int QuizId { get; set; }
    public IEnumerable<QuizAnswerDto>? QuizAnswers { get; set; }
}