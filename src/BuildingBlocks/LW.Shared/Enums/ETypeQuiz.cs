using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum ETypeQuiz
{
    [Display(Name = "Luyện tập")]
    Practice = 1,
    [Display(Name = "Kiểm tra")]
    Test = 2
}