// <copyright file="File.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2023 11:00 AM
// Purpose:         Defines the File class

#region Usings
using ForekOnline.Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a file entity with associated metadata.
    /// </summary>
    [SkipAuditInterceptor]
    public class File : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the file.
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// Gets or sets the type of the file.
        /// </summary>
        public eFileType Type { get; set; }

        /// <summary>
        /// Gets or sets the phase associated with the file, if applicable.
        /// </summary>
        public ePhase? Phase { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user associated with the file.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the start date for the file's validity or relevance.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for the file's validity or relevance.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the attachment file path or reference.
        /// </summary>
        public string? Attachment { get; set; }

        /// <summary>
        /// Gets or sets the URL of the file stored in a blob storage.
        /// </summary>
        public string? BlobFileURL { get; set; }


        /// <summary>
        /// Gets or sets the file attachment as an uploaded form file (not mapped to the database).
        /// </summary>
        [NotMapped]
        public IFormFile? AttachmentFile { get; set; }
    }

}
