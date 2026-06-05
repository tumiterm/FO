// <copyright file="FoEnrollment.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/03/2026 00:00 AM
// Purpose:         Defines the ForekOnline-owned Enrollment entity (SQL Server)

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a course enrollment for a <see cref="StudentEntity"/>.
    /// </summary>
    [Table("EnrollmentHistory", Schema = "Academics")]
    public class EnrollmentEntity : EntityBase<Guid>
    {
        [Key]
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [MaxLength(200)]
        public string? CourseTitle { get; set; }

        [MaxLength(50)]
        public string? CourseType { get; set; }

        /// <summary>
        /// Active | Completed | Dropped Out | Suspended. Use <see cref="global::ForekOnline.Domain.Entities.EnrollmentStatus.Normalize"/> for external values.
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string EnrollmentStatus { get; set; } = global::ForekOnline.Domain.Entities.EnrollmentStatus.Active;

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateCompleted { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Navigation — the student this enrollment belongs to.
        /// </summary>
        [ForeignKey(nameof(StudentId))]
        public StudentEntity Student { get; set; } = null!;
    }
}