using ForekOnline.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IFileStorageProvider
    {
        // <summary>
        /// Gets the unique name of this provider (e.g., "AzureBlob", "AwsS3").
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Uploads a file asynchronously.
        /// </summary>
        /// <param name="request">The upload request containing file stream and metadata.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task that resolves to the upload response.</returns>
        Task<UploadFileResponse> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Downloads a file asynchronously as a stream.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task that resolves to the file stream and metadata response.</returns>
        Task<DownloadFileResponse> DownloadAsync(string fileId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a file asynchronously.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task that completes when the deletion is done.</returns>
        Task DeleteAsync(string fileId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a pre-signed URL for temporary access to the file.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file.</param>
        /// <param name="expiryInMinutes">The expiry duration in minutes.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task that resolves to the pre-signed URL.</returns>
        Task<string> GeneratePresignedUrlAsync(string fileId, int expiryInMinutes, CancellationToken cancellationToken = default);

        // Additional methods can be added here for extensibility, e.g., ListFilesAsync, GetMetadataAsync, etc.
    }
}
