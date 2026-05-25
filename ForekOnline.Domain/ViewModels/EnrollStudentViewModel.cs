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
    public class EnrollStudentViewModel
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

        [MaxLength(10)]
        public eGender? Gender { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? Nationality { get; set; }

        [MaxLength(15)]
        public string? Cellphone { get; set; }

        [MaxLength(256)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [MaxLength(50)]
        [Display(Name = "Highest Grade")]
        public string? HighestGrade { get; set; }

        [MaxLength(200)]
        [Display(Name = "Name of School")]
        public string? NameOfSchool { get; set; }

        [MaxLength(250)]
        [Display(Name = "Street Address Line 1")]
        public string? StreetAddressLine1 { get; set; }

        [MaxLength(250)]
        [Display(Name = "Street Address Line 2")]
        public string? StreetAddressLine2 { get; set; }

        /// <summary>
        /// The course to enroll the student into.
        /// </summary>
        [Required]
        [Display(Name = "Course")]
        public Guid? CourseId { get; set; }

        #endregion

        #region Attachments
        [Display(Name = "ID / Passport Document")]
        public IFormFile? IDPassFile { get; set; }

        [Display(Name = "Highest Qualification")]
        public IFormFile? HighestQualFile { get; set; }

        [Display(Name = "Proof of Residence")]
        public IFormFile? ResidenceFile { get; set; }
        #endregion
    }
}