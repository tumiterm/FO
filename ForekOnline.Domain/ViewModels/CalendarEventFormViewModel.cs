// <copyright file="CalendarEventFormViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    28/12/2025 13:46:27 PM
// Purpose:         Defines the CalendarEventFormViewModel class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the data model for a calendar event form, including event details such as title,  start and end
    /// times, and additional metadata.
    /// </summary>
    /// <remarks>This view model is typically used to capture or display information about a calendar event 
    /// in a user interface. It includes properties for event identification, scheduling, and  categorization, as well
    /// as optional fields for additional details such as color and description.</remarks>
    public class CalendarEventFormViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the event.
        /// </summary>
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
        /// Gets or sets the end date and time of the event in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime? EndUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event spans the entire day.
        /// </summary>
        public bool AllDay { get; set; }

        /// <summary>
        /// Gets or sets the category associated with the item.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the hexadecimal color code representing the color.
        /// </summary>
        public string? ColorHex { get; set; }

        /// <summary>
        /// Gets or sets the description associated with the object.
        /// </summary>
        public string? Description { get; set; }
    }
}
