using LW.Shared.Enums;

namespace LW.Shared.DTOs.Submission;

public class SubmissionDto
{
    public int Id { get; set; }
    public int TestCaseId { get; set; }
    public string SourceCode { get; set; }
    public int StatusId { get; set; }
    public string Status { get; set; }
    public float? ExecutionTime { get; set; }
    public float? MemoryUsage { get; set; }
    public string? CompileOutput { get; set; }
    public string? Input { get; set; }
    public string? Output { get; set; }
    public string? ExpectedOutput { get; set; }
    public int LanguageId { get; set; }
    public int ProblemId { get; set; }
    public string UserId { get; set; }
}