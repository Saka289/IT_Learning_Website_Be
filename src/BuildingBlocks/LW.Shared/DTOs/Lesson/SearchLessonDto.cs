using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Lesson;

public class SearchLessonDto : SearchRequestParameters
{
    public override string? Key { get; set; } = "keyWord";
}