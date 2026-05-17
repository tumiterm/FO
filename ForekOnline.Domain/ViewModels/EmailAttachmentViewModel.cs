// <copyright file="EmailAttachmentViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/02/2026 03:17 PM
// Purpose:         Defines the EmailAttachmentViewModel class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents an email attachment, including its file name, content, and MIME type.
    /// </summary>
    /// <remarks>Use this class to provide attachment data when composing or displaying emails. The properties
    /// are immutable and must be set during object initialization. This type is suitable for scenarios where
    /// attachments are handled in memory, such as web APIs or email clients.</remarks>
    public class EmailAttachmentViewModel
    {
        /// <summary>
        /// Gets the name of the file associated with this instance.
        /// </summary>
        public string FileName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the binary content associated with this instance.
        /// </summary>
        public byte[] Content { get; init; } = Array.Empty<byte>();

        /// <summary>
        /// Gets the MIME type of the content represented by this instance.
        /// </summary>
        /// <remarks>The default value is "application/octet-stream", which indicates generic binary data.
        /// Set this property to specify the actual content type when known, such as "application/json" or
        /// "text/plain".</remarks>
        public string ContentType { get; init; } = "application/octet-stream";
    }
}
