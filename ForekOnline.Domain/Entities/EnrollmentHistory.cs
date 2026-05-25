
// <copyright file="EnrollmentHistory.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    05/01/2025 11:26:27 PM
// Purpose:         Defines the EnrollmentHistory class

#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents the enrollment history of a student in a course.
    /// </summary>
    public class EnrollmentHistory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the enrollment record.
        /// </summary>
        [Key]
        public Guid EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the student.
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the title of the course.
        /// </summary>
        public string CourseTitle { get; set; }

        /// <summary>
        /// Gets or sets the type of the course.
        /// </summary>
        public string CourseType { get; set; }

        /// <summary>
        /// Gets or sets the current enrollment status of the student in the course.
        /// </summary>
        public string EnrollmentStatus { get; set; }

        /// <summary>
        /// Gets or sets the date when the student started the course.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the enrollment is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the date when the student completed the course.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DateCompleted { get; set; }
    }
}
