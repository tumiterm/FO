using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Domain.ViewModels
{
    public class ForekOnlineStudentsSectionViewModel
    {
        public int Count { get; set; }
        public List<Student> Students { get; set; } = new();
    }
}
