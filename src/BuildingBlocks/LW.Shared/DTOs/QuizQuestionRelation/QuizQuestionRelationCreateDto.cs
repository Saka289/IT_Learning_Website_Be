namespace LW.Shared.DTOs.QuizQuestionRelation;

public class QuizQuestionRelationCreateDto
{
    public int QuizId { get; set; }
    public IEnumerable<int> QuizQuestionIds { get; set; }
}