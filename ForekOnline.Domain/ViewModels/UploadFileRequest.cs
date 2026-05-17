using System.ComponentModel.DataAnnotations;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Request object for uploading a file.
    /// </summary>
    public record UploadFileRequest(
        /// <summary>
        /// The file stream to upload. Required.
        /// </summary>
        [Required]
        Stream FileStream,

        /// <summary>
        /// The original file name. Required for metadata.
        /// </summary>
        [Required, StringLength(255, MinimumLength = 1)]
        string FileName,

        /// <summary>
        /// The MIME type of the file (e.g., "image/jpeg"). Optional, can be inferred.
        /// </summary>
        string? ContentType = null,

        /// <summary>
        /// Optional metadata tags as key-value pairs (e.g., for categorization or search).
        /// </summary>
        Dictionary<string, string>? Metadata = null,

        /// <summary>
        /// Optional provider hint (e.g., "AwsS3") to override default routing.
        /// </summary>
        string? ProviderHint = null,

        /// <summary>
        /// Optional expiry date for the file (for auto-deletion policies).
        /// </summary>
        DateTimeOffset? ExpiryDate = null,

        /// <summary>
        /// Optional tenant scope for provider selection. Null means global.
        /// </summary>
        Guid? TenantId = null,

        /// <summary>
        /// Optional document type scope for provider selection (e.g., "Payslip", "Invoice").
        /// </summary>
        string? DocumentType = null
    );
}

