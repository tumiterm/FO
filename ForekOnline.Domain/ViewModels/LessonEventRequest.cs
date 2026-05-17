// <copyright file="LessonEventRequest.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/02/2025 16:00 PM
// Purpose:         Defines the LessonEventRequest model

#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a request to record an event related to a lesson, such as a participant joining or leaving.
    /// </summary>
    /// <remarks>Use this type to submit information about lesson-related events for processing or auditing
    /// purposes. Both the lesson identifier and the event type are required. The event type typically indicates actions
    /// such as "Joined" or "Left".</remarks>
    public sealed class LessonEventRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier for the lesson.
        /// </summary>
        [Required]
        public Guid LessonId { get; set; }

        /// <summary>
        /// Gets or sets the type of event represented by this instance.
        /// </summary>
        /// <remarks>The event type is a string value that identifies the nature of the event, such as
        /// "Joined" or "Left". The value is required and must not exceed 10 characters in length.</remarks>
        [Required]
        [StringLength(10)]
        public string EventType { get; set; } = string.Empty; // "Joined" | "Left"
    }
}
