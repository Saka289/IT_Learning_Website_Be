using LW.Shared.DTOs.QuizAnswer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LW.Shared.DTOs.QuizQuestion
{
    public class QuizQuestionExcelDto 
    {
        public string? TypeName { get; set; }
        public string Content { get; set; }
        public IEnumerable<QuizAnswerDto>? QuizAnswers { get; set; }
        public string? Correct { get; set; }
        public string? QuestionLevelName { get; set; }
        public bool IsShuffle { get; set; }
        public string? Hint { get; set; }
        public List<String> Errors { get; set; }
    }
}
