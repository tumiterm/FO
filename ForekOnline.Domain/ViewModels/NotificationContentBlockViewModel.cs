using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    public sealed class NotificationContentBlockViewModel
    {
        public Guid? Id { get; set; } // null for create
        [Required]
        public eNotificationContentType Type { get; set; }

        public string? Text { get; set; }
        public string?[]? ListItems { get; set; }
        public string? TableJson { get; set; }
        public string? ImageUrl { get; set; }
        public string? AltText { get; set; }
        public int Order { get; set; }

    }
}
