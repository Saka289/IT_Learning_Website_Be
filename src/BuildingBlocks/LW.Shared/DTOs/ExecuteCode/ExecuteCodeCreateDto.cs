namespace LW.Shared.DTOs.ExecuteCode;

public class ExecuteCodeCreateDto
{
    public string MainCode { get; set; }
    public string SampleCode { get; set; }
    public int ProblemId { get; set; }
    public int LanguageId { get; set; }
}