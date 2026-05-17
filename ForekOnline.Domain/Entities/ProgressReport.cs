
// <copyright file="ProgressReport.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the ProgressReport class

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
    /// Represents a progress report for a student, including the course, report details, and attached files.
    /// </summary>
    [SkipAuditInterceptor]
    public class ProgressReport : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the report.
        /// </summary>
        [Key]
        public Guid ReportId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the student associated with the report.
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// Gets or sets the student number associated with the progress report.
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the course associated with the progress report.
        /// </summary>
        public eTrade Course { get; set; }

        /// <summary>
        /// Gets or sets the name of the progress report.
        /// </summary>
        [Display(Name = "Report Name")]
        public string ReportName { get; set; }

        /// <summary>
        /// Gets or sets the file path or reference for the attached report, if any.
        /// </summary>
        public string? AttachReport { get; set; }

        /// <summary>
        /// Gets or sets the file for uploading the report. This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? ReportFile { get; set; }
    }

}
