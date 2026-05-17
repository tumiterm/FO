// <copyright file="SMSViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/08/2023 13:18 PM
// Purpose:         Defines the SMSViewModel class

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the model for sending an SMS message.
    /// </summary>
    public class SMSViewModel
    {
        /// <summary>
        /// Gets or sets the content of the SMS message.
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Gets or sets the list of recipients for the SMS message.
        /// </summary>
        public List<RecipientViewModel> recipients { get; set; }

        /// <summary>
        /// Gets or sets the scheduled time for sending the SMS message (optional).
        /// </summary>
        public string? scheduledTime { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of message segments allowed (optional).
        /// </summary>
        public int? maxSegments { get; set; }
    }

}
