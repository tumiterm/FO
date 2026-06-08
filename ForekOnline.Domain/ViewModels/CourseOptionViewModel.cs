using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    public class CourseOptionViewModel
    {
        public Guid CourseOptionId { get; set; }
        public Guid CourseId { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Option Description")]
        public string OptionDescription { get; set; } = string.Empty;

        [Display(Name = "Option Type")]
        public eCourseOptionType OptionType { get; set; } = eCourseOptionType.Custom;

        public decimal TotalAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public List<CourseOptionFeeViewModel> Fees { get; set; } = new();
    }

    public class CourseOptionFeeViewModel
    {
        public Guid CourseOptionFeeId { get; set; }
        public Guid CourseOptionId { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Fee Description")]
        public string FeeDescription { get; set; } = string.Empty;

        [Display(Name = "Charge Type")]
        public eCourseChargeType ChargeType { get; set; } = eCourseChargeType.Fixed;

        [Range(1, int.MaxValue)]
        public int? Days { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal Amount { get; set; }

        public decimal TotalAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
    }
}
