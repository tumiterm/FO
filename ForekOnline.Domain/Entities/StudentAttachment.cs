// <copyright file="StudentAttachment.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 12:01:14 PM
// Purpose:         Defines the StudentAttachment class

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
    /// Represents an attachment related to a student, including the document and student details.
    /// </summary>
    [SkipAuditInterceptor]
    public class StudentAttachment : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the attachment.
        /// </summary>
        [Key]
        public Guid AttachmentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the document related to the student attachment.
        /// </summary>
        [Display(Name = "Document Name")]
        public eLearnerAdministration DocumentName { get; set; }

        /// <summary>
        /// Gets or sets the file path or name of the document associated with the attachment.
        /// </summary>
        public string? Document { get; set; }

        /// <summary>
        /// Gets or sets the student number related to the attachment.
        /// </summary>
        [Display(Name = "StudentId Number")]
        public string? StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the student related to the attachment.
        /// </summary>
        public Guid? StudentId { get; set; }

        /// <summary>
        /// Gets or sets the file to be attached as part of the student attachment. This is not mapped to the database.
        /// </summary>
        [NotMapped]
        [Display(Name = "File")]
        public IFormFile? AttachmentFile { get; set; }
    }
}
