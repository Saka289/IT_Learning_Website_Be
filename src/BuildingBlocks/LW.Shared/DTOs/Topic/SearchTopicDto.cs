using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Topic;

public class SearchTopicDto : SearchRequestValue
{
    public int? DocumentId { get; set; }
}