using LW.Shared.Enums;

namespace LW.Shared.DTOs.Submission;

public class SubmissionDto
{
    public int Id { get; set; }
    public string SourceCode { get; set; }
    public string Status { get; set; }
    public decimal ExecutionTime { get; set; }
    public decimal MemoryUsage { get; set; }
    public int LanguageId { get; set; }
    public int ProblemId { get; set; }
    public string UserId { get; set; }
}