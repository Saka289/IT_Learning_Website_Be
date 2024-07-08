using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum EQuestionLevel
{
    [Display(Name = "Cấp độ rất thấp")]
    VeryLow = 1,     
                     
    [Display(Name = "Cấp độ thấp")]
    Low = 2,         
    [Display(Name = "Cấp độ trung bình")]
    Medium = 3,      
                     
    [Display(Name = "Cấp độ cao")]
    High = 4
}