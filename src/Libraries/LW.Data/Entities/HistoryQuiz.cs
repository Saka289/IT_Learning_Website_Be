using LW.Shared.DTOs.QuizQuestion;

namespace LW.Data.Entities;

public class HistoryQuiz
{
    public int QuestionId { get; set; }
    public IEnumerable<int>? AnswerId { get; set; }
    public decimal Score { get; set; }
    public bool IsCorrect { get; set; }
    public QuizQuestionDto QuizQuestionDto { get; set; }
}