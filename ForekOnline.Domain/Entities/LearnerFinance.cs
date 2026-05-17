// <copyright file="LearnerFinance.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the LearnerFinance class

#region Usings
using ForekOnline.Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents the financial records of a learner, including statements and file attachments.
    /// </summary>
    [SkipAuditInterceptor]
    public class LearnerFinance : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the learner's finance record.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the student associated with this financial record.
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// Gets or sets the student number.
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the file path or URL of the financial document.
        /// </summary>
        public string? File { get; set; }

        /// <summary>
        /// Gets or sets the name of the financial statement.
        /// </summary>
        [Display(Name = "Statement Name")]
        public string? StatementName { get; set; }

        /// <summary>
        /// Gets or sets the uploaded file attachment, not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? FileAttach { get; set; }
    }

}
