namespace LW.Shared.DTOs.Competition;

public class CompetitionCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; } = false;
}