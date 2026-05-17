using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Domain.ViewModels
{
    public class AssessmentResultsViewModel
    {
        public Guid AssessmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<AssessmentResultRowViewModel> Rows { get; set; } = new();
    }
}
