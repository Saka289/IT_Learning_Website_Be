namespace LW.Shared.DTOs.TestCase;

public class TestCaseCreateDto
{
    public string? Input { get; set; }
    public string Output { get; set; }
    public bool IsHidden { get; set; }
    public bool IsActive { get; set; }
    public int ProblemId { get; set; }
}