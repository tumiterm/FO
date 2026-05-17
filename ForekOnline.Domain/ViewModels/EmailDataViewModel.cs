// <copyright file="EmailDataViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the EmailDataViewModel class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the data required to send an email.
    /// </summary>
    public class EmailDataViewModel
    {
        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// Gets or sets the subject of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body/content of the email.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the sender or the display name for the sender.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the Header of the Email.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets optional email attachments.
        /// </summary>
        public List<EmailAttachmentViewModel> Attachments { get; set; } = new();
    }

}
