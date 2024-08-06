namespace LW.Shared.DTOs.UserExam;

public class HistoryAnswerDto
{
    public int NumberOfQuestion { get; set; }
    public string? UserAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public string? CorrectAnswer { get; set; }
}