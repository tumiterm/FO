// <copyright file="ApplicationRejection.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/08/2023 12:09 PM
// Purpose:         Defines the ApplicationRejection class

#region Usings
using ForekOnline.Domain.Shared;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents the rejection of an application, including the reason, comments, and next steps.
    /// </summary>
    [SkipAuditInterceptor]
    public class ApplicationRejection : Base
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
        /// Gets or sets the application associated with this rejection.
        /// </summary>
        [ValidateNever]
        public Application Application { get; set; }

        /// <summary>
        /// Gets or sets the reason for rejecting the application.
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets any additional comments regarding the rejection.
        /// </summary>
        public string? AdditionalComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the rejection is final.
        /// </summary>
        public bool IsFinal { get; set; }

        /// <summary>
        /// Gets or sets the suggested next steps for the applicant, if any.
        /// </summary>
        public string? NextSteps { get; set; }

        /// <summary>
        /// Gets or sets the optional follow-up date for reconsideration or review.
        /// </summary>
        public DateTime? FollowUpDate { get; set; }
    }

}
