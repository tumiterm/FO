// <copyright file="LoginHistoryRowViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    26/01/2026 22:04:27 PM
// Purpose:         Defines the LoginHistoryRowViewModel class

#region Usings
using ForekOnline.Domain.Entities;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a view model for a single user login history entry, containing user and session details for display
    /// purposes.
    /// </summary>
    public class LoginHistoryRowViewModel
    {
        /// <summary>
        /// Gets the user login history associated with the current session.
        /// </summary>
        public UserLoginHistory Session { get; init; } = default!;

        /// <summary>
        /// Gets the unique identifier for the user.
        /// </summary>
        public Guid UserId { get; init; }

        /// <summary>
        /// Gets the display name associated with the user.
        /// </summary>
        public string DisplayName { get; init; } = "Unknown User";

        /// <summary>
        /// Gets the email address associated with the entity.
        /// </summary>
        public string? Email { get; init; }

        /// <summary>
        /// Gets the role assigned to the user, or null if no role is specified.
        /// </summary>
        public eSysRole? Role { get; init; }

        /// <summary>
        /// Gets the CSS class for the duration colour indicator.
        /// Green for less than 30 min, orange for 30–120 min, red for more than 2 hours.
        /// </summary>
        public string DurationCssClass
        {
            get
            {
                var mins = Session.SessionDuration.TotalMinutes;
                return mins switch
                {
                    < 30 => "text-success",
                    < 120 => "text-warning",
                    _ => "text-danger"
                };
            }
        }

        /// <summary>
        /// Gets a human-readable string for the session duration (e.g., "2h 14m").
        /// </summary>
        public string DurationDisplay
        {
            get
            {
                var d = Session.SessionDuration;
                if (d.TotalDays >= 1)
                    return $"{(int)d.TotalDays}d {d.Hours}h {d.Minutes}m";
                if (d.TotalHours >= 1)
                    return $"{(int)d.TotalHours}h {d.Minutes}m";
                return $"{(int)d.TotalMinutes}m {d.Seconds}s";
            }
        }

        /// <summary>
        /// Gets the session status string: Active, Closed, Abnormal, or Forced.
        /// </summary>
        public string Status
        {
            get
            {
                if (!Session.LogoutTimeUtc.HasValue)
                    return "Active";

                if (Session.ForceLogoutPerformed == true ||
                    string.Equals(Session.LogoutReason, "ForceLogoutAdmin", StringComparison.OrdinalIgnoreCase))
                    return "Forced";

                if (string.Equals(Session.LogoutReason, "AbnormalTermination", StringComparison.OrdinalIgnoreCase))
                    return "Abnormal";

                return "Closed";
            }
        }

        /// <summary>
        /// Gets the CSS class for the status badge.
        /// </summary>
        public string StatusCssClass => Status switch
        {
            "Active" => "active",
            "Forced" => "forced",
            "Abnormal" => "abnormal",
            "Closed" => "closed",
            _ => "closed"
        };

        /// <summary>
        /// Gets the Font Awesome icon class for the status badge.
        /// </summary>
        public string StatusIcon => Status switch
        {
            "Active" => "fa-circle-play",
            "Forced" => "fa-user-shield",
            "Abnormal" => "fa-triangle-exclamation",
            "Closed" => "fa-circle-check",
            _ => "fa-circle-check"
        };

        /// <summary>
        /// Gets the Font Awesome icon class for the device type.
        /// </summary>
        public string DeviceIcon => (Session.DeviceType?.ToLowerInvariant()) switch
        {
            "mobile" or "smartphone" => "fa-mobile-screen",
            "tablet" => "fa-tablet-screen-button",
            "desktop" => "fa-desktop",
            _ => "fa-display"
        };

        /// <summary>
        /// Gets the Font Awesome icon class for the browser.
        /// </summary>
        public string BrowserIcon => (Session.Browser?.ToLowerInvariant()) switch
        {
            string b when b.Contains("chrome") => "fa-brands fa-chrome",
            string b when b.Contains("firefox") => "fa-brands fa-firefox-browser",
            string b when b.Contains("safari") => "fa-brands fa-safari",
            string b when b.Contains("edge") => "fa-brands fa-edge",
            _ => "fa-globe"
        };

        /// <summary>
        /// Gets a value indicating whether this session was recently active (within the last 2 minutes).
        /// Used to show a pulsing dot in the UI.
        /// </summary>
        public bool IsRecentlyActive =>
            Session.LastActivityUtc.HasValue &&
            (DateTimeOffset.UtcNow - Session.LastActivityUtc.Value).TotalMinutes < 2;

        /// <summary>
        /// Gets a relative time description for the login time (e.g., "3 minutes ago").
        /// </summary>
        public string LoginRelative => ToRelativeTime(Session.LoginTimeUtc);

        /// <summary>
        /// Gets a relative time description for the last activity time.
        /// </summary>
        public string LastActivityRelative =>
            Session.LastActivityUtc.HasValue ? ToRelativeTime(Session.LastActivityUtc.Value) : "-";

        /// <summary>
        /// Gets a relative time description for the logout time.
        /// </summary>
        public string LogoutRelative =>
            Session.LogoutTimeUtc.HasValue ? ToRelativeTime(Session.LogoutTimeUtc.Value) : "-";

        /// <summary>
        /// Converts a DateTimeOffset to a human-readable relative time string.
        /// </summary>
        private static string ToRelativeTime(DateTimeOffset dt)
        {
            var span = DateTimeOffset.UtcNow - dt;

            if (span.TotalSeconds < 60) return "just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
            if (span.TotalDays < 30) return $"{(int)(span.TotalDays / 7)}w ago";
            return dt.ToString("yyyy-MM-dd");
        }
    }
}