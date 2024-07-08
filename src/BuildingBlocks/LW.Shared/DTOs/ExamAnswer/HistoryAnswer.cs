namespace LW.Shared.DTOs.ExamAnswer;

public class HistoryAnswer
{
    public int NumberOfQuestion { get; set; }
    public string? UserAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public string? CorrectAnswer { get; set; }

}