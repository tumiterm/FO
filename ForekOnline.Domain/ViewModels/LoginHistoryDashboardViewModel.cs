// <copyright file="LoginHistoryDashboardViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    07/04/2026 10:00 AM
// Purpose:         Dashboard view model for login history

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Aggregated dashboard view model for the login history feature.
    /// Contains KPI data, filter state, and the session rows for display.
    /// </summary>
    public class LoginHistoryDashboardViewModel
    {
        #region KPI Properties

        /// <summary>
        /// Gets or sets the total number of sessions (filtered or all-time).
        /// </summary>
        public int TotalSessions { get; set; }

        /// <summary>
        /// Gets or sets the count of currently active sessions.
        /// </summary>
        public int ActiveNow { get; set; }

        /// <summary>
        /// Gets or sets the count of completed (closed) sessions.
        /// </summary>
        public int ClosedCount { get; set; }

        /// <summary>
        /// Gets or sets the count of abnormal or force-closed sessions.
        /// </summary>
        public int AbnormalOrForcedCount { get; set; }

        /// <summary>
        /// Gets or sets the average session duration for today.
        /// </summary>
        public TimeSpan AvgDurationToday { get; set; }

        /// <summary>
        /// Gets or sets the average session duration for yesterday.
        /// </summary>
        public TimeSpan AvgDurationYesterday { get; set; }

        /// <summary>
        /// Gets or sets the percentage change in average duration (today vs yesterday).
        /// Positive = increase, negative = decrease.
        /// </summary>
        public double AvgDurationChangePercent { get; set; }

        /// <summary>
        /// Gets or sets the peak number of concurrent sessions recorded today.
        /// </summary>
        public int PeakConcurrentToday { get; set; }

        /// <summary>
        /// Gets or sets the number of new logins in the last 5 minutes (for the live ticker).
        /// </summary>
        public int RecentLoginsLast5Min { get; set; }

        #endregion

        #region Filter State

        /// <summary>
        /// Gets or sets the maximum number of rows to return.
        /// </summary>
        public int Take { get; set; } = 200;

        /// <summary>
        /// Gets or sets the currently active quick-filter chip.
        /// Values: "all", "active", "abnormal", "thisweek", "last7", "last30".
        /// </summary>
        public string ActiveFilter { get; set; } = "all";

        /// <summary>
        /// Gets or sets the From date filter.
        /// </summary>
        public DateOnly? From { get; set; }

        /// <summary>
        /// Gets or sets the To date filter.
        /// </summary>
        public DateOnly? To { get; set; }

        #endregion

        #region Data

        /// <summary>
        /// Gets or sets the session rows for display in the table.
        /// </summary>
        public IReadOnlyList<LoginHistoryRowViewModel> Rows { get; set; } = Array.Empty<LoginHistoryRowViewModel>();

        #endregion
    }
}