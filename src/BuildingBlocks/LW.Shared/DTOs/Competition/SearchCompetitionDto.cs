using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Competition;

public class SearchCompetitionDto : SearchRequestParameters
{
    public override string? Key { get; set; } = "keyWord";
}