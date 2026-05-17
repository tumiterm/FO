// <copyright file="EnrollmentHistoryViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the EnrollmentHistoryViewModel class

using System.ComponentModel.DataAnnotations;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the enrollment history of a student.
    /// </summary>
    public class EnrollmentHistoryViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the enrollment record.
        /// </summary>
        public Guid EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the student associated with the enrollment.
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the course associated with the enrollment.
        /// </summary>
        public Guid? CourseId { get; set; }

        /// <summary>
        /// Gets or sets the title of the course.
        /// </summary>
        [Display(Name = "Course")]
        public string CourseTitle { get; set; }

        /// <summary>
        /// Gets or sets the type of the course.
        /// </summary>
        [Display(Name = "Course Type")]
        public string CourseType { get; set; }

        /// <summary>
        /// Gets or sets the enrollment status of the student.
        /// </summary>
        [Display(Name = "Enrollment Status")]
        public string EnrollmentStatus { get; set; }

        /// <summary>
        /// Gets or sets the start date of the enrollment.
        /// </summary>
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the enrollment is currently active.
        /// </summary>
        public bool IsActive { get; set; }
    }

}
