using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum EBookType
{
    [Display(Name = "Sách giáo khoa")] 
    SGK = 1,
    [Display(Name = "Sách bài tập")] 
    SBT = 2,
    [Display(Name = "Sách giáo viên")] 
    SGV = 3,
    [Display(Name = "Tài liệu tham khảo")] 
    TLTK = 4
}