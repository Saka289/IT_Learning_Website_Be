namespace LW.Shared.DTOs.Grade;

public class GradeCreateDto
{
    public string Title { get; set; }
    public bool IsActive { get; set; } = true;
    public int LevelId { get; set; }
}