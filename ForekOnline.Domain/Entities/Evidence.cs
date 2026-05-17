// <copyright file="Evidence.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the Evidence class


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
    /// Represents evidence related to a student's module, including photos and student details.
    /// </summary>
    [SkipAuditInterceptor]
    public class Evidence : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the evidence record.
        /// </summary>
        [Key]
        public Guid EvidenceId { get; set; }

        /// <summary>
        /// Gets or sets the module associated with the evidence.
        /// </summary>
        public eModule Module { get; set; }

        /// <summary>
        /// Gets or sets the student number associated with the evidence.
        /// </summary>
        public string? StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the student associated with the evidence.
        /// </summary>
        public Guid? StudentId { get; set; }

        /// <summary>
        /// Gets or sets the file path or URL of the photo evidence.
        /// </summary>
        public string? Photo { get; set; }

        /// <summary>
        /// Gets or sets the uploaded photo file, not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile PhotoFile { get; set; }
    }

}
