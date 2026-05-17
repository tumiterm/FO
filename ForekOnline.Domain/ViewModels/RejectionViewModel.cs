// <copyright file="RejectionViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 11:50:27 AM
// Purpose:         Defines the RejectionViewModel class

#region Usings
using ForekOnline.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the view model for an application rejection, including rejection reason,
    /// additional comments, finality status, next steps, and follow-up details.
    /// </summary>
    public class RejectionViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the rejection record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the rejected application.
        /// </summary>
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the application associated with the rejection.
        /// This property is ignored by validation.
        /// </summary>
        [ValidateNever]
        public Application Application { get; set; }

        /// <summary>
        /// Gets or sets the primary reason for the rejection.
        /// Defaults to an empty string.
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets any additional comments regarding the rejection.
        /// This property is optional.
        /// </summary>
        public string? AdditionalComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the rejection is final.
        /// </summary>
        public bool IsFinal { get; set; }

        /// <summary>
        /// Gets or sets the suggested next steps for the applicant, if applicable.
        /// This property is optional.
        /// </summary>
        public string? NextSteps { get; set; }

        /// <summary>
        /// Gets or sets the follow-up date for reconsideration or further action, if applicable.
        /// This property is optional.
        /// </summary>
        public DateTime? FollowUpDate { get; set; }
    }

}
