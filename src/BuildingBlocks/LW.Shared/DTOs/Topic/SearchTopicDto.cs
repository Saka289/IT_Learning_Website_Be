using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Topic;

public class SearchTopicDto: SearchRequestParameters
{
    public override string? Key { get; set; } = "keyWord";
}