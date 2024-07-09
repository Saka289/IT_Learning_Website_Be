using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LW.Shared.DTOs.QuizQuestion
{
    public class QuizQuestionImportDto : QuizQuestionDto
    {
        public List<String> Errors { get; set; }
        public bool IsImported { get; set; }
        public string? TypeName { get; set; }
        public string? QuestionLevelName { get; set; }
        public QuizQuestionImportDto()
        {
            Errors = new List<String>();
        }
    }
}
