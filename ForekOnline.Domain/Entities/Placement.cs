// <copyright file="Placement.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the Placement class

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
    /// Represents a placement record for a student at a company, including details such as placement status, duration, and associated course/module.
    /// </summary>
    [SkipAuditInterceptor]
    public class Placement : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the placement.
        /// </summary>
        public Guid PlacementId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the company associated with the placement.
        /// </summary>
        public Guid CompanyId { get; set; }

        public Company? Company { get; set; }

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
        /// Gets or sets the student ID associated with the placement. This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public Guid? StudentId { get; set; }

        public Student Students { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to send an SMS notification regarding the placement.
        /// This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        [Display(Name = "Send SMS?")]
        public bool SendNotification { get; set; }

        /// <summary>
        /// Gets or sets the workplace mentor name responsible for day-to-day learner supervision.
        /// </summary>
        public string? WorkplaceMentorName { get; set; }

        /// <summary>
        /// Gets or sets the workplace mentor email address used for onboarding and approvals.
        /// </summary>
        public string? WorkplaceMentorEmail { get; set; }

        /// <summary>
        /// Gets or sets the workplace mentor phone number used for urgent placement communication.
        /// </summary>
        public string? WorkplaceMentorPhone { get; set; }

        /// <summary>
        /// Gets or sets the stored placement agreement file name.
        /// </summary>
        public string? PlacementAgreement { get; set; }

        /// <summary>
        /// Gets or sets the captured digital signature text or signer confirmation.
        /// </summary>
        public string? DigitalSignature { get; set; }

        /// <summary>
        /// Gets or sets the uploaded placement agreement file.
        /// </summary>
        [NotMapped]
        public IFormFile? PlacementAgreementFile { get; set; }

        /// <summary>
        /// Gets or sets the weekly timesheets submitted against this placement.
        /// </summary>
        public ICollection<WeeklyTimesheet> WeeklyTimesheets { get; set; } = new List<WeeklyTimesheet>();

        /// <summary>
        /// Gets or sets the integrated campus visit history for this placement.
        /// </summary>
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();

        public ICollection<LearnerWorkplaceModules> LearnerWorkplaceModules { get;set; }
    }

}
