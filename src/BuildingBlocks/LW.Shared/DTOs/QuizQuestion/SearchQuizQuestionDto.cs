using System.ComponentModel.DataAnnotations;
using LW.Shared.Enums;
using LW.Shared.SeedWork;

namespace LW.Shared.DTOs.QuizQuestion;

public class SearchQuizQuestionDto : SearchRequestValue
{
    [Required]
    public int QuizId { get; set; } = 0;
    public int NumberOfQuestion { get; set; } = 0;
}