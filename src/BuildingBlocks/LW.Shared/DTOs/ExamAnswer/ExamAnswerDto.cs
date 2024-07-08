namespace LW.Shared.DTOs.ExamAnswer;

public class ExamAnswerDto
{
    public int Id { get; set; }
    
    public int NumberOfQuestion { get; set; }
    
    public char Answer { get; set; }
    
    public int ExamId { get; set; }
}