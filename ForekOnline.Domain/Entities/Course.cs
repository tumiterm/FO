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

        /// <summary>
        /// Gets or sets additional notes regarding the minimum requirements for the course. 
        /// This field can provide more detailed information about the requirements, such as specific qualifications,
        /// experience, or other criteria that applicants must meet to be eligible for enrollment.
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "Minimum Requirement Notes")]
        public string? MinimumRequirementNotes { get; set; }

        /// <summary>
        /// Gets or sets the duration value of the course. This represents the length of time required to complete the course,
        /// </summary>
        [Display(Name = "Duration Value")]
        public int? DurationValue { get; set; }

        /// <summary>
        /// Gets or sets the duration type of the course, which indicates the unit of time for the duration value (e.g., weeks, months, years).
        /// </summary>
        [Display(Name = "Duration Type")]
        public eDurationType? DurationType { get; set; }

        /// <summary>
        /// Gets or sets the study mode of the course, which indicates whether the course is offered as full-time,
        /// part-time, or any other defined study modes.
        /// </summary>
        [Display(Name = "Study Mode")]
        public eStudyMode StudyMode { get; set; } = eStudyMode.FullTime;

        /// <summary>
        /// Gets or sets the delivery method of the course, which indicates how the course is delivered to students (e.g., contact, online, blended).
        /// </summary>
        [Display(Name = "Delivery Method")]
        public eDeliveryMethod DeliveryMethod { get; set; } = eDeliveryMethod.Contact;

        /// <summary>
        /// Gets or sets a value indicating whether the course is accredited. Accreditation indicates that the course has
        /// been evaluated and meets certain standards of quality and rigor, which can enhance the credibility and recognition 
        /// of the course for students and employers.
        /// </summary>
        [Display(Name = "Is Accredited")]
        public bool IsAccredited { get; set; }

        /// <summary>
        /// Gets or sets the name of the accreditation body that has accredited the course. This field provides information about   
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Accreditation Body")]
        public string? AccreditationBody { get; set; }

        /// <summary>
        /// Gets or sets the accreditation number associated with the course. This unique identifier can be used to verify the
        /// accreditation status of the course
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Accreditation Number")]
        public string? AccreditationNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the course requires an aptitude test as part of the admission process.
        /// </summary>
        [Display(Name = "Requires Aptitude Test")]
        public bool RequiresAptitudeTest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the course requires an interview as part of the admission process.
        /// An interview can be used to assess the suitability of applicants for the course, allowing for a more personalized evaluation
        /// of their qualifications and motivations.
        /// </summary>
        [Display(Name = "Requires Interview")]
        public bool RequiresInterview { get; set; }

        /// <summary>
        /// Gets or sets the application fee for the course. This fee is typically required to be paid by applicants when submitting
        /// their applications for enrollment in the course.
        /// </summary>
        [Display(Name = "Application Fee")]
        public decimal? ApplicationFee { get; set; }

        /// <summary>
        /// Gets or sets the registration fee for the course. This fee is typically required to be paid by students upon successful
        /// </summary>
        [Display(Name = "Registration Fee")]
        public decimal? RegistrationFee { get; set; }

        /// <summary>
        /// Gets or sets the tuition fee for the course. This fee represents the cost of enrolling in and attending the course, and it may vary based on factors such as the duration,
        /// delivery method, and accreditation status of the course.
        /// </summary>
        [Display(Name = "Tuition Fee")]
        public decimal? TuitionFee { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of students that can enroll in the course. This field can be used to manage class sizes and ensure that
        /// the course does not exceed its capacity,
        /// </summary>
        [Display(Name = "Maximum Students")]
        public int? MaximumStudents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the course has additional options available for students to choose from. If true, students may have the opportunity to select from various course options that can enhance their learning
        /// experience or provide additional benefits.
        /// </summary>
        [Display(Name = "Has Course Options")]
        public bool HasCourseOptions { get; set; }

        /// <summary>
        /// Gets or sets a collection of course options associated with the course. Each course option represents an alternative or additional offering related to the course, which may include different fees, durations, or other features that can provide students with
        /// more choices when enrolling in the course.
        /// </summary>
        public List<CourseOption> CourseOptions { get; set; } = new();
    }
}
