// <copyright file="License.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the License class

#region Usings
using ForekOnline.Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a license record, including details such as title, personal information, course association, and validity dates.
    /// </summary>
    [SkipAuditInterceptor]
    public class License : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the license record.
        /// </summary>
        [Key]
        public Guid LicenseId { get; set; }

        /// <summary>
        /// Gets or sets the title associated with the license (e.g., Mr., Mrs., etc.).
        /// </summary>
        public eTitle Title { get; set; }

        #region Course Foreign Key
        /// <summary>
        /// Gets or sets the unique identifier for the associated course.
        /// </summary>
        public Guid CourseKey { get; set; }
        #endregion

        /// <summary>
        /// Gets or sets the full name of the individual to whom the license is issued.
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Full Name")]
        [MinLength(3)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the last name of the individual to whom the license is issued.
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        [MinLength(3)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the ID or passport number of the individual to whom the license is issued.
        /// </summary>
        [Display(Name = "ID/Passport Number")]
        public string? IDNumber { get; set; }

        /// <summary>
        /// Gets or sets the date when the license was issued.
        /// </summary>
        [Required(ErrorMessage = "Date of Issue is required")]
        public DateTime DateOfIssue { get; set; }

        /// <summary>
        /// Gets or sets the date when the license will expire.
        /// </summary>
        [Required(ErrorMessage = "Date of Expiry is required")]
        public DateTime DateOfExpiry { get; set; }

        /// <summary>
        /// Gets or sets the file path or URL of any uploaded supporting file for the license.
        /// </summary>
        public string? FileUpload { get; set; }

        /// <summary>
        /// Gets or sets the client type associated with the license (e.g., individual, corporate, etc.).
        /// </summary>
        public eClientType ClientType { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the company associated with the license.
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the frequency of the license (e.g., annual, bi-annual, etc.).
        /// </summary>
        public eLicenseFrequency Frequency { get; set; }

        /// <summary>
        /// Gets or sets the uploaded license document file, not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? IFormFileUpload { get; set; }
    }

}
