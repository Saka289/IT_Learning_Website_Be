namespace LW.Shared.Solution;

public class SolutionDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Coding { get; set; }
    public int ProblemId { get; set; }
    public string ProblemTitle { get; set; }
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Image { get; set; }
    public bool IsActive { get; set; }
}