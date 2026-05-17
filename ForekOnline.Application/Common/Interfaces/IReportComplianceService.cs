// <copyright file="IReportComplianceService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant
// Created Date:    18/04/2026
// Purpose:         Service interface for report submission checks and compliance calculation

#region Usings
using ForekOnline.Domain.Entities;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Handles report submission eligibility checks and compliance calculations.
    /// </summary>
    public interface IReportComplianceService
    {
        /// <summary>
        /// Checks whether a user can submit a report and determines if it would be late.
        /// Never hard-blocks — always returns an allowed result with context.
        /// </summary>
        /// <param name="idPass">The user's decrypted IDPass.</param>
        /// <param name="reportType">The type of report being submitted.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A rich result describing the submission status.</returns>
        Task<ReportSubmissionCheckResult> CheckSubmissionAsync(string idPass, ReportType reportType, CancellationToken ct = default);

        /// <summary>
        /// Calculates the compliance summary for a user across all report types.
        /// Looks back from the user's first report to now.
        /// </summary>
        /// <param name="idPass">The user's decrypted IDPass.</param>
        /// <param name="userName">Display name for the compliance record.</param>
        /// <param name="lookbackMonths">How many months to look back. Default 12.</param>
        /// <param name="ct">Cancellation token.</param>
        Task<UserReportCompliance> CalculateComplianceAsync(string idPass, string userName, int lookbackMonths = 12, CancellationToken ct = default);

        /// <summary>
        /// Stamps the late-submission fields on a report entity before saving.
        /// Call this after the user acknowledges the late submission warning.
        /// </summary>
        /// <param name="report">The report to stamp.</param>
        /// <param name="checkResult">The result from <see cref="CheckSubmissionAsync"/>.</param>
        void ApplyLateSubmissionFields(Report report, ReportSubmissionCheckResult checkResult);
    }
}