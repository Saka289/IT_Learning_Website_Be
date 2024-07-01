using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LW.Shared.DTOs.Document
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string KeyWord { get; set; }
        public string Description { get; set; }
        public string BookCollection { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public int Edition { get; set; }
        public string TypeOfBook { get; set; }
        public bool IsActive { get; set; }
        public int GradeId { get; set; }
        public double? AverageRating { get; set; }
        public string GradeTitle { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
    }
}