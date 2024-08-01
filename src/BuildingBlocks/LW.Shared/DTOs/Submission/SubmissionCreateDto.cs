namespace LW.Shared.DTOs.Submission;

public class SubmissionCreateDto
{
    public string SourceCode { get; set; }
    public string Status { get; set; }
    public float ExecutionTime { get; set; }
    public float MemoryUsage { get; set; }
    public int LanguageId { get; set; }
    public int ProblemId { get; set; }
    public string UserId { get; set; }
}