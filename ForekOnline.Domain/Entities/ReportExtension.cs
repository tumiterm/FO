// <copyright file="ReportExtensions.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    18/04/2026
// Purpose:         Extends the Report entity with late-submission tracking fields

#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Extends <see cref="Report"/> with late-submission and compliance tracking fields.
    /// These fields are added via a partial class to keep the migration diff clean.
    /// </summary>
    public partial class Report
    {
        /// <summary>
        /// Gets or sets whether this report was submitted after the normal submission window
        /// (i.e. the current period already had a report of this type).
        /// </summary>
        public bool IsLateSubmission { get; set; }

        /// <summary>
        /// Gets or sets the start date of the reporting period this report is intended to cover.
        /// For on-time submissions this matches the current period.
        /// For late submissions this is the missed period (e.g. the previous month).
        /// </summary>
        public DateTime? IntendedPeriodStart { get; set; }

        /// <summary>
        /// Gets or sets the end date of the reporting period this report covers.
        /// </summary>
        public DateTime? IntendedPeriodEnd { get; set; }

        /// <summary>
        /// Gets or sets a human-readable label for the intended period 
        /// (e.g. "March 2026", "Week 14 of 2026", "2025").
        /// </summary>
        [StringLength(60)]
        public string? IntendedPeriodLabel { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the user acknowledged the late submission warning.
        /// Null for on-time submissions.
        /// </summary>
        public DateTime? LateAcknowledgedUtc { get; set; }
    }
}