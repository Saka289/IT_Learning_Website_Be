using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum ETypeQuestion
{
    QuestionTrueFalse = 1,
    QuestionFourAnswer = 2,
    QuestionFiveAnswer = 3,
    QuestionMultiChoice = 4,
}