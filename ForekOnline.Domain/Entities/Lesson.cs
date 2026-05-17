// <copyright file="Lesson.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/02/2025 16:01 PM
// Purpose:         Defines the Lesson model

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a scheduled lesson, including details such as room, topic, timing, and access information.
    /// </summary>
    /// <remarks>A Lesson encapsulates the core information required to describe and manage a single
    /// instructional session or meeting. This includes metadata for scheduling, access control, and audit purposes.
    /// Instances of this class are typically used in scheduling, calendar, or virtual classroom applications.</remarks>
    [SkipAuditInterceptor]
    public class Lesson
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the room.
        /// </summary>
        [Required]
        [StringLength(80)]
        public string RoomName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the topic associated with the entity.
        /// </summary>
        [Required]
        [StringLength(120)]
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start date and time of the event in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime StartUtc { get; set; }

        /// <summary>
        /// Gets or sets the end date and time of the period, expressed in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime EndUtc { get; set; }

        /// <summary>
        /// Gets or sets the URL that participants can use to join the meeting.
        /// </summary>
        [StringLength(400)]
        public string JoinUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password associated with the user or account.
        /// </summary>
        /// <remarks>The password is limited to a maximum of 50 characters. It is recommended to store
        /// passwords securely and avoid exposing them in logs or user interfaces.</remarks>
        [StringLength(50)]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the name or identifier of the user who created the entity.
        /// </summary>
        [StringLength(120)]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the entity was created, in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the current status of the item.
        /// </summary>
        /// <remarks>The status is represented as a string with a maximum length of 20 characters. The
        /// default value is "Scheduled". Valid status values may depend on the application's business logic.</remarks>
        [StringLength(20)]
        public string Status { get; set; } = "Scheduled";
    }
}
