// <copyright file="InAppNotification.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/04/2026 21:19 PM
// Purpose:         Defines the InAppNotification class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents an in-app notification targeted at a specific user.
    /// </summary>
    [Table(nameof(InAppNotification), Schema = "Alerts")]
    public class InAppNotification : EntityBase<Guid>
    {
        /// <summary>
        /// Gets or sets the recipient user's ID.
        /// </summary>
        public Guid RecipientUserId { get; set; }

        /// <summary>
        /// Gets or sets the notification message.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional URL the user is redirected to when they click the notification.
        /// </summary>
        [MaxLength(500)]
        public string? ActionUrl { get; set; }

        /// <summary>
        /// Gets or sets the icon CSS class for the notification (e.g. "fa fa-file-alt").
        /// </summary>
        [MaxLength(100)]
        public string IconCss { get; set; } = "fa fa-bell";

        /// <summary>
        /// Gets or sets a value indicating whether the notification has been read.
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Gets or sets the date and time the notification was read (UTC).
        /// </summary>
        public DateTime? ReadUtc { get; set; }
    }
}