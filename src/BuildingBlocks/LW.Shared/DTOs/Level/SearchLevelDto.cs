using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Level;

public class SearchLevelDto:SearchRequestParameters
{
    public override string? Key { get; set; } = "keyWord";
}