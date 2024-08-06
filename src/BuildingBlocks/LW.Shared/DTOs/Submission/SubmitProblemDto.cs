namespace LW.Shared.DTOs.Submission;

public class SubmitProblemDto
{
    public int ProblemId { get; set; }
    public int LanguageId { get; set; }
    public string SourceCode { get; set; }
    public string UserId { get; set; }
    public bool Submit { get; set; } = false;
}