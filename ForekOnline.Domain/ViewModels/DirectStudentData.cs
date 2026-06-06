// <copyright file="DirectStudentData.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Serializable student data captured by the direct enrollment form.
    /// </summary>
    public class DirectStudentData
    {
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? IDNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? StudyPermitNumber { get; set; }
        public DateTime? StudyPermitExpiry { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public eGender? Gender { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? Language { get; set; }
        public bool? HasDisability { get; set; }
        public string? Disability { get; set; }
        public string? Cellphone { get; set; }
        public string? AlternativePhone { get; set; }
        public string? Email { get; set; }
        public string? HighestGrade { get; set; }
        public string? NameOfSchool { get; set; }
        public string? StreetAddressLine1 { get; set; }
        public string? StreetAddressLine2 { get; set; }
        public string? City { get; set; }
        public eProvince? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public eAdmissionCategory AdmissionCategory { get; set; }
        public List<EnrollmentDocumentData> Documents { get; set; } = new();
    }
}
