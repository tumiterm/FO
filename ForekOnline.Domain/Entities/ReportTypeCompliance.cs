// <copyright file="ReportTypeCompliance.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    18/04/2026
// Purpose:         Represents the report type compliance breakdown for a specific report type, showing counts of on-time, late, missed, and expected reports.

#region Usings
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Compliance breakdown for a specific report type.
    /// </summary>
    public sealed class ReportTypeCompliance
    {
        public ReportType ReportType { get; init; }
        public int OnTime { get; init; }
        public int Late { get; init; }
        public int Missed { get; init; }
        public int Expected { get; init; }
    }
}
