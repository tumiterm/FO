using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Domain.ViewModels
{
    public class SubmitAssessmentRequest
    {
        public Guid AttemptId { get; set; }
        public Guid AssessmentId { get; set; }
        public List<SubmittedAnswerItem> Answers { get; set; } = new();
        public bool ForcedAutoSubmit { get; set; }
    }
}
