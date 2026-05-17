// <copyright file="FoStudent.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/03/2026 00:00 AM
// Purpose:         Defines the ForekOnline-owned Student entity (SQL Server)

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a fully enrolled student owned by ForekOnline (SQL Server).
    /// This replaces the external API <see cref="Student"/> once the API is switched off.
    /// </summary>
    [Table("Students", Schema = "Academics")]
    public class StudentEntity : EntityBase<Guid>
    {
        /// <summary>
        /// Institution-assigned student number (e.g. FIT-2026-001).
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string StudentNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? MiddleName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(13)]
        public string? IDNumber { get; set; }

        [MaxLength(20)]
        public string? PassportNumber { get; set; }

        [MaxLength(20)]
        public string? StudyPermitNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(100)]
        public string? PlaceOfBirth { get; set; }

        [MaxLength(100)]
        public string? Nationality { get; set; }

        [MaxLength(50)]
        public string? Language { get; set; }

        [MaxLength(50)]
        public string? AdmissionCategory { get; set; }

        [MaxLength(250)]
        public string? StreetAddressLine1 { get; set; }

        [MaxLength(250)]
        public string? StreetAddressLine2 { get; set; }

        [MaxLength(15)]
        public string? Cellphone { get; set; }

        [MaxLength(256)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? HighestGrade { get; set; }

        [MaxLength(200)]
        public string? NameOfSchool { get; set; }

        public DateTime AdmissionDate { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeregistered { get; set; }

        /// <summary>
        /// Where this record came from: "API", "SQLite", "WalkIn", "Application".
        /// </summary>
        [MaxLength(30)]
        public string RegistrationSource { get; set; } = "WalkIn";

        /// <summary>
        /// Links back to the original Application if the student was an approved applicant.
        /// </summary>
        public Guid? OriginalApplicationId { get; set; }

        public string IdPassportDocument { get; set; } = string.Empty;

        /// <summary>
        /// Navigation — enrollments for this student.
        /// </summary>
        public List<EnrollmentEntity> Enrollments { get; set; } = new();
    }
}