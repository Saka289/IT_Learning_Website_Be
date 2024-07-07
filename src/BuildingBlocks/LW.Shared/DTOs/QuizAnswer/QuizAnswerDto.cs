namespace LW.Shared.DTOs.QuizAnswer;

public class QuizAnswerDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public bool IsCorrect { get; set; }
    public int QuizQuestionId { get; set; }

    public QuizAnswerDto(bool IsCorrect, string Content)
    {
        this.IsCorrect = IsCorrect;
        this.Content = Content;
    }
}