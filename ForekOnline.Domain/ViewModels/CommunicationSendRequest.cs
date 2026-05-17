using System.ComponentModel.DataAnnotations;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a request to send a communication message to one or more recipients using a specified channel.
    /// </summary>
    /// <remarks>This class is used to encapsulate the details required to send a message, such as the
    /// delivery channel, recipient type, message content, and optional course association. The request supports sending
    /// messages via email or SMS to students. Additional recipient types may be supported in future versions.</remarks>
    public sealed class CommunicationSendRequest
    {
        /// <summary>
        /// Gets or sets the delivery channel to use for notifications.
        /// </summary>
        /// <remarks>Valid values are "email" and "sms". The default value is "email". This property is
        /// required.</remarks>
        [Required]
        public string Channel { get; set; } = "email"; // email | sms

        /// <summary>
        /// Gets or sets the type of recipient for the message.
        /// </summary>
        /// <remarks>Valid values are "student" and, in future versions, "guardian". The default value is
        /// "student". Support for "guardian" may be added in a later release.</remarks>
        [Required]
        public string RecipientType { get; set; } = "student"; // student | guardian (guardian support later)

        /// <summary>
        /// Gets or sets the unique identifier of the associated course.
        /// </summary>
        public Guid? CourseId { get; set; }

        /// <summary>
        /// Gets or sets the collection of student numbers associated with the entity.
        /// </summary>
        public List<string> StudentNumbers { get; set; } = new();

        /// <summary>
        /// Gets or sets the subject associated with the current instance.
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
