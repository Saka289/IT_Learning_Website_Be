namespace LW.Shared.DTOs.TestCase;

public class TestCaseDto
{
    public int Id { get; set; }
    public string? InputView { get; set; }
    public string OutputView { get; set; }
    public string? Input { get; set; }
    public string Output { get; set; }
    public bool IsHidden { get; set; }
    public bool IsActive { get; set; }
    public int ProblemId { get; set; }
}