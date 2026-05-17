// <copyright file="StoredDocumentContent.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    12/03/2025 16:00 PM
// Purpose:         Defines the StoredDocumentContent class

#region Usings
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Stores the raw file bytes for database-backed document storage.
    /// </summary>
    public class StoredDocumentContent
    {
        /// <summary>
        /// Primary key and foreign key to <see cref="StoredDocument.Id"/>.
        /// </summary>
        [Key]
        [ForeignKey(nameof(StoredDocument))]
        public Guid Id { get; set; }

        /// <summary>
        /// Raw file content (varbinary(max)).
        /// </summary>
        [Required]
        public byte[] Content { get; set; }

        public StoredDocument? StoredDocument { get; set; }
    }
}
