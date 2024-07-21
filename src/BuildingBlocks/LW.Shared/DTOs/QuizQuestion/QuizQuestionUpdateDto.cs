using LW.Shared.DTOs.Quiz;
using LW.Shared.DTOs.QuizAnswer;
using LW.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace LW.Shared.DTOs.QuizQuestion;

public class QuizQuestionUpdateDto
{
    public int Id { get; set; }
    public ETypeQuestion Type { get; set; }
    public string Content { get; set; }
    public string? Hint { get; set; }
    public IFormFile? Image { get; set; }
    public bool IsShuffle { get; set; }
    public bool IsActive { get; set; }
    public EQuestionLevel QuestionLevel { get; set; }
    public int QuizId { get; set; }
    public IEnumerable<QuizAnswerUpdateDto> QuizAnswers { get; set; }
}