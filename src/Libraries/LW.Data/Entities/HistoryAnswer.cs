namespace LW.Data.Entities;

public class HistoryAnswer
{
    public int NumberOfQuestion { get; set; }
    public string? UserAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public string? CorrectAnswer { get; set; }
}