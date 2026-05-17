// <copyright file="Placement.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the Placement class

#region Usings
using ForekOnline.Domain.Shared;
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

        /// <summary>
        /// Gets or sets a value indicating whether to send an SMS notification regarding the placement.
        /// This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        [Display(Name = "Send SMS?")]
        public bool SendNotification { get; set; }
    }

}
