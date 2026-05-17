// <copyright file="ReportSubmissionViewModel.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-03-2026 21:37 PM
// Purpose:         Defines the ReportSubmissionViewModel.

#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the data required to submit a report, including the main report details and any associated
    /// sub-reports.
    /// </summary>
    /// <remarks>This view model is typically used to transfer report submission data between the user
    /// interface and the application logic. It contains the primary report information as well as a collection of
    /// sub-report uploads, allowing for complex report submissions involving multiple related documents.</remarks>
    public sealed class ReportSubmissionViewModel
    {
        /// <summary>
        /// Gets or sets the report data associated with this instance.
        /// </summary>
        [Required]
        public Entities.Report Report { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of sub-report upload rows associated with this report.
        /// </summary>
        public List<SubReportUploadRow> SubReports { get; set; } = new();
    }
}
