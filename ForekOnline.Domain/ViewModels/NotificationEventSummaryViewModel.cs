using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Domain.ViewModels
{
    public sealed class NotificationEventSummaryViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string? HeaderIconCss { get; set; }
        public string? HeaderGradientCss { get; set; }
        public string? HeaderTextColor { get; set; }
        public string SizeClass { get; set; } = "";
        public string? ImageUrl { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public IEnumerable<NotificationContentBlockViewModel> Blocks { get; set; } = Enumerable.Empty<NotificationContentBlockViewModel>();
    }
}
