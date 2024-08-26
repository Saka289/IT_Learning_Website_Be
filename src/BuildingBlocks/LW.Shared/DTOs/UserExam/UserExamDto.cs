using LW.Shared.DTOs.ExamAnswer;

namespace LW.Shared.DTOs.UserExam;

public class UserExamDto
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public int ExamId { get; set; }
    public string? ExamName { get; set; }
    public int ExamCodeId { get; set; }
    
    public string? ExamCodeImage { get; set; }
    public string? Code { get; set; }
    public decimal Score { get; set; }
    public List<HistoryAnswerDto> HistoryExam { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}