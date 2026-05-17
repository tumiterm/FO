// <copyright file="Training.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    03/01/2025 12:01:14 PM
// Purpose:         Defines the Training class

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
    /// Represents a training record for a student, including related documents and types.
    /// </summary>
    [SkipAuditInterceptor]
    public class Training : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the training record.
        /// </summary>
        [Key]
        public Guid TrainingId { get; set; }

        /// <summary>
        /// Gets or sets the student number associated with the training record.
        /// </summary>
        public string? StudentNumber { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the student related to the training record.
        /// </summary>
        public Guid? StudentId { get; set; }

        /// <summary>
        /// Gets or sets the file path or name of the document associated with the training record.
        /// </summary>
        public string? Document { get; set; }

        /// <summary>
        /// Gets or sets the type of training document in the training administration system.
        /// </summary>
        [Display(Name = "Document Type")]
        public eTrainingAdministration Type { get; set; }

        /// <summary>
        /// Gets or sets the file to be attached as part of the training record. This is not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? DocumentFile { get; set; }
    }
}
