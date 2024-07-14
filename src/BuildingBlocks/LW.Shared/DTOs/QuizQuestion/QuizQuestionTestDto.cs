using LW.Shared.DTOs.QuizAnswer;

namespace LW.Shared.DTOs.QuizQuestion;

public class QuizQuestionTestDto
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string KeyWord { get; set; }
    public string Content { get; set; }
    public string? Hint { get; set; }
    public string? Image { get; set; }
    public bool IsShuffle { get; set; }
    public bool IsActive { get; set; }
    public string QuestionLevel { get; set; }
    public int QuizId { get; set; }
    public IEnumerable<QuizAnswerTestDto> QuizAnswers { get; set; }
}