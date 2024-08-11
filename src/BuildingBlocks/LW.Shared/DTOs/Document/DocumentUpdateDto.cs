using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LW.Shared.Enums;

namespace LW.Shared.DTOs.Document
{
    public class DocumentUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        
        //public string Code { get; set; }
        public EBookCollection BookCollection { get; set; } //1-KetNoiTriThuc 2-CanhDieu 3-ChanTroiSangTao
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public int Edition { get; set; }
        public EBookType TypeOfBook { get; set; } // 1- SGK 2-SBT 3-SGV 4-TLTK 
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int GradeId { get; set; }
        public IEnumerable<string> tagValues { get; set; }
    }
}
