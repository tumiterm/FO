using System.ComponentModel.DataAnnotations;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Response object for a successful file upload.
    /// </summary>
    public record UploadFileResponse(
        /// <summary>
        /// The unique identifier for the uploaded file (e.g., GUID or provider-specific key).
        /// </summary>
        [Required]
        string FileId,

        /// <summary>
        /// Timestamp of the upload.
        /// </summary>
        DateTimeOffset UploadTimestamp,


        /// <summary>
        /// The provider used for storage.
        /// </summary>
        string ProviderName,

        /// <summary>
        /// File size in bytes.
        /// </summary>
        long FileSizeBytes,

        /// <summary>
        /// The permanent URL or path to the file.
        /// </summary>
        string? FileUrl = null,

        /// <summary>
        /// Any additional metadata returned by the provider.
        /// </summary>
        Dictionary<string, string>? AdditionalMetadata = null

    );
}

