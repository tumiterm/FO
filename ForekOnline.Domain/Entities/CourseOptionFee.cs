// <copyright file="CourseOptionFee.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    10/06/2026 14:09:27 PM
// Purpose:         Defines the CourseOptionFee class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a fee associated with a specific course option.
    /// This entity captures details about the fee, including its description,
    /// </summary>
    public class CourseOptionFee : EntityBase<Guid>
    {
        /// <summary>
        /// Foreign key referencing the associated course option. This establishes a relationship between the
        /// fee and the course option it belongs to.
        /// </summary>
        public Guid CourseOptionIdFK { get; set; }

        /// <summary>
        /// A descriptive name for the fee. This field is required and has a maximum length of 200 characters.  
        /// </summary>
        [Required, StringLength(200)]
        [Display(Name = "Fee Description")]
        public string FeeDescription { get; set; } = string.Empty;

        /// <summary>
        /// Indicates the type of charge for the fee. This is an enumeration that categorizes the fee,
        /// such as whether it is a fixed amount or a percentage of the course price.
        /// </summary>
        [Display(Name = "Charge Type")]
        public eCourseChargeType ChargeType { get; set; } = eCourseChargeType.Fixed;

        /// <summary>
        /// The number of days associated with the fee, if applicable. This can be used to specify
        /// a duration or deadline related to the fee.
        /// </summary>
        public int? Days { get; set; }

        /// <summary>
        /// The amount of the fee. This value can represent either a fixed amount or a percentage,
        /// depending on the ChargeType.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The total amount for the fee, which may be calculated based on the Amount and ChargeType.
        /// </summary>
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Navigation property to the associated course option. This allows for easy access to the course option details from the fee.
        /// </summary>
        public CourseOption CourseOption { get; set; } = null!;
    }
}
