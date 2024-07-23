using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Exam;

public class SearchExamDto:SearchRequestParameters
{
    public override string? Key { get; set; } = "keyWord";
    public int? CompetitionId { get; set; }
   
    public string? Province { get; set; }
    
    public int? Year { get; set; }
    
    public int? Type { get; set; }
        
}