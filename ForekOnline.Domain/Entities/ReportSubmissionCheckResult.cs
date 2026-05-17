// <copyright file="ReportSubmissionCheckResult.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    18/04/2026
// Purpose:         Encapsulates the outcome of a report submission eligibility check

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents the result of checking whether a user is eligible to submit a report,
    /// including whether the submission would be considered late and which period it covers.
    /// </summary>
    public sealed class ReportSubmissionCheckResult
    {
        /// <summary>
        /// Gets whether the submission is allowed (always true now — we never hard-block).
        /// </summary>
        public bool IsAllowed { get; init; } = true;

        /// <summary>
        /// Gets whether the current period already has a report of this type,
        /// meaning this submission would be an additional (late) one.
        /// </summary>
        public bool CurrentPeriodAlreadyCovered { get; init; }

        /// <summary>
        /// Gets whether this submission would be flagged as a late report.
        /// True when the user is submitting for a period that has already passed.
        /// </summary>
        public bool WouldBeLate { get; init; }

        /// <summary>
        /// Gets whether there are missed periods the user could submit a late report for,
        /// even if the current period is not yet covered.
        /// </summary>
        public bool HasMissedPeriods { get; init; }

        /// <summary>
        /// Gets the start date of the period this report would cover.
        /// For a normal submission this is the current period start.
        /// For a late submission this is the most recent uncovered period.
        /// </summary>
        public DateTime IntendedPeriodStart { get; init; }

        /// <summary>
        /// Gets the end date of the period this report would cover.
        /// </summary>
        public DateTime IntendedPeriodEnd { get; init; }

        /// <summary>
        /// Gets a human-readable label for the intended period (e.g. "March 2026", "Week 14, 2026").
        /// </summary>
        public string IntendedPeriodLabel { get; init; } = string.Empty;

        /// <summary>
        /// Gets the report type being checked.
        /// </summary>
        public ReportType ReportType { get; init; }

        /// <summary>
        /// Gets a user-facing message explaining the submission status.
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Gets the total number of late reports this user has for this report type.
        /// Used for compliance display.
        /// </summary>
        public int ExistingLateCount { get; init; }

        /// <summary>
        /// Gets the list of periods that are missing a report (gaps).
        /// Useful for showing the user exactly which periods they missed.
        /// </summary>
        public IReadOnlyList<MissedPeriod> MissedPeriods { get; init; } = [];
    }
}