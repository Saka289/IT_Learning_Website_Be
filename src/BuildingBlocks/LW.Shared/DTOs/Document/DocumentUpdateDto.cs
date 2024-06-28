using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LW.Shared.DTOs.Document
{
    public class DocumentUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Code { get; set; }

        public string BookCollection { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public int Edition { get; set; }
        public string TypeOfBook { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int GradeId { get; set; }
    }
}
