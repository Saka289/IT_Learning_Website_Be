using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum EQuestionLevel
{
    [Display(Name = "Cấp độ rất dễ")]
    VeryLow = 1,     
    [Display(Name = "Cấp độ dễ")]
    Low = 2,         
    [Display(Name = "Cấp độ trung bình")]
    Medium = 3,      
    [Display(Name = "Cấp độ khó")]
    High = 4
}