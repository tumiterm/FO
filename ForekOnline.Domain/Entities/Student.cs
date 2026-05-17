// <copyright file="Student.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 12:01:14 PM
// Purpose:         Defines the Student class

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a student with personal details, admission information, and enrollment history.
    /// </summary>
    public class Student
    {
        /// <summary>
        /// Gets or sets the unique identifier for the student.
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// Gets or sets the student number.
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the admission date of the student.
        /// </summary>
        public DateTime AdmissionDate { get; set; }

        /// <summary>
        /// Gets or sets the first name of the student.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the middle name of the student.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the student.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the ID number of the student.
        /// </summary>
        public string IDNumber { get; set; }

        /// <summary>
        /// Gets or sets the study permit number of the student.
        /// </summary>
        public string StudyPermitNumber { get; set; }

        /// <summary>
        /// Gets or sets the passport number of the student.
        /// </summary>
        public string PassportNumber { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the student.
        /// </summary>
        public DateTime DateofBirth { get; set; }

        /// <summary>
        /// Gets or sets the gender of the student.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the place of birth of the student.
        /// </summary>
        public string PlaceofBirth { get; set; }

        /// <summary>
        /// Gets or sets the nationality of the student.
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// Gets or sets the preferred language of the student.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the admission category of the student. The category is an object type that can be extended based on the admission system.
        /// </summary>
        public object AdmissionCategory { get; set; }

        /// <summary>
        /// Gets or sets the first line of the student's street address.
        /// </summary>
        public string StreetAddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the second line of the student's street address.
        /// </summary>
        public string StreetAddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the student's cellphone number.
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// Gets or sets the student's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the highest grade level the student has achieved.
        /// </summary>
        public string HighestGrade { get; set; }

        /// <summary>
        /// Gets or sets the name of the school the student attended before admission.
        /// </summary>
        public string NameofSchool { get; set; }

        /// <summary>
        /// Gets or sets whether the student is active or inactive.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets whether the student has been deregistered.
        /// </summary>
        public bool Deregistered { get; set; }

        /// <summary>
        /// Gets or sets the enrollment history of the student, representing past courses and their status.
        /// </summary>
        public List<EnrollmentHistory>? EnrollmentHistory { get; set; }

        /// <summary>
        /// Gets or sets the source from which the student was registered (e.g., online, in-person).
        /// </summary>
        public string RegistrationSource { get; set; }

        /// <summary>
        /// Gets or sets whether the student has been deregistrered.
        /// </summary>
        public bool Deregistrered { get; set; }  
    }

}



