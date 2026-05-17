// <copyright file="SubReportUploadRow.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    09-03-2026 21:38 PM
// Purpose:         Defines the SubReportUploadRow.

#region Usings
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a row of data for uploading a sub-report, including file metadata and the uploaded document.
    /// </summary>
    /// <remarks>This class is typically used to capture user input for sub-report uploads in web
    /// applications, such as within form submissions. All required fields must be provided for successful
    /// processing.</remarks>
    public class SubReportUploadRow
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description associated with the entity.
        /// </summary>
        [StringLength(250)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the uploaded document file associated with the request.
        /// </summary>
        /// <remarks>This property is typically used to receive a file from a client in a
        /// multipart/form-data HTTP request. The file must be provided for the request to be considered
        /// valid.</remarks>
        [Required]
        public IFormFile? DocumentFile { get; set; }
    }
}
