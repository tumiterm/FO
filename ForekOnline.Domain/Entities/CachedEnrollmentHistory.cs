// <copyright file="CachedEnrollmentHistory.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    14/03/2026 21:14 PM
// Purpose:         Defines the CachedEnrollmentHistory entity for SQLite API data caching.

#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a locally cached copy of an enrollment history record from the external API.
    /// </summary>
    public class CachedEnrollmentHistory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the enrollment.
        /// </summary>
        public Guid EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the cached unique identifier for the student.
        /// </summary>
        public Guid CachedStudentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the student.
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the course.
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
        /// Gets or sets the enrollment status for the entity.
        /// </summary>
        public string EnrollmentStatus { get; set; }

        /// <summary>
        /// Gets or sets the start date for the associated event or period.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the date on which the operation or task was completed.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DateCompleted { get; set; }
    }
}