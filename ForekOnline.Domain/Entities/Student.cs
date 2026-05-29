// <copyright file="Student.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 12:01:14 PM
// Purpose:         Defines the Student class

#region Usings
using ForekOnline.Domain.Shared;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion
namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a student with personal details, admission information, and enrollment history.
    /// </summary>
    public class Student : EntityBase<Guid>
    {
        #region Identity 
        public Guid StudentId { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
        #endregion

        #region Personal Details
        public DateTime DateOfBirth { get; set; }
        public int Age => DateTime.Today.Year - DateOfBirth.Year
                          - (DateTime.Today < DateOfBirth.AddYears(DateTime.Today.Year - DateOfBirth.Year) ? 1 : 0);
        public eGender Gender { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string? Language { get; set; }
        public bool? HasDisability { get; set; }    
        public string? Disability { get; set; }
        #endregion

        #region Identification
        public string? IDNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? StudyPermitNumber { get; set; }
        public DateTime? StudyPermitExpiry { get; set; }

        #endregion

        #region  Contact 
        public string? Email { get; set; }
        public string? Cellphone { get; set; }
        public string? AlternativePhone { get; set; }

        #endregion

        #region Address 
        public string? StreetAddressLine1 { get; set; }
        public string? StreetAddressLine2 { get; set; }
        public string? City { get; set; }                   
        public eProvince? Province { get; set; }               
        public string? PostalCode { get; set; }             
        public string? Country { get; set; }
        #endregion

        #region Admission 
        public DateTime AdmissionDate { get; set; }
        public eAdmissionCategory AdmissionCategory { get; set; }
        public string RegistrationSource { get; set; } = string.Empty;
        public string? HighestGrade { get; set; }
        public string? NameOfSchool { get; set; }

        #endregion

        #region Academic Status
        public bool IsActive { get; set; }
        public bool Deregistered { get; set; }             
        public DateTime? DeregistrationDate { get; set; }  
        public string? DeregistrationReason { get; set; }

        #endregion

        #region Placement & Enrollment
        public Guid? PlacementId { get; set; }
        public Placement? Placement { get; set; }
        public List<EnrollmentHistory>? EnrollmentHistory { get; set; }

        #endregion

        #region Guardian
        public Guardian? Guardian { get; set; }
        public Guid GuardianId { get; set; }
        #endregion

        #region Documents
        public List<StudentDocument>? Documents { get; set; }
        public bool HasUploadedID =>
            Documents?.Any(d => d.DocumentType == eStudentDocumentType.NationalID
                             || d.DocumentType == eStudentDocumentType.Passport) ?? false;

        public bool IsDocumentationComplete =>
            HasUploadedID &&
            (Documents?.Any(d => d.DocumentType == eStudentDocumentType.HighestQualification) ?? false) &&
            (Documents?.Any(d => d.DocumentType == eStudentDocumentType.StudyPermit) ?? false);

        #endregion

        #region Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;   
        public DateTime? UpdatedAt { get; set; }                      
        public string? CreatedBy { get; set; }                        
        public string? UpdatedBy { get; set; }
        #endregion

    }

}



