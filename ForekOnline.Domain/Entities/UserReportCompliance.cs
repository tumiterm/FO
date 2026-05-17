// <copyright file="UserReportCompliance.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    18/04/2026
// Purpose:         Represents a user's report compliance summary

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents the compliance summary for a user's report submissions.
    /// Calculated on-the-fly from report data — not persisted.
    /// Scoped to the current calendar year, offset by the grace period.
    /// </summary>
    public sealed class UserReportCompliance
    {
        /// <summary>
        /// Gets the user's IDPass.
        /// </summary>
        public string IdPass { get; init; } = string.Empty;

        /// <summary>
        /// Gets the user's display name.
        /// </summary>
        public string UserDisplayName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the overall compliance status.
        /// </summary>
        public eComplianceStatus Status { get; init; }

        /// <summary>
        /// Gets the compliance percentage (0–100). 
        /// Calculated as: (OnTimeCount / TotalExpected) * 100.
        /// </summary>
        public double CompliancePercentage { get; init; }

        /// <summary>
        /// Gets the total number of reports submitted on time.
        /// </summary>
        public int OnTimeCount { get; init; }

        /// <summary>
        /// Gets the total number of reports submitted late.
        /// </summary>
        public int LateCount { get; init; }

        /// <summary>
        /// Gets the total number of expected reporting periods that were missed entirely (no report at all).
        /// </summary>
        public int MissedCount { get; init; }

        /// <summary>
        /// Gets the total number of reports expected based on the user's active period.
        /// </summary>
        public int TotalExpected { get; init; }

        /// <summary>
        /// Gets the total number of reports actually submitted (on time + late).
        /// </summary>
        public int TotalSubmitted { get; init; }

        /// <summary>
        /// Gets the breakdown per report type.
        /// </summary>
        public IReadOnlyList<ReportTypeCompliance> ByType { get; init; } = [];

        /// <summary>
        /// Gets the calendar year this compliance snapshot is scoped to.
        /// </summary>
        public int ComplianceYear { get; init; }

        /// <summary>
        /// Gets the number of grace weeks skipped at the start of the year.
        /// </summary>
        public int GraceWeeks { get; init; }

        /// <summary>
        /// Gets the effective compliance start date (after grace period).
        /// </summary>
        public DateTime ComplianceStartDate { get; init; }

        /// <summary>
        /// Gets missed periods grouped by report type.
        /// </summary>
        public IReadOnlyDictionary<ReportType, IReadOnlyList<MissedPeriod>> MissedPeriodsByType { get; init; } = new Dictionary<ReportType, IReadOnlyList<MissedPeriod>>();
    }
}