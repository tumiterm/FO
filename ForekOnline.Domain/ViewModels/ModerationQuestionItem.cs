using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    public class ModerationQuestionItem
    {
        public Guid QuestionId { get; set; }
        public int DisplayOrder { get; set; }
        public eAssessmentQuestionType QuestionType { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public string? ImagePath { get; set; }
        public bool EnableAnnotation { get; set; }
        public int Marks { get; set; }
        public List<ModerationOptionItem> Options { get; set; } = new();

        public string QuestionTypeLabel => QuestionType switch
        {
            eAssessmentQuestionType.MultipleChoice => "Multiple Choice",
            eAssessmentQuestionType.ShortAnswer => "Short Answer",
            eAssessmentQuestionType.MathInput => "Math Input",
            _ => QuestionType.ToString()
        };
    }
}
