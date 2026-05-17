// <copyright file="FileStorageSetting.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2023 14:09:27 PM
// Purpose:         Defines the FileStorageSetting class

#region Usings
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents configuration for selecting and enforcing file storage policies per tenant/document type.
    /// </summary>
    public class FileStorageSetting
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Optional tenant scope. Null means global.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Optional document type scope (string to avoid coupling to enums across contexts).
        /// Null means applies to all document types.
        /// </summary>
        [StringLength(100)]
        public string? DocumentType { get; set; }

        /// <summary>
        /// Provider type key used for resolution (e.g. "AzureBlob", "Database", "OneDrive", "GoogleDrive").
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ProviderType { get; set; } = string.Empty;

        /// <summary>
        /// Optional connection/provider details (recommended: encrypted/protected JSON).
        /// Examples: container name override, drive id, folder id, etc.
        /// </summary>
        public string? ConnectionDetails { get; set; }

        /// <summary>
        /// Max file size in MB.
        /// </summary>
        [Range(1, 10240)]
        public int MaxSizeMB { get; set; } = 100;

        public bool EncryptAtRest { get; set; } = true;

        public bool Compress { get; set; } = false;

        /// <summary>
        /// JSON array of allowed MIME types. Null means allow all.
        /// </summary>
        public string? AllowedMimeTypesJson { get; set; }

        /// <summary>
        /// Retention in days. 0 means no retention policy enforced here.
        /// </summary>
        [Range(0, 36500)]
        public int RetentionDays { get; set; } = 0;

        /// <summary>
        /// Optional fallback provider type (e.g. if primary provider is unavailable).
        /// </summary>
        [StringLength(100)]
        public string? FallbackProviderType { get; set; }

        /// <summary>
        /// JSON object of custom metadata to stamp on a stored document.
        /// </summary>
        public string? CustomMetadataJson { get; set; }

        /// <summary>
        /// If true, this setting is considered the default match (within its scope).
        /// </summary>
        public bool IsDefault { get; set; }

        public bool EnablePresignedUrls { get; set; } = false;

        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }

        #region Convenience (NotMapped) JSON helpers

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public IReadOnlyCollection<string>? AllowedMimeTypes
        {
            get => string.IsNullOrWhiteSpace(AllowedMimeTypesJson)
                ? null
                : JsonSerializer.Deserialize<IReadOnlyCollection<string>>(AllowedMimeTypesJson);
            set => AllowedMimeTypesJson = value is null ? null : JsonSerializer.Serialize(value);
        }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public Dictionary<string, string>? CustomMetadata
        {
            get => string.IsNullOrWhiteSpace(CustomMetadataJson)
                ? null
                : JsonSerializer.Deserialize<Dictionary<string, string>>(CustomMetadataJson);
            set => CustomMetadataJson = value is null ? null : JsonSerializer.Serialize(value);
        }

        #endregion
    }
}
