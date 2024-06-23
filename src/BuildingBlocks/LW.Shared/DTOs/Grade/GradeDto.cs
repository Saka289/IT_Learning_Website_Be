namespace LW.Shared.DTOs.Grade;

public class GradeDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string KeyWord { get; set; }
    public bool IsActive { get; set; } 
    public int LevelId { get; set; }
}