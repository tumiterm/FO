// <copyright file="LicenseViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 14:09:27 PM
// Purpose:         Defines the LicenseViewModel class

#region Usings
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
using Microsoft.AspNetCore.Http;
#endregion

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a license associated with a course and company.
    /// </summary>
    public record LicenseViewModel
    {
        #region Course Foreign Key

        /// <summary>
        /// Gets or sets the unique identifier for the associated course.
        /// </summary>
        public Guid CourseKey { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the name of the license.
        /// </summary>
        public string? LicenseName { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the license.
        /// </summary>
        public Guid LicenseId { get; set; }

        /// <summary>
        /// Gets or sets the title associated with the license holder.
        /// </summary>
        public eTitle Title { get; set; }

        /// <summary>
        /// Gets or sets the first name of the license holder.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated company.
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the last name of the license holder.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the identification number of the license holder.
        /// </summary>
        public string? IDNumber { get; set; }

        /// <summary>
        /// Gets or sets the date the license was issued.
        /// </summary>
        public DateTime DateOfIssue { get; set; }

        /// <summary>
        /// Gets or sets the date the license expires.
        /// </summary>
        public DateTime DateOfExpiry { get; set; }

        /// <summary>
        /// Gets or sets the file upload path or name related to the license.
        /// </summary>
        public string? FileUpload { get; set; }

        /// <summary>
        /// Gets or sets the client type associated with the license.
        /// </summary>
        public eClientType ClientType { get; set; }

        /// <summary>
        /// Gets or sets the frequency at which the license needs to be renewed.
        /// </summary>
        public eLicenseFrequency Frequency { get; set; }

        /// <summary>
        /// Gets or sets the company name associated with the license.
        /// </summary>
        public string? Company { get; set; }

        /// <summary>
        /// Gets or sets the uploaded file related to the license (not mapped to the database).
        /// </summary>
        [NotMapped]
        public IFormFile? IFormFileUpload { get; set; }
    }

}
