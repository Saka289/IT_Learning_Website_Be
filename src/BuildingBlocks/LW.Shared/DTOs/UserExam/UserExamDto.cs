using LW.Shared.DTOs.ExamAnswer;

namespace LW.Shared.DTOs.UserExam;

public class UserExamDto
{
    
    
    public string? UserId { get; set; }
    
    public string? UserName { get; set; }
    
    public int ExamId { get; set; }
    
    public string? ExamName { get; set; }
    
    public decimal Score { get; set; }

    public List<HistoryAnswer> HistoryExam { get; set; }

   
}