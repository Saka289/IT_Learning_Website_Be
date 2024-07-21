namespace LW.Shared.DTOs;

public class UserGradeCreateRangeDto
{
    public string UserId { get; set; }
    public IEnumerable<int> GradeIds { get; set; }
}