// <copyright file="PayslipRequestViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 10:09:27 AM
// Purpose:         Defines the PayslipRequestViewModel class

#region Usings
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// ViewModel for creating a payslip or IRP5 request.
    /// </summary>
    public class PayslipRequestViewModel
    {
        /// <summary>
        /// Unique identifier for the request.
        /// </summary>
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// Unique identifier for the request.
        /// </summary>
        public Guid Id { get; set; }


        /// <summary>
        /// Gets or Sets Document.
        /// </summary>
        public string? Document { get; set; }

        /// <summary>
        /// The type of document being requested (e.g., Payslip, IRP5).
        /// </summary>
        [Required(ErrorMessage = "Document type is required.")]
        public eTypeFile DocumentType { get; set; }

        /// <summary>
        /// The reason for the request.
        /// </summary>
        [MaxLength(250, ErrorMessage = "Reason cannot exceed 250 characters.")]
        public string? Reason { get; set; }

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
        /// Gets or Sets DocumentFile to be uploaded.
        /// </summary>
        [ValidateNever]
        public IFormFile? DocumentFile { get; set; }

    }
}
