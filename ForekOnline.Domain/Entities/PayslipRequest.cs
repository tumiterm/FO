// <copyright file="PayslipRequest.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2023 08:09:27 AM
// Purpose:         Defines the PayslipRequest class

#region Usings
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a request for a payslip or IRP5.
    /// </summary>
    public class PayslipRequest
    {
        /// <summary>
        /// Unique identifier for the request.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or Sets Document.
        /// </summary>
        public string? Document { get; set; }

        /// <summary>
        /// Gets or Sets StartMonth.
        /// </summary>
        public DateTime StartMonth { get; set; }

        /// <summary>
        /// Gets or Sets EndMonth.
        /// </summary>
        public DateTime? EndMonth { get; set; }

        /// <summary>
        /// Gets or Sets PayslipUploaded.
        /// </summary>
        public bool PayslipUploaded { get; set; } = false;

        /// <summary>
        /// Gets or Sets UploadDate.
        /// </summary>
        public DateTime? UploadDate { get; set; }

        /// <summary>
        /// The ID of the userId making the request.
        /// </summary>
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// The type of document being requested (e.g., Payslip, IRP5).
        /// </summary>
        public eTypeFile DocumentType { get; set; }

        /// <summary>
        /// The reason for the request, if provided.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// The current status of the request (e.g., Pending, Completed).
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// The date the request was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The date the request was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or Sets DocumentFile to be uploaded.
        /// </summary>
        [ValidateNever]
        [NotMapped]
        public IFormFile? DocumentFile { get; set; }
    }

}
