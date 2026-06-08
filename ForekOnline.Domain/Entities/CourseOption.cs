
#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    public class CourseOption : EntityBase<Guid>
    {
        public Guid CourseIdFK { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Option Description")]
        public string OptionDescription { get; set; } = string.Empty;

        [Display(Name = "Option Type")]
        public eCourseOptionType OptionType { get; set; } = eCourseOptionType.Custom;

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        public Course Course { get; set; } = null!;

        public List<CourseOptionFee> Fees { get; set; } = new();
    }
}
