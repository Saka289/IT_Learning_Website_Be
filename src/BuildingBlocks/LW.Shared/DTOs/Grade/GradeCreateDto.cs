namespace LW.Shared.DTOs.Grade;

public class GradeCreateDto
{
    public string Title { get; set; }
    public bool IsActive { get; set; } = false;
    public int LevelId { get; set; }
}