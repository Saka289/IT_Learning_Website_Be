namespace LW.Shared.DTOs.UserQuiz;

public class UserQuizDto
{
    public int Id { get; set; }
    public decimal Score { get; set; }
    public decimal TotalScoreQuiz { get; set; }
    public int  NumberCorrect { get; set; }
    public int TotalQuestion { get; set; }
    public List<HistoryQuizDto>? HistoryQuizzes { get; set; }
    public string UserId { get; set; }
    public int QuizId { get; set; }
    public string? QuizTitle { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}