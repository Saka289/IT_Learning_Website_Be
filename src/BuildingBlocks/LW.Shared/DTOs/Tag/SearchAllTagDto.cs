using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Tag;

public class SearchAllTagDto 
{
    public IEnumerable<string> TagValue { get; set; }
    public bool OrderDate { get; set; } = false;
    public bool? Status { get; set; }
}