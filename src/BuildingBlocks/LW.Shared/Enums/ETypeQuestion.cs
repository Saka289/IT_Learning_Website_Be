using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum ETypeQuestion
{
    [Display(Name = "Loại true false")]
    QuestionTrueFalse = 1,
    [Display(Name = "Loại bốn câu trả lời")]
    QuestionFourAnswer = 2,
    //QuestionFiveAnswer = 3,

}