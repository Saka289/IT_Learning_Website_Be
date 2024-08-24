using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Exam;

public class SearchExamDto : SearchRequestValue
{
    public int? CompetitionId { get; set; }
    public string? Province { get; set; }
    public int? Year { get; set; }
    public int? Type { get; set; }
    
    public int? LevelId { get; set; }
    public int? GradeId { get; set; }
    
    public bool? Status { get; set; }
}