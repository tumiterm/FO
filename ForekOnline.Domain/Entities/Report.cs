
// <copyright file="Report.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 12:01:14 PM
// Purpose:         Defines the Report class

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
    /// Represents a report entity containing details about various activities, challenges, and recommendations.
    /// </summary>
    [SkipAuditInterceptor]
    public partial class Report : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the report.
        /// </summary>
        [Key]
        public Guid ReportId { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier associated with the report.
        /// </summary>
        public string? Reference { get; set; }

        /// <summary>
        /// Gets or sets the ID or passport number related to the report.
        /// </summary>
        public string? IdPass { get; set; }

        /// <summary>
        /// Gets or sets the type of the report.
        /// </summary>
        [Display(Name = "Report Type")]
        public ReportType ReportType { get; set; }

        /// <summary>
        /// Gets or sets the date of the report.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets the module associated with the report.
        /// </summary>
        public string? Module { get; set; }

        /// <summary>
        /// Gets or sets the activity report description.
        /// </summary>
        [Display(Name = "Activity Report")]
        public string? ActivityReport { get; set; }

        /// <summary>
        /// Gets or sets the challenges encountered related to the report.
        /// </summary>
        public string? Challenges { get; set; }

        /// <summary>
        /// Gets or sets the facilitator's unique identifier.
        /// </summary>
        public Guid? FacilitatorId { get; set; }

        /// <summary>
        /// Gets or sets recommendations based on the report findings.
        /// </summary>
        public string? Recommendation { get; set; }

        /// <summary>
        /// Gets or sets the urgency level of the report.
        /// </summary>
        public eUrgency? Urgency { get; set; }

        /// <summary>
        /// Gets or sets the operation type related to the report.
        /// </summary>
        public eOperation Operation { get; set; }

        /// <summary>
        /// Gets or sets the document associated with the report.
        /// </summary>
        public string? Document { get; set; }

        /// <summary>
        /// Gets or sets the report url.
        /// </summary>
        public string? ReportURL { get; set; }

        /// <summary>
        /// Gets or sets IsRead.
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Gets or sets the OpenCount.
        /// </summary>
        public int OpenCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the LastOpened date.
        /// </summary>
        public DateTime? LastOpened { get; set; }

        /// <summary>
        /// Gets or sets the HasExpired.
        /// </summary>
        public bool HasExpired { get; set; }

        /// <summary>
        /// Gets or sets the ExpiryDate.
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets IsLiked - indicative of whether a report is liked or not.
        /// </summary>
        public bool? IsLiked { get; set; }

        /// <summary>
        /// Gets or sets the LastDownloaded date.
        /// </summary>
        public DateTime? LastDownloaded { get; set; }

        /// <summary>
        /// Gets or sets the DownloadCount.
        /// </summary>
        public int DownloadCount { get; set; } = 0;


        /// <summary>
        /// Gets or sets the uploaded document file. This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? DocumentFile { get; set; }

    }
}
