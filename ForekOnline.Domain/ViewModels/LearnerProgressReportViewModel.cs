using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Domain.ViewModels
{
    public class LearnerProgressReportViewModel
    {
        public string StudentNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;

        public List<LearnerAssessmentResultViewModel> Assessments { get; set; } = new();

        // Aggregates
        public int TotalAssessments => Assessments.Count;
        public int CompletedAssessments => Assessments.Count(a => a.HasScore);
        public decimal CompletionPercent => TotalAssessments == 0 ? 0 : Math.Round((decimal)CompletedAssessments / TotalAssessments * 100, 2);
        public decimal AveragePercent => Assessments.Any(a => a.HasScore)
            ? Math.Round(Assessments.Where(a => a.HasScore).Average(a => a.Percent ?? 0), 2)
            : 0;

        public IDictionary<string, decimal> AverageByModule =>
            Assessments
                .Where(a => a.HasScore)
                .GroupBy(a => a.Module)
                .OrderBy(g => g.Key)
                .ToDictionary(
                    g => g.Key,
                    g => Math.Round(g.Average(x => x.Percent ?? 0), 2)
                );
    }
}


    

