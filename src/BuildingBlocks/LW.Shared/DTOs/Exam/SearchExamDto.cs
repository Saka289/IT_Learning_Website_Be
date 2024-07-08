using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Exam;

public class SearchExamDto:SearchRequestParameters
{
    public override string? Key { get; set; } = "keyWord";
}