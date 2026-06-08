using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.Entities
{
    /// <summary>Represents a selectable commercial or delivery option for a course.</summary>
    [SkipAuditInterceptor]
    public class CourseOption : Base
    {
        [Key]
        public Guid CourseOptionId { get; set; }

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
