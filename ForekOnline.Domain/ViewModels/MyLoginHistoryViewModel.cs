// <copyright file="MyLoginHistoryViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    07/04/2026 10:00 AM
// Purpose:         Self-service login history view model for current user

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// View model for the "My Login History" self-service page.
    /// Filtered to the current authenticated user only.
    /// </summary>
    public class MyLoginHistoryViewModel
    {
        /// <summary>
        /// Gets or sets the number of currently active sessions for this user.
        /// </summary>
        public int ActiveSessionCount { get; set; }

        /// <summary>
        /// Gets or sets the total login count in the last 30 days.
        /// </summary>
        public int TotalLogins30Days { get; set; }

        /// <summary>
        /// Gets or sets the average session duration over the last 30 days.
        /// </summary>
        public TimeSpan AvgDuration { get; set; }

        /// <summary>
        /// Gets or sets the user's display name.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the session rows for this user (last 90 days).
        /// </summary>
        public IReadOnlyList<LoginHistoryRowViewModel> Rows { get; set; } = Array.Empty<LoginHistoryRowViewModel>();
    }
}