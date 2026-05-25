// <copyright file="CachedStudent.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    14/03/2026 00:00 AM
// Purpose:         Defines the CachedStudent entity for SQLite API data caching.

using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a locally cached copy of a student record from the external API.
    /// Stored in SQLite for offline resilience.
    /// </summary>
    public class CachedStudent
    {
        /// <summary>
        /// Gets or sets the unique identifier for the student.
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier assigned to the student by the institution.
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the date when the admission occurred.
        /// </summary>
        public DateTime AdmissionDate { get; set; }

        /// <summary>
        /// Gets or sets the first name of the person.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the middle name of the person.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the person.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the identification number associated with the entity.
        /// </summary>
        public string IDNumber { get; set; }

        /// <summary>
        /// Gets or sets the study permit number associated with the individual.
        /// </summary>
        public string StudyPermitNumber { get; set; }

        /// <summary>
        /// Gets or sets the passport number associated with the individual.
        /// </summary>
        public string PassportNumber { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the individual.
        /// </summary>
        public DateTime DateofBirth { get; set; }

        /// <summary>
        /// Gets or sets the gender of the individual.
        /// </summary>
        public eGender Gender { get; set; }

        /// <summary>
        /// Gets or sets the place of birth of the individual.
        /// </summary>
        public string PlaceofBirth { get; set; }

        /// <summary>
        /// Gets or sets the nationality of the individual.
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// Gets or sets the primary language of the individual.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the admission category of the individual.
        /// </summary>
        public eAdmissionCategory AdmissionCategory { get; set; }

        /// <summary>
        /// Gets or sets the first line of the street address of the individual.
        /// </summary>
        public string StreetAddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the second line of the street address of the individual.
        /// </summary>
        public string StreetAddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the cellphone number of the individual.
        /// </summary>  
        public string Cellphone { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the highest grade achieved.
        /// </summary>
        public string HighestGrade { get; set; }

        /// <summary>
        /// Gets or sets the name of the school.
        /// </summary>
        public string NameofSchool { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deregistered.
        /// </summary>
        public bool Deregistered { get; set; }

        /// <summary>
        /// Gets or sets the source from which the registration was initiated.
        /// </summary>
        public string RegistrationSource { get; set; }
        public bool Deregistrered { get; set; }

        /// <summary>
        /// Gets or sets the cached enrollment history records.
        /// </summary>
        public List<CachedEnrollmentHistory> EnrollmentHistory { get; set; } = new();

        /// <summary>
        /// The UTC timestamp when this record was last synced from the API.
        /// </summary>
        public DateTime LastSyncedUtc { get; set; }
    }
}