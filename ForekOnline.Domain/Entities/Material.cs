// <copyright file="Material.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the Material class

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
    /// Represents a material resource, including details such as document, type, due date, and trade association.
    /// </summary>
    [SkipAuditInterceptor]
    public class Material : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the material.
        /// </summary>
        [Key]
        public Guid MaterialId { get; set; }

        /// <summary>
        /// Gets or sets the document associated with the material (e.g., file path or URL).
        /// </summary>
        public string? Document { get; set; }

        /// <summary>
        /// Gets or sets the type of material (e.g., course material, reference, etc.).
        /// </summary>
        public eMaterialType Type { get; set; }

        /// <summary>
        /// Gets or sets the message associated with the material, which can contain additional information or instructions.
        /// </summary>
        [DataType(DataType.MultilineText)]
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the due date for the material, indicating when it needs to be completed or reviewed.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the trade associated with the material (e.g., a specific trade or industry sector).
        /// </summary>
        public eTrade Trade { get; set; }

        /// <summary>
        /// Gets or sets the module associated with the material, which can represent a specific section of a course or training program.
        /// </summary>
        public eModule? Module { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the instructor associated with the material, representing the person who provided or oversees it.
        /// </summary>
        public string? InstructorId { get; set; }

        /// <summary>
        /// Gets or sets the file attached to the material, which is not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? AttachmentFile { get; set; }
    }
}
