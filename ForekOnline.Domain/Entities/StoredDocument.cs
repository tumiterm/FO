// <copyright file="StoredDocument.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    12/03/2025 16:00 AM
// Purpose:         Defines the StoredDocument class

#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a document stored in an external storage provider, including metadata and provider-specific
    /// information.
    /// </summary>
    /// <remarks>This class encapsulates information required to identify, retrieve, and manage a document
    /// stored in various storage backends (such as cloud blob storage or databases). It includes metadata such as file
    /// name, content type, file size, and optional retention or deletion status. The properties support scenarios such
    /// as soft deletion, retention policies, and auditing. Thread safety is not guaranteed; callers should ensure
    /// appropriate synchronization if instances are accessed concurrently.</remarks>
    public class StoredDocument 
    {
        /// <summary>
        /// Gets or sets the document identifier (Guid) used by the application.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the storage provider name (e.g., "AzureBlob", "Database").
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the provider-specific key (e.g., blob name, DB key, OneDrive item id).
        /// </summary>
        [Required]
        [StringLength(512)]
        public string ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the MIME type.
        /// </summary>
        [StringLength(255)]
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets or sets file size in bytes.
        /// </summary>
        public long FileSizeBytes { get; set; }

        /// <summary>
        /// Gets or sets optional JSON metadata.
        /// </summary>
        public string? MetadataJson { get; set; }

        /// <summary>
        /// Gets or sets optional expiry date for retention policies.
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the stored document is deleted (soft delete).
        /// </summary>
        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ModifiedOn { get; set; } = DateTimeOffset.UtcNow;
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
