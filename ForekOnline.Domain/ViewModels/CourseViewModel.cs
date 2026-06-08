// <copyright file="CourseViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 19:09:27 PM
// Purpose:         Defines the CourseViewModel class

#region Usings
using ForekOnline.Domain.Entities;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Data Transfer Object for Course details.
    /// </summary>
    /// <summary>
    /// Represents the view model for a course.
    /// </summary>
    public class CourseViewModel : Base/*, IMapFrom<Course>, IMapTo<Course>*/
    {
        /// <summary>
        /// Gets or sets the unique identifier for the course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the name of the course.
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// Gets or sets the type of the course.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the N-type classification of the course.
        /// </summary>
        public string? NType { get; set; }

        /// <summary>
        /// Gets or sets the credit value of the course.
        /// </summary>
        public double? Credit { get; set; }

        /// <summary>
        /// Gets or sets the NQF (National Qualifications Framework) level of the course.
        /// </summary>
        public string? NQFLevel { get; set; }

        /// <summary>
        /// Gets or sets the list of modules associated with the course.
        /// </summary>
        public List<ModuleViewModel> Modules { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the course is eligible for online applications.
        /// </summary>
        public bool IsEligibleForOnlineApplications { get; set; } = false;
        /// <summary>
        /// Gets or sets the minimum requirement for the course.
        /// </summary>  
        public eMinimumRequirement MinimumRequirement { get; set; }
        public string? MinimumRequirementNotes { get; set; }
        public int? DurationValue { get; set; }
        public eDurationType? DurationType { get; set; }
        public eStudyMode StudyMode { get; set; }
        public eDeliveryMethod DeliveryMethod { get; set; }
        public bool IsAccredited { get; set; }
        public string? AccreditationBody { get; set; }
        public string? AccreditationNumber { get; set; }
        public bool RequiresAptitudeTest { get; set; }
        public bool RequiresInterview { get; set; }
        public decimal? ApplicationFee { get; set; }
        public decimal? RegistrationFee { get; set; }
        public decimal? TuitionFee { get; set; }
        public int? MaximumStudents { get; set; }
        public bool HasCourseOptions { get; set; }
        public List<CourseOptionViewModel> CourseOptions { get; set; } = new();

    }

}
