using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.QuizQuestion;

public class SearchQuizQuestionDto : SearchRequestParameters
{
    public override string? Key { get; set; } = "keyWord";
    public int? QuizId { get; set; }
}