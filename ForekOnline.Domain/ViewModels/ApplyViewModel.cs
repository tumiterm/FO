// <copyright file="ApplyViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the ApplyViewModel class

#region Usings
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for an application submission, including applicant details, address, funding, and supporting documents.
    /// </summary>
    public class ApplyViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the applicant.
        /// </summary>
        public Guid ApplicantId { get; set; }

        /// <summary>
        /// Gets or sets the reference number associated with the application.
        /// This property is optional.
        /// </summary>
        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the applicant's selection choice.
        /// </summary>
        public eChoice Selection { get; set; }

        /// <summary>
        /// Gets or sets the applicant's passport number, if applicable.
        /// </summary>
        public string? PassportNumber { get; set; }

        /// <summary>
        /// Gets or sets the category of the applicant's study permit.
        /// </summary>
        public eCategory? StudyPermitCategory { get; set; }

        /// <summary>
        /// Gets or sets the applicant's identification number, if applicable.
        /// </summary>
        public string? IDNumber { get; set; }

        /// <summary>
        /// Gets or sets the applicant's email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the applicant's cellphone number.
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the applicant's address.
        /// </summary>
        public Guid AddressId { get; set; }


        /// <summary>
        /// Gets or sets the ID Document URL
        /// </summary>
        [ValidateNever]
        public string? IDPassFileUrl { get; set; }

        /// <summary>
        /// Gets or sets the Qualification Document URL
        /// </summary>
        [ValidateNever]
        public string? HighestQualFileUrl { get; set; }

        /// <summary>
        /// Gets or sets the Residence Document URL
        /// </summary>
        [ValidateNever]
        public string? ResidenceFileUrl { get; set; }

        /// <summary>
        /// Gets or sets the street name of the applicant's address.
        /// This property is optional.
        /// </summary>
        public string? StreetName { get; set; }

        /// <summary>
        /// Gets or sets additional address details (Line 1).
        /// This property is optional.
        /// </summary>
        public string? Line1 { get; set; }

        /// <summary>
        /// Gets or sets the city of the applicant's residence.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the province of the applicant's residence.
        /// </summary>
        public eProvince? Province { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the applicant's residence.
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for address association.
        /// </summary>
        public Guid AddressAssociativeId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the applicant.
        /// </summary>
        public string ApplicantName { get; set; }

        /// <summary>
        /// Gets or sets the surname of the applicant.
        /// </summary>
        public string ApplicantSurname { get; set; }

        /// <summary>
        /// Gets or sets the funding type for the application.
        /// </summary>
        public eFunderType FunderType { get; set; }

        /// <summary>
        /// Gets or sets the reason for the current application status, if applicable.
        /// </summary>
        public string? StatusReason { get; set; }

        /// <summary>
        /// Gets or sets the title of the applicant (e.g., Mr, Ms, Dr).
        /// </summary>
        public eTitle ApplicantTitle { get; set; }

        /// <summary>
        /// Gets or sets the gender of the applicant.
        /// </summary>
        public eGender Gender { get; set; }

        /// <summary>
        /// Gets or sets the highest qualification attained by the applicant.
        /// </summary>
        public HighestQualification HighestQualification { get; set; }

        /// <summary>
        /// Gets or sets the file path or reference to the applicant's ID or passport document.
        /// </summary>
        public string? IDPassDoc { get; set; }

        /// <summary>
        /// Gets or sets the file path or reference to the applicant's highest qualification document.
        /// </summary>
        public string? HighestQualDoc { get; set; }

        /// <summary>
        /// Gets or sets the file path or reference to the applicant's residence document.
        /// </summary>
        public string? ResidenceDoc { get; set; }

        /// <summary>
        /// Gets or sets the current application status.
        /// </summary>
        public ApplicationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the applicant's guardian.
        /// </summary>
        public Guid GuardianId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier linking the guardian to the application.
        /// </summary>
        public Guid GuardianApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the applicant's guardian.
        /// </summary>
        public string GuardianFirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the applicant's guardian.
        /// </summary>
        public string GuardianLastName { get; set; }

        /// <summary>
        /// Gets or sets the relationship between the applicant and the guardian.
        /// </summary>
        public eRelationship GuardianRelationship { get; set; }

        /// <summary>
        /// Gets or sets the cellphone number of the applicant's guardian.
        /// </summary>
        public string GuardianCellphone { get; set; }

        /// <summary>
        /// Gets or sets the file path or reference to the guardian's identification document.
        /// </summary>
        public string? GuardianIDDoc { get; set; }

        /// <summary>
        /// Gets or sets the name of the applicant's medical aid provider, if applicable.
        /// </summary>
        public string? MedicalName { get; set; }

        /// <summary>
        /// Gets or sets the applicant's medical aid member number, if applicable.
        /// </summary>
        public string? MemberNumber { get; set; }

        /// <summary>
        /// Gets or sets the telephone number of the medical aid provider, if applicable.
        /// </summary>
        public string? Telephone { get; set; }

        /// <summary>
        /// Gets or sets information about any disability the applicant has, if applicable.
        /// </summary>
        public string? Disability { get; set; }

        /// <summary>
        /// Gets or sets the guardian assigned to the applicant.
        /// This property is ignored by validation.
        /// </summary>
        [ValidateNever]
        public Guardian ApplicantGuardian { get; set; }

        /// <summary>
        /// Gets or sets the course ID for which the applicant is applying.
        /// </summary>
        public Guid CourseId { get; set; }

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
        /// Gets or sets the uploaded guardian ID document file.
        /// This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? GuardianIDFile { get; set; }

        /// <summary>
        /// Gets or sets the uploaded applicant's ID or passport document file.
        /// This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? ApplicantIDPassFile { get; set; }

        /// <summary>
        /// Gets or sets the uploaded highest qualification document file.
        /// This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? AplicantHighestQualFile { get; set; }

        /// <summary>
        /// Gets or sets the uploaded residence proof document file.
        /// This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? ApplicantResidenceFile { get; set; }
    }

}
