using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.QuizQuestion;

public class SearchAllQuizQuestionDto : SearchRequestValue
{
    public EQuestionLevel? Level { get; set; }
}