// <copyright file="LessonAttendance.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/02/2025 15:55 PM
// Purpose:         Defines the LessonAttendance model

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
#endregion
namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a record of a student's attendance for a specific lesson.
    /// </summary>
    /// <remarks>This class captures information about when a student joined and left a lesson, as well as the
    /// total duration of their attendance. It is typically used to track participation and calculate attendance metrics
    /// in educational applications.</remarks>
    [SkipAuditInterceptor]
    public class LessonAttendance
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the lesson.
        /// </summary>
        [Required]
        public Guid LessonId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the student.
        /// </summary>
        [Required]
        public Guid StudentId { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the user joined.
        /// </summary>
        public DateTime? JoinedUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the user left.
        /// </summary>
        public DateTime? LeftUtc { get; set; }

        /// <summary>
        /// Gets or sets the duration of the operation, in seconds.
        /// </summary>
        public int DurationSeconds { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the most recent event occurred.
        /// </summary>
        public DateTime LastEventUtc { get; set; }
    }
}
