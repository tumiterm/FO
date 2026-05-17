using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Domain.ViewModels
{
    public sealed class NotificationEventUpdateViewModel : NotificationEventCreateViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}
