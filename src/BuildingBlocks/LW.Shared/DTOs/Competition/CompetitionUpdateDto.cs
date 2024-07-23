namespace LW.Shared.DTOs.Competition;

public class CompetitionUpdateDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = false;
}