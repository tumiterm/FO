// <copyright file="Application.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    06/01/2024 12:09:08 PM
// Purpose:         Defines the Application class

#region Usings
using ForekOnline.Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents an application for enrollment or registration.
    /// </summary>
    [SkipAuditInterceptor]
    public class Application
    {
        /// <summary>
        /// Gets or sets the unique identifier for the application.
        /// </summary>
        [Key]
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the reference number for the application.
        /// </summary>
        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the selection choice of the applicant.
        /// </summary>
        public eChoice Selection { get; set; }

        /// <summary>
        /// Gets or sets the passport number of the applicant, if applicable.
        /// </summary>
        public string? PassportNumber { get; set; }

        /// <summary>
        /// Gets or sets the study permit category of the applicant.
        /// </summary>
        public eCategory StudyPermitCategory { get; set; }

        /// <summary>
        /// Gets or sets the South African ID number of the applicant.
        /// </summary>
        public string? IDNumber { get; set; }

        /// <summary>
        /// Gets or sets the email address of the applicant.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the cellphone number of the applicant.
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// Gets or sets the address of the applicant.
        /// </summary>
        public Address ApplicantAddress { get; set; }

        /// <summary>
        /// Gets or sets the first name of the applicant.
        /// </summary>
        public string ApplicantName { get; set;}

        /// <summary>
        /// Gets or sets the surname of the applicant.
        /// </summary>
        public string ApplicantSurname { get; set;}

        /// <summary>
        /// Gets or sets the title of the applicant (e.g., Mr, Ms, Dr).
        /// </summary>
        public eTitle ApplicantTitle { get; set; }

        /// <summary>
        /// Gets or sets the ID Document URL
        /// </summary>
        public string IDPassFileUrl { get; set; }

        /// <summary>
        /// Gets or sets the Qualification Document URL
        /// </summary>
        public string HighestQualFileUrl { get; set; }

        /// <summary>
        /// Gets or sets the Residence Document URL
        /// </summary>
        public string ResidenceFileUrl { get; set; }

        /// <summary>
        /// Gets or sets the gender of the applicant.
        /// </summary>
        public eGender Gender { get; set; }

        /// <summary>
        /// Gets or sets the guardian details of the applicant.
        /// </summary>
        public Guardian ApplicantGuardian { get; set; }

        /// <summary>
        /// Gets or sets the highest qualification of the applicant.
        /// </summary>
        public HighestQualification HighestQualification { get; set; }

        /// <summary>
        /// Gets or sets the type of funder supporting the applicant.
        /// </summary>
        public eFunderType FunderType { get; set; }

        /// <summary>
        /// Gets or sets the reason for the application status, if applicable.
        /// </summary>
        public string? StatusReason { get; set; }

        /// <summary>
        /// Gets or sets the document path for the applicant's ID or passport.
        /// </summary>
        public string? IDPassDoc { get; set; }

        /// <summary>
        /// Gets or sets the document path for the applicant's highest qualification.
        /// </summary>
        public string? HighestQualDoc { get; set; }

        /// <summary>
        /// Gets or sets the document path for the applicant's residence proof.
        /// </summary>
        public string? ResidenceDoc { get; set; }

        /// <summary>
        /// Gets or sets the current status of the application.
        /// </summary>
        public ApplicationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the reason for the application status, if applicable.
        /// </summary>
        public string? PendingStatusReason { get; set; }

        /// <summary>
        /// Gets or sets an optional user-facing status message (e.g., shown to the applicant).
        /// </summary>
        [MaxLength(250)]
        public string? PendingStatusMessage { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the course applied for.
        /// </summary>
        [ForeignKey(nameof(Course))]   
        public Guid CourseId { get; set; }

        /// <summary>
        /// Gets or sets the uploaded file for ID or passport (not mapped to the database).
        /// </summary>
        [NotMapped]
        public IFormFile? IDPassFile { get; set; }

        /// <summary>
        /// Gets or sets the uploaded file for highest qualification (not mapped to the database).
        /// </summary>
        [NotMapped]
        public IFormFile? HighestQualFile { get; set; }

        /// <summary>
        /// Gets or sets the uploaded file for residence proof (not mapped to the database).
        /// </summary>
        [NotMapped]
        public IFormFile? ResidenceFile { get;set; }

    }
}
