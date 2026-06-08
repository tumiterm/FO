// <copyright file="Course.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the Course class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a course entity containing details about the course and its related modules.
    /// </summary>
    [SkipAuditInterceptor]
    public class Course : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the course.
        /// </summary>
        [Key]
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the name of the course.
        /// </summary>
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        /// <summary>
        /// Gets or sets the type of the course (e.g., Online, In-Person).
        /// </summary>
        [Display(Name = "Course Type")]
        public eCourseType Type { get; set; }

        /// <summary>
        /// Gets or sets the N-Type of the course, if applicable.
        /// </summary>
        public eNType? NType { get; set; }

        /// <summary>
        /// Gets or sets the credit value of the course.
        /// </summary>
        public double? Credit { get; set; }

        /// <summary>
        /// Gets or sets the NQF level of the course, if applicable.
        /// </summary>
        public eNQF? NQFLevel { get; set; }

        /// <summary>
        /// Gets or sets the list of modules associated with the course.
        /// </summary>
        public List<Module>? Module { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the course is eligible for online applications.
        /// </summary>
        public bool IsEligibleForOnlineApplications { get; set; } = true;

        /// <summary>
        /// Gets or sets the minimum requirements for enrolling in the course.
        /// </summary>
        public eMinimumRequirement MinimumRequirement { get; set; } = eMinimumRequirement.NoRequirement;

        [StringLength(1000)]
        [Display(Name = "Minimum Requirement Notes")]
        public string? MinimumRequirementNotes { get; set; }

        [Display(Name = "Duration Value")]
        public int? DurationValue { get; set; }

        [Display(Name = "Duration Type")]
        public eDurationType? DurationType { get; set; }

        [Display(Name = "Study Mode")]
        public eStudyMode StudyMode { get; set; } = eStudyMode.FullTime;

        [Display(Name = "Delivery Method")]
        public eDeliveryMethod DeliveryMethod { get; set; } = eDeliveryMethod.Contact;

        [Display(Name = "Is Accredited")]
        public bool IsAccredited { get; set; }

        [StringLength(200)]
        [Display(Name = "Accreditation Body")]
        public string? AccreditationBody { get; set; }

        [StringLength(100)]
        [Display(Name = "Accreditation Number")]
        public string? AccreditationNumber { get; set; }

        [Display(Name = "Requires Aptitude Test")]
        public bool RequiresAptitudeTest { get; set; }

        [Display(Name = "Requires Interview")]
        public bool RequiresInterview { get; set; }

        [Display(Name = "Application Fee")]
        public decimal? ApplicationFee { get; set; }

        [Display(Name = "Registration Fee")]
        public decimal? RegistrationFee { get; set; }

        [Display(Name = "Tuition Fee")]
        public decimal? TuitionFee { get; set; }

        [Display(Name = "Maximum Students")]
        public int? MaximumStudents { get; set; }

        [Display(Name = "Has Course Options")]
        public bool HasCourseOptions { get; set; }

        public List<CourseOption> CourseOptions { get; set; } = new();
    }
}
