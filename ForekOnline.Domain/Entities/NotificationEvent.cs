// <copyright file="NotificationEvent.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    18/10/2025 20:24:27 PM
// Purpose:         Defines the NotificationEvent class

#region Usings
using ForekOnline.Domain.Shared;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a notification event that can be displayed to users, including its metadata, content, and display
    /// settings.
    /// </summary>
    /// <remarks>A notification event defines the content and appearance of a notification, as well as its
    /// scheduling and audience targeting. It includes properties for specifying the event's title, display order,
    /// active status, and associated content blocks. Notifications can be grouped using the <see
    /// cref="CarouselGroupKey"/> property to appear together in a carousel.</remarks>
    [SkipAuditInterceptor]
    public class NotificationEvent
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the item.
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// Gets or sets the CSS class for the header icon.
        /// </summary>
        public string? HeaderIconCss { get; set; }   // e.g. "fa fa-rocket"

        /// <summary>
        /// Gets or sets the CSS class or inline style defining the gradient for the header.
        /// </summary>
        public string? HeaderGradientCss { get; set; } // fallback to your existing var(--ap-grad)

        /// <summary>
        /// Gets or sets the color of the header text.
        /// </summary>
        public string? HeaderTextColor { get; set; } // e.g. "#fff"

        /// <summary>
        /// Gets or sets the size of the notification modal.
        /// </summary>
        public eNotificationModalSize ModalSize { get; set; } = eNotificationModalSize.Large;

        /// <summary>
        /// Gets or sets the URL of the hero image.
        /// </summary>
        public string? ImageUrl { get; set; }        // Optional hero image

        /// <summary>
        /// Gets or sets the start date and time of the event in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime StartUtc { get; set; }

        /// <summary>
        /// Gets or sets the end date and time of the event in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime EndUtc { get; set; }

        /// <summary>
        /// Gets or sets the display order of the item. 
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the entity is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the role of the audience for which the operation is intended.
        /// </summary>
        public string? AudienceRole { get; set; }    // null = all

        /// <summary>
        /// Gets or sets the key used to group events together in a carousel.
        /// </summary>
        public string? CarouselGroupKey { get; set; } // Group events to appear together

        /// <summary>
        /// Gets or sets the collection of notification content blocks.
        /// </summary>
        public ICollection<NotificationContentBlock> Blocks { get; set; } = new List<NotificationContentBlock>();
    }
}
