using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.QuizQuestion;

public class SearchAllQuizQuestionDto : SearchRequestValue
{
    public int? QuizId { get; set; } = 0;
    public EQuestionLevel? Level { get; set; }
}