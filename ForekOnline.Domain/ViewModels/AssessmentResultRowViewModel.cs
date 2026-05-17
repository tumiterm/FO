using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Domain.ViewModels
{
    public class AssessmentResultRowViewModel
    {
        public Guid AttemptId { get; set; }
        public string LearnerIdPass { get; set; } = string.Empty;
        public string StudentDisplayName { get; set; } = string.Empty;

        public DateTime StartedUtc { get; set; }

        public DateTime? SubmittedUtc { get; set; }

        public string Status { get; set; } = string.Empty;

        public int? FinalScore { get; set; }

        public double? Percentage { get; set; }
    }
}
