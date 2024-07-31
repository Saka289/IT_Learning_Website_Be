namespace LW.Shared.DTOs.QuizQuestionRelation;

public class QuestionRelationCustomCreateDto
{
    public int QuizChildId { get; set; }
    public int NumberOfQuestion { get; set; }
    public bool Shuffle { get; set; } = false;
}