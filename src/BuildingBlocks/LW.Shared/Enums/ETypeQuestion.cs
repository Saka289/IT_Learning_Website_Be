using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum ETypeQuestion
{
    [Display(Name = "Loại true false")] 
    QuestionTrueFalse = 1,
    [Display(Name = "Loại một câu trả lời")]
    QuestionFourAnswer = 2,
    [Display(Name = "Loại nhiều câu trả lời")]
    QuestionMultiChoice = 3,
}