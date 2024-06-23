using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LW.Shared.DTOs.Document
{
    public  class DocumentCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int GradeId { get; set; }
    }
}
