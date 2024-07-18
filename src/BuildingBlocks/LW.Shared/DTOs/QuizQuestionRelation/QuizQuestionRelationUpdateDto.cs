namespace LW.Shared.DTOs.QuizQuestionRelation;

public class QuizQuestionRelationUpdateDto
{
    public int QuizId { get; set; }
    public IEnumerable<int> QuizQuestionIds { get; set; }
}