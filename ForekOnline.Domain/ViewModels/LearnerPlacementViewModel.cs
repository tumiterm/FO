// <copyright file="LearnerPlacementViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    12/10/2025 12:45:27 PM
// Purpose:         Defines the LearnerPlacementViewModel class

#region Usings
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    public record LearnerPlacementViewModel 
    {
        /// <summary>
        /// Gets or sets the unique identifier for the placement.
        /// </summary>
        public Guid PlacementId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the company associated with the placement.
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the student who is placed at the company.
        /// </summary>
        public string Student { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the individual who placed the student at the company.
        /// </summary>
        public Guid PlacedBy { get; set; }

        /// <summary>
        /// Gets or sets the status of the placement (e.g., active, completed, canceled).
        /// </summary>
        public eStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the start date of the placement.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the placement.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the module associated with the placement, if applicable.
        /// </summary>
        [ForeignKey("Module")]
        public Guid? Module { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the course associated with the placement.
        /// </summary>
        [ForeignKey("Course")]
        public Guid? CourseId { get; set; }

        /// <summary>
        /// Gets or sets the duration of the placement in days, if applicable.
        /// </summary>
        public int? Duration { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier assigned to a student.
        /// </summary>
        public string StudentNumber { get; set; }   

        /// <summary>
        /// Gets or sets the student ID associated with the placement. This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public Guid? StudentId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to send an SMS notification regarding the placement.
        /// This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        [Display(Name = "Send SMS?")]
        public bool SendNotification { get; set; }

        /// <summary>
        /// Gets or sets the collection of companies.
        /// </summary>
        public IReadOnlyCollection<Company> Companys { get; set; } = new List<Company>();

        /// <summary>
        /// Gets or sets the collection of users.
        /// </summary>
        public IReadOnlyCollection<User> Users { get; set; } = new List<User>();
        public bool IsActive { get; set; }

        [NotMapped]
        public DateTime CreatedOn { get; set; }

        [NotMapped]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the workplace mentor name responsible for learner supervision.
        /// </summary>
        [Required]
        [Display(Name = "Workplace Mentor Name")]
        public string? WorkplaceMentorName { get; set; }

        /// <summary>
        /// Gets or sets the workplace mentor email address.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Workplace Mentor Email")]
        public string? WorkplaceMentorEmail { get; set; }

        /// <summary>
        /// Gets or sets the workplace mentor phone number.
        /// </summary>
        [Required]
        [Display(Name = "Workplace Mentor Phone")]
        public string? WorkplaceMentorPhone { get; set; }

        /// <summary>
        /// Gets or sets the saved placement agreement file name.
        /// </summary>
        public string? PlacementAgreement { get; set; }

        /// <summary>
        /// Gets or sets the uploaded placement agreement PDF.
        /// </summary>
        [NotMapped]
        [Display(Name = "Placement Agreement PDF")]
        public IFormFile? PlacementAgreementFile { get; set; }

        /// <summary>
        /// Gets or sets the signer confirmation for the placement agreement.
        /// </summary>
        [Display(Name = "Digital Signature")]
        public string? DigitalSignature { get; set; }
    }
}
