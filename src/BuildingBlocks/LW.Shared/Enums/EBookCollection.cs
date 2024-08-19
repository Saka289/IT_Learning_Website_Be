using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum EBookCollection
{
    [Display(Name = "Kết Nối Tri Thức")]
    KetNoiTriThuc = 1,
    [Display(Name = "Cánh Diều")]
    CanhDieu = 2,
    [Display(Name = "Chân Trời Sáng Tạo")]
    ChanTroiSangTao = 3
}