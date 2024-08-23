using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Competition;

public class SearchCompetitionDto : SearchRequestValue
{
    public bool? Status { get; set; }
}