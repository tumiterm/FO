// <copyright file="EnrollStudentViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/03/2026 00:00 AM
// Purpose:         ViewModel for the Enroll Student view

#region Usings
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// ViewModel for the student enrollment form.
    /// Pre-populated from Application if approved, blank for walk-ins.
    /// </summary>
    public class EnrollStudentViewModel : IValidatableObject
    {
        #region Page State

        /// <summary>
        /// Whether the initial ID/Passport lookup has been completed.
        /// </summary>
        public bool IsLookupComplete { get; set; }

        /// <summary>
        /// Whether this enrollment originated from an approved Application.
        /// </summary>
        public bool IsFromApplication { get; set; }

        /// <summary>
        /// Success message shown after enrollment submission.
        /// </summary>
        public string? SuccessMessage { get; set; }

        /// <summary>
        /// Available courses for the dropdown.
        /// </summary>
        public List<SelectListItem> AvailableCourses { get; set; } = new();

        #endregion

        #region Form Data

        /// <summary>
        /// The original Application ID (if pre-populated from an approved applicant).
        /// </summary>
        public Guid? OriginalApplicationId { get; set; }

        [MaxLength(13)]
        [Display(Name = "ID Number")]
        public string? IDNumber { get; set; }

        [MaxLength(20)]
        [Display(Name = "Passport Number")]
        public string? PassportNumber { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        public eGender? Gender { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? PlaceOfBirth { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Nationality { get; set; }

        [MaxLength(100)]
        public string? Language { get; set; }

        [Display(Name = "Has a disability")]
        public bool? HasDisability { get; set; }

        [MaxLength(250)]
        [Display(Name = "Disability details")]
        public string? Disability { get; set; }

        [MaxLength(30)]
        [Display(Name = "Study Permit Number")]
        public string? StudyPermitNumber { get; set; }

        [Display(Name = "Study Permit Expiry")]
        public DateTime? StudyPermitExpiry { get; set; }

        [Required]
        [MaxLength(15)]
        public string? Cellphone { get; set; }

        [MaxLength(15)]
        [Display(Name = "Alternative Phone")]
        public string? AlternativePhone { get; set; }

        [MaxLength(256)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Highest Grade")]
        public string? HighestGrade { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Name of School")]
        public string? NameOfSchool { get; set; }

        [MaxLength(250)]
        [Display(Name = "Street Address Line 1")]
        public string? StreetAddressLine1 { get; set; }

        [MaxLength(250)]
        [Display(Name = "Street Address Line 2")]
        public string? StreetAddressLine2 { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public eProvince? Province { get; set; }

        [MaxLength(10)]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [Required]
        [Display(Name = "Admission Category")]
        public eAdmissionCategory? AdmissionCategory { get; set; }

        /// <summary>
        /// The course to enroll the student into.
        /// </summary>
        [Required]
        [Display(Name = "Course")]
        public Guid? CourseId { get; set; }

        #endregion

        #region Attachments
        [Required]
        [Display(Name = "ID / Passport Document")]
        public IFormFile? IDPassFile { get; set; }

        [Required]
        [Display(Name = "Highest Qualification")]
        public IFormFile? HighestQualFile { get; set; }

        [Display(Name = "Proof of Residence")]
        public IFormFile? ResidenceFile { get; set; }

        [Display(Name = "Study Permit Document")]
        public IFormFile? StudyPermitFile { get; set; }

        [Display(Name = "Disability Supporting Document")]
        public IFormFile? DisabilityFile { get; set; }
        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(IDNumber) && string.IsNullOrWhiteSpace(PassportNumber))
            {
                yield return new ValidationResult(
                    "Provide either an ID Number or a Passport Number.",
                    new[] { nameof(IDNumber), nameof(PassportNumber) });
            }

            if (DateOfBirth is null || DateOfBirth.Value.Date >= DateTime.Today)
            {
                yield return new ValidationResult(
                    "Enter a valid date of birth in the past.",
                    new[] { nameof(DateOfBirth) });
            }

            if (!string.IsNullOrWhiteSpace(PassportNumber))
            {
                if (string.IsNullOrWhiteSpace(StudyPermitNumber))
                {
                    yield return new ValidationResult(
                        "A study permit number is required for passport holders.",
                        new[] { nameof(StudyPermitNumber) });
                }

                if (StudyPermitExpiry is null || StudyPermitExpiry.Value.Date <= DateTime.Today)
                {
                    yield return new ValidationResult(
                        "Enter a study permit expiry date in the future.",
                        new[] { nameof(StudyPermitExpiry) });
                }

                if (StudyPermitFile is null || StudyPermitFile.Length == 0)
                {
                    yield return new ValidationResult(
                        "Upload the study permit document for a passport holder.",
                        new[] { nameof(StudyPermitFile) });
                }
            }

            if (HasDisability == true && string.IsNullOrWhiteSpace(Disability))
            {
                yield return new ValidationResult(
                    "Provide disability details when disability is selected.",
                    new[] { nameof(Disability) });
            }

            foreach (var file in GetUploadedFiles())
            {
                if (file.Length > 10 * 1024 * 1024)
                {
                    yield return new ValidationResult(
                        $"{file.FileName} exceeds the 10 MB file limit.",
                        new[] { nameof(IDPassFile), nameof(HighestQualFile), nameof(ResidenceFile), nameof(StudyPermitFile), nameof(DisabilityFile) });
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension is not (".pdf" or ".jpg" or ".jpeg" or ".png"))
                {
                    yield return new ValidationResult(
                        $"{file.FileName} must be a PDF, JPG, or PNG file.",
                        new[] { nameof(IDPassFile), nameof(HighestQualFile), nameof(ResidenceFile), nameof(StudyPermitFile), nameof(DisabilityFile) });
                }
            }
        }

        private IEnumerable<IFormFile> GetUploadedFiles()
        {
            var files = new[] { IDPassFile, HighestQualFile, ResidenceFile, StudyPermitFile, DisabilityFile };
            return files.Where(file => file is not null && file.Length > 0).Cast<IFormFile>();
        }
    }
}
