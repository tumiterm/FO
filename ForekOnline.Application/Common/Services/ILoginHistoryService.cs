// <copyright file="ILoginHistoryService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    07/04/2026 10:00 AM
// Purpose:         Defines the ILoginHistoryService interface

#region Usings
using ForekOnline.Domain.ViewModels;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Defines methods for managing and querying user login history, including dashboard KPIs,
    /// filtered session retrieval, and force-logout operations.
    /// </summary>
    public interface ILoginHistoryService
    {
        /// <summary>
        /// Builds the full dashboard view model including KPIs and filtered session rows.
        /// </summary>
        /// <param name="take">Maximum number of rows to return (capped at 2000).</param>
        /// <param name="filter">Quick-filter chip: "all", "active", "abnormal", "thisweek", "last7", "last30".</param>
        /// <param name="from">Optional start date filter.</param>
        /// <param name="to">Optional end date filter.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A fully populated <see cref="LoginHistoryDashboardViewModel"/>.</returns>
        Task<LoginHistoryDashboardViewModel> GetDashboardAsync(int take = 200, string filter = "all", DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Builds the "My Login History" view model for a specific user.
        /// </summary>
        /// <param name="userId">The authenticated user's ID.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="MyLoginHistoryViewModel"/> for the specified user.</returns>
        Task<MyLoginHistoryViewModel> GetMyLoginHistoryAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Force-closes an active session identified by the given session key.
        /// </summary>
        /// <param name="sessionKey">The unique session key to close.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>True if the session was found and closed; false otherwise.</returns>
        Task<bool> ForceLogoutSessionAsync(string sessionKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Force-closes multiple active sessions (bulk action).
        /// </summary>
        /// <param name="sessionKeys">Collection of session keys to close.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The number of sessions successfully closed.</returns>
        Task<int> BulkForceLogoutAsync(IEnumerable<string> sessionKeys, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the count of new logins in the last N minutes (for live ticker / AJAX polling).
        /// </summary>
        /// <param name="minutes">Look-back window in minutes.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The count of logins within the time window.</returns>
        Task<int> GetRecentLoginCountAsync(int minutes = 5, CancellationToken cancellationToken = default);

        /// <summary>
        /// Exports all login history rows matching the filter to a flat list suitable for CSV/JSON export.
        /// </summary>
        /// <param name="from">Optional start date.</param>
        /// <param name="to">Optional end date.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="LoginHistoryRowViewModel"/> for export.</returns>
        Task<IReadOnlyList<LoginHistoryRowViewModel>> GetExportDataAsync(DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);
    }
}