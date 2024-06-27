using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Grade;

public class SearchGradeDto : SearchRequestParameters
{
    public override string? Key { get; set; } = "keyWord";
    public int? LevelId { get; set; }
}