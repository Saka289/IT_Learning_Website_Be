namespace LW.Shared.DTOs.Editorial;

public class EditorialDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Image { get; set; }
    public int ProblemId { get; set; }
}