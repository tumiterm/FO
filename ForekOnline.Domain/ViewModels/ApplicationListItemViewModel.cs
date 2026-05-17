using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Domain.ViewModels
{
    public class ApplicationListItemViewModel
    {
        public Guid ApplicationId { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsNew { get; set; }
        public int? DaysOld { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
