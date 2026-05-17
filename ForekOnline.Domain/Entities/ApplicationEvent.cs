// <copyright file="ApplicationEvent.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    06/01/2024 12:09:08 PM
// Purpose:         Defines the ApplicationEvent class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a scheduled event within the application, including timing, category, and descriptive details.
    /// </summary>
    /// <remarks>Use this class to store and manage information about events such as application windows,
    /// holidays, or exams. The event supports both all-day and timed occurrences, and includes metadata for auditing
    /// and categorization. Event times are stored in Coordinated Universal Time (UTC) to ensure consistency across time
    /// zones.</remarks>
    [SkipAuditInterceptor]
    public class ApplicationEvent
    {
        /// <summary>
        /// Gets or sets the unique identifier for the event.
        /// </summary>
        [Key]
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the title associated with the object.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start date and time of the event in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime StartUtc { get; set; }

        /// <summary>
        /// Gets or sets the exclusive end date and time of the event in Coordinated Universal Time (UTC).
        /// </summary>
        /// <remarks>The end time is exclusive, meaning the event does not include this exact moment. This
        /// is consistent with FullCalendar's handling of all-day events. If the value is null, the event is considered
        /// to have no defined end.</remarks>
        public DateTime? EndUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event lasts for the entire day.
        /// </summary>
        public bool AllDay { get; set; }

        /// <summary>
        /// Gets or sets the category associated with the item.
        /// </summary>
        public string? Category { get; set; } // e.g. "Application Window", "Holiday", "Exam"

        /// <summary>
        /// Gets or sets the custom color for the category as a hexadecimal color code.
        /// </summary>
        /// <remarks>The color code should be specified in standard hexadecimal format (for example,
        /// "#FF5733"). If the value is null or empty, a default color may be used instead.</remarks>
        public string? ColorHex { get; set; } // optional custom color per category

        /// <summary>
        /// Gets or sets the description associated with the object.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the date and time when the entity was created, in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the name or identifier of the user who created the entity.
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time, in Coordinated Universal Time (UTC), when the entity was last modified.
        /// </summary>
        public DateTime? ModifiedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last modified the entity.
        /// </summary>
        public string? ModifiedBy { get; set; }
    }
}
