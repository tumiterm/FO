using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    public class LearnerAssessmentResultViewModel
    {
        public Guid Id { get; set; }
        public string Module { get; set; } = string.Empty;
        public eAssessmentAdministration AssessmentType { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal? Score { get; set; }
        public decimal? MaxScore { get; set; }
        public bool HasScore => Score.HasValue && MaxScore.HasValue && MaxScore > 0;
        public decimal? Percent => HasScore ? Math.Round((Score!.Value / MaxScore!.Value) * 100m, 2) : null;
        public string Status => HasScore ? "Completed" : "Pending";
    }
}
