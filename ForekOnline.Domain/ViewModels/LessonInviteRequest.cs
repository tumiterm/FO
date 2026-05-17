using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Domain.ViewModels
{
    public sealed class LessonInviteRequest
    {
        [Required]
        [StringLength(80)]
        public string RoomName { get; set; } = string.Empty;

        [Required]
        [StringLength(120)]
        public string Topic { get; set; } = string.Empty;

        [Required]
        public DateTime StartUtc { get; set; }

        [Required]
        public DateTime EndUtc { get; set; }

        [StringLength(50)]
        public string? Password { get; set; }

        [MinLength(1)]
        public List<string> AttendeeEmails { get; set; } = new();
    }
}
