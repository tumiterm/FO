// <copyright file="UserLoginHistory.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    26/01/2026 20:15:27 PM
// Purpose:         Defines the UserLoginHistory class


#region Usings
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a record of a user's login session, including session identifiers, timestamps, and related metadata.
    /// </summary>
    /// <remarks>This class is typically used to track user authentication activity, such as login and logout
    /// times, session duration, and device information. It can be useful for auditing, security analysis, and session
    /// management scenarios. Properties such as <see cref="SessionKey"/>, <see cref="IpAddress"/>, and <see
    /// cref="UserAgent"/> provide additional context about the session environment.</remarks>
    public class UserLoginHistory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the unique session key used to identify the current user session.
        /// </summary>
        /// <remarks>The session key must be a non-null string with a maximum length of 64 characters.
        /// This value is typically generated as a GUID in a compact format, but any unique string that meets the length
        /// requirement can be used.</remarks>
        [Required]
        [MaxLength(64)]
        public string? SessionKey { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the user logged in.
        /// </summary>
        public DateTimeOffset LoginTimeUtc { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets or sets the UTC date and time when the user logged out.
        /// </summary>
        public DateTimeOffset? LogoutTimeUtc { get; set; } 

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the last activity occurred for the
        /// entity.
        /// </summary>
        public DateTimeOffset? LastActivityUtc { get; set; }

        /// <summary>
        /// Gets the duration of the session, calculated as the difference between the login time and the logout time,
        /// or the current time if the session is still active.
        /// </summary>
        /// <remarks>If the session has not ended, the duration is measured up to the current UTC time.
        /// This property is not mapped to the database.</remarks>
        [NotMapped]
        public TimeSpan SessionDuration =>
            (LogoutTimeUtc ?? DateTimeOffset.UtcNow) - LoginTimeUtc;

        /// <summary>
        /// Gets or sets the IP address associated with the entity.
        /// </summary>
        [MaxLength(45)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent string associated with the request.
        /// </summary>
        /// <remarks>The user agent string typically identifies the client software initiating the
        /// request, such as a browser or application. The value is limited to a maximum of 250 characters.</remarks>
        [MaxLength(250)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the type of device associated with this entity.
        /// </summary>
        /// <remarks>The device type is represented as a string and may be used to categorize or identify
        /// the kind of device (for example, "Smartphone", "Tablet", or "Laptop"). The value is limited to a maximum of
        /// 100 characters.</remarks>
        [MaxLength(100)]
        public string? DeviceType { get; set; }

        /// <summary>
        /// Gets or sets the name and version of the browser used by the client, if available.
        /// </summary>
        [MaxLength(100)]
        public string? Browser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current session is active.
        /// </summary>
        public bool IsCurrentSession { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a forced logout has been performed for the current session.
        /// </summary>
        public bool? ForceLogoutPerformed { get; set; }

        /// <summary>
        /// Gets or sets the reason for the user's logout from the system.
        /// </summary>
        /// <remarks>Common values include "Explicit", "SessionExpired", and "AbnormalTermination". The
        /// value may be null if the reason is not specified.</remarks>
        [MaxLength(50)]
        public string? LogoutReason { get; set; } // "Explicit", "SessionExpired", "AbnormalTermination", etc.
    }
}