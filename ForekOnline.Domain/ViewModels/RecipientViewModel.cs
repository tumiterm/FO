// <copyright file="RecipientViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the RecipientViewModel class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for a message recipient.
    /// </summary>
    public class RecipientViewModel
    {
        /// <summary>
        /// Gets or sets the recipient's mobile number.
        /// </summary>
        public string mobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the client message ID, if available.
        /// </summary>
        public string? ClientMessageId { get; set; }
    }

}
