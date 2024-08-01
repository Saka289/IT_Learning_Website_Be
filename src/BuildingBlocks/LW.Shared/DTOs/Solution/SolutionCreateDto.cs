namespace LW.Shared.Solution;

public class SolutionCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Coding { get; set; }
    public int ProblemId { get; set; }
    public string UserId { get; set; }
    public bool IsActive { get; set; }
}