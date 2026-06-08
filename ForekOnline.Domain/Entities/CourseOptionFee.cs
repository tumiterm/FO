using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.Entities
{
    public class CourseOptionFee : EntityBase<Guid>
    {
        public Guid CourseOptionIdFK { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Fee Description")]
        public string FeeDescription { get; set; } = string.Empty;

        [Display(Name = "Charge Type")]
        public eCourseChargeType ChargeType { get; set; } = eCourseChargeType.Fixed;

        public int? Days { get; set; }

        public decimal Amount { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        public CourseOption CourseOption { get; set; } = null!;
    }
}
