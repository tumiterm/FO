// <copyright file="NotificationContentBlock.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    18/10/2025 20:27:27 PM
// Purpose:         Defines the NotificationContentBlock class

#region Usings
using ForekOnline.Domain.Shared;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a content block within a notification, which can include text, lists, tables, images, or other
    /// content types.
    /// </summary>
    /// <remarks>A notification content block is associated with a specific notification event and can contain
    /// various types of content,  such as plain text, a list of items, a table (in JSON format), or an image with
    /// optional alternative text.  The <see cref="Type"/> property determines the content type, and the corresponding
    /// properties (e.g., <see cref="Text"/>,  <see cref="ListItems"/>, <see cref="TableJson"/>, <see cref="ImageUrl"/>)
    /// are used to store the content data.</remarks>
    [SkipAuditInterceptor]
    public class NotificationContentBlock
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the notification event.
        /// </summary>
        public Guid NotificationEventId { get; set; }

        /// <summary>
        /// Gets or sets the type of notification content.
        /// </summary>
        public eNotificationContentType Type { get; set; }

        /// <summary>
        /// Gets or sets the text content associated with this instance.
        /// </summary>
        public string? Text { get; set; }   
        
        /// <summary>
        /// Gets or sets the collection of list items.
        /// </summary>
        public string?[]? ListItems { get; set; }    // For list blocks

        /// <summary>
        /// Gets or sets the JSON representation of a table, including headers and rows.
        /// </summary>
        public string? TableJson { get; set; }       // JSON: { "headers":[], "rows":[[]] }

        /// <summary>
        /// Gets or sets the URL of the image associated with this entity.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the alternative text for an image or visual element.
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>
        /// Gets or sets the notification event associated with the current operation.
        /// </summary>
        public NotificationEvent? NotificationEvent { get; set; }

        /// <summary>
        /// Gets or sets the order in which this item is processed or displayed.
        /// </summary>
        public int Order { get; set; }

    }
}
