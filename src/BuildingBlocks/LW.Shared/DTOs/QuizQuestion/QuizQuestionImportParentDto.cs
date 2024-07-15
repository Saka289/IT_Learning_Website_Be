using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LW.Shared.DTOs.QuizQuestion
{
    public class QuizQuestionImportParentDto
    {
        public int CountSuccess { get; set; }
        public int CountFail { get; set; }

        public string IdImport { get; set; }
        // save id with records fail 
        public string IdImportFail { get; set; }
        public string IdImportResult { get; set; }
  

        public List<QuizQuestionImportDto> QuizQuestionImportDtos { get; set; }
       

        public QuizQuestionImportParentDto()
        {
            QuizQuestionImportDtos = new List<QuizQuestionImportDto>();
        }
    }
}
