namespace LW.Shared.DTOs.QuizQuestionRelation;

public class QuizQuestionRelationCustomCreateDto
{
    public int QuizId { get; set; }
    public IEnumerable<QuestionRelationCustomCreateDto> QuiQuestionRelationCustomCreate { get; set; }
}