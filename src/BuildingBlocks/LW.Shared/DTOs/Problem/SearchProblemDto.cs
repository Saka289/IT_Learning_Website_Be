using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.Problem;

public class SearchProblemDto : SearchRequestValue
{
    public EDifficulty? Difficulty { get; set; }

    public int? TopicId { get; set; }
    public int? LessonId { get; set; }
    public EStatusProblem? Status { get; set; }
    public string UserId { get; set; }
}