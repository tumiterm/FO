// <copyright file="CourseOption.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    10/06/2026 14:09:27 PM
// Purpose:         Defines the CourseOption class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents an option for a course, which may include additional fees and details.
    /// </summary>
    public class CourseOption : EntityBase<Guid>
    {
        /// <summary>
        /// Foreign key referencing the associated course. This establishes a relationship between the course option and the course it belongs to.
        /// </summary>
        public Guid CourseIdFK { get; set; }

        /// <summary>
        /// A descriptive name for the course option. This field is required and has a maximum length of 200 characters.
        /// It provides a clear and concise description of the option being offered,
        /// which can help students understand what they are selecting when enrolling in a course.
        /// </summary>
        [Required, StringLength(200)]
        [Display(Name = "Option Description")]
        public string OptionDescription { get; set; } = string.Empty;

        /// <summary>
        /// Indicates the type of course option. This is an enumeration that categorizes the option,
        /// such as whether it is a standard option, a custom option, or any other defined types.
        /// </summary>
        [Display(Name = "Option Type")]
        public eCourseOptionType OptionType { get; set; } = eCourseOptionType.Custom;

        /// <summary>
        /// The total amount for the course option, which may include the base price of the course plus any
        /// additional fees associated with this option.
        /// </summary>
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Navigation property to the associated course. This allows for easy access to the course details from the course option.
        /// </summary>
        public Course Course { get; set; } = null!;

        /// <summary>
        /// A collection of fees associated with this course option. Each fee represents an additional cost that may be incurred
        /// </summary>
        public List<CourseOptionFee> Fees { get; set; } = new();
    }
}
