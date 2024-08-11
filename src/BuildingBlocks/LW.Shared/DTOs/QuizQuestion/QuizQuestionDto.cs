using LW.Shared.DTOs.QuizAnswer;
using LW.Shared.DTOs.QuizQuestionRelation;
using LW.Shared.Enums;

namespace LW.Shared.DTOs.QuizQuestion;

public class QuizQuestionDto
{
    public int Id { get; set; }
    public ETypeQuestion Type { get; set; }
    public string KeyWord { get; set; }
    public string Content { get; set; }
    public string? Hint { get; set; }
    public string? Image { get; set; }
    public bool IsShuffle { get; set; }
    public bool IsActive { get; set; }
    public int HashQuestion { get; set; }
    public EQuestionLevel QuestionLevel { get; set; }
    public IEnumerable<QuizQuestionRelationDto> QuizQuestionRelations { get; set; }
    public IEnumerable<QuizAnswerDto> QuizAnswers { get; set; }
}