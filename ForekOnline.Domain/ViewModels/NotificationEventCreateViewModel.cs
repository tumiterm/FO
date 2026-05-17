using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    public  class NotificationEventCreateViewModel
    {
        [Required, MaxLength(180)]
        public string Title { get; set; } = "";

        public string? HeaderIconCss { get; set; }
        public string? HeaderGradientCss { get; set; }
        public string? HeaderTextColor { get; set; }

        public eNotificationModalSize ModalSize { get; set; } = eNotificationModalSize.Large;
        public string? ImageUrl { get; set; }

        [Required]
        public DateTime StartUtc { get; set; }
        [Required]
        public DateTime EndUtc { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public string? AudienceRole { get; set; }
        public string? CarouselGroupKey { get; set; }

        public List<NotificationContentBlockViewModel> Blocks { get; set; } = new();

    }
}
