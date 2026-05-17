// <copyright file="MissedPeriod.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    18/04/2026
// Purpose:         Encapsulates the MissedPeriod for reporting periods that were missed (no report submitted).

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a reporting period that was missed (no report submitted).
    /// </summary>
    public sealed class MissedPeriod
    {
        /// <summary>
        /// Gets the start of the missed period.
        /// </summary>
        public DateTime PeriodStart { get; init; }

        /// <summary>
        /// Gets the end of the missed period.
        /// </summary>
        public DateTime PeriodEnd { get; init; }

        /// <summary>
        /// Gets the human-readable label (e.g. "February 2026").
        /// </summary>
        public string Label { get; init; } = string.Empty;
    }
}
