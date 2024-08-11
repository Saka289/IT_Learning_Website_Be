namespace LW.Shared.DTOs.Solution;

public class SolutionUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Coding { get; set; }
    public int ProblemId { get; set; }
    public string UserId { get; set; }
    public bool IsActive { get; set; }
}