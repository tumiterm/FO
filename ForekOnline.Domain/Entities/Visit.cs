// <copyright file="Visit.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 12:01:14 PM
// Purpose:         Defines the Visit class

#region Usings
using ForekOnline.Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a visit record to a company, including associated feedback, reports, and the visit details.
    /// </summary>
    [SkipAuditInterceptor]
    public class Visit : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the visit record.
        /// </summary>
        [Key]
        public Guid VisitId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the company associated with the visit.
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Gets or sets an array of selected IDs (not mapped to the database) representing specific records or entities.
        /// </summary>
        [NotMapped]
        public string[] SelectedIDArray { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the visit has an associated report.
        /// </summary>
        public bool HasReport { get; set; }

        /// <summary>
        /// Gets or sets the report associated with the visit.
        /// </summary>
        public string? Report { get; set; }

        /// <summary>
        /// Gets or sets the date of the visit.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the IDs of selected employees for the visit, stored as a string.
        /// </summary>
        public string? SelectedEmployeeIDs { get; set; }

        /// <summary>
        /// Gets or sets the learner feedback provided during the visit.
        /// </summary>
        public string? LearnerFeedback { get; set; }

        /// <summary>
        /// Gets or sets the purpose of the visit.
        /// </summary>
        public string? VisitPurpose { get; set; }

        /// <summary>
        /// Gets or sets the name of the mentor associated with the visit.
        /// </summary>
        public string? Mentor { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user who conducted the visit.
        /// </summary>
        public Guid VisitBy { get; set; }

        /// <summary>
        /// Gets or sets the report file associated with the visit (not mapped to the database).
        /// </summary>
        [NotMapped]
        public IFormFile? ReportFile { get; set; }
    }

}
