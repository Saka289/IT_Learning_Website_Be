using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Tag;

public class SearchTagDto : SearchRequestParameters
{
    public override string? Key { get; set; } = "keyWord";
}
    
