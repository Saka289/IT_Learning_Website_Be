namespace LW.Shared.DTOs.ExecuteCode;

public class ExecuteCodeDto
{
    public int Id { get; set; }
    public string MainCode { get; set; }
    public string SampleCode { get; set; }
    public int ProblemId { get; set; }
    public int LanguageId { get; set; }
}