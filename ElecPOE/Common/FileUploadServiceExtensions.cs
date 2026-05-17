// <copyright file="FileUploadServiceExtensions.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    17/01/2026 15:31:27 PM
// Purpose:         Defines the FileUploadServiceExtensions controller

#region Using Directives
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.ViewModels;
#endregion

namespace ElecPOE.Common
{
    /// <summary>
    /// Provides extension methods for the IFileUploadService interface to simplify common file upload, download, and
    /// deletion operations with optional presence checks.
    /// </summary>
    /// <remarks>These extension methods enable streamlined handling of file operations by automatically
    /// checking for null or empty values before invoking the underlying service methods. This helps prevent unnecessary
    /// exceptions and reduces boilerplate code when working with optional file uploads or downloads. All methods are
    /// asynchronous and support cancellation via a CancellationToken parameter.</remarks>
    internal static class FileUploadServiceExtensions
    {
        /// <summary>
        /// Uploads the specified file using the provided file upload service if the file is present and has content.
        /// </summary>
        /// <remarks>If the file parameter is null or has a length of zero, the method does not perform an
        /// upload and returns null. The method is an extension for IFileUploadService to simplify conditional file
        /// uploads.</remarks>
        /// <param name="fileUploadService">The file upload service used to perform the upload operation.</param>
        /// <param name="file">The file to upload. If null or empty, no upload is performed and the method returns null.</param>
        /// <param name="documentType">The type of document being uploaded. Cannot be null, empty, or whitespace.</param>
        /// <param name="metadata">A read-only dictionary containing metadata to associate with the uploaded file. Cannot be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A string containing the unique identifier of the uploaded file if the upload succeeds; otherwise, null if
        /// the file is not present or has no content.</returns>
        /// <exception cref="ArgumentNullException">Thrown if fileUploadService or metadata is null.</exception>
        /// <exception cref="ArgumentException">Thrown if documentType is null, empty, or consists only of white-space characters.</exception>
        public static async Task<string?> UploadIfPresentAsync(this IFileUploadService fileUploadService, IFormFile? file, string documentType, IReadOnlyDictionary<string, string> metadata, CancellationToken ct = default)
        {
            if (fileUploadService is null)
            {
                throw new ArgumentNullException(nameof(fileUploadService));
            }

            if (file is null || file.Length <= 0)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(documentType))
            {
                throw new ArgumentException("Document type is required.", nameof(documentType));
            }

            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            await using var stream = file.OpenReadStream();

            var response = await fileUploadService.UploadAsync(
                new UploadFileRequest(
                    FileStream: stream,
                    FileName: file.FileName,
                    ContentType: file.ContentType,
                    Metadata: new Dictionary<string, string>(metadata),
                    ProviderHint: null,
                    ExpiryDate: null,
                    TenantId: null,
                    DocumentType: documentType),
                ct).ConfigureAwait(false);

            return response.FileId;
        }

        /// <summary>
        /// Attempts to download a file by its identifier if it exists, returning the file stream, content type, and
        /// file name.
        /// </summary>
        /// <param name="fileUploadService">The file upload service used to retrieve the file. Cannot be null.</param>
        /// <param name="fileId">The unique identifier of the file to download. If null or whitespace, the method returns null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A tuple containing the file stream, content type, and file name if the file is found; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileUploadService"/> is null.</exception>
        public static async Task<(Stream FileStream, string? ContentType, string FileName)?> DownloadIfPresentAsync(this IFileUploadService fileUploadService, string? fileId, CancellationToken ct = default)
        {
            if (fileUploadService is null)
            {
                throw new ArgumentNullException(nameof(fileUploadService));
            }

            if (string.IsNullOrWhiteSpace(fileId))
            {
                return null;
            }

            var download = await fileUploadService.DownloadAsync(fileId, ct).ConfigureAwait(false);

            return (download.FileStream, download.ContentType, download.FileName);
        }

        /// <summary>
        /// Attempts to delete a file with the specified identifier if it exists.
        /// </summary>
        /// <param name="fileUploadService">The file upload service used to perform the deletion operation. Cannot be null.</param>
        /// <param name="fileId">The identifier of the file to delete. If null, empty, or consists only of white-space characters, no action
        /// is taken.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the file
        /// identifier was valid and the delete operation was invoked; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileUploadService"/> is null.</exception>
        public static async Task<bool> DeleteIfPresentAsync(this IFileUploadService fileUploadService, string? fileId, CancellationToken ct = default)
        {
            if (fileUploadService is null)
            {
                throw new ArgumentNullException(nameof(fileUploadService));
            }

            if (string.IsNullOrWhiteSpace(fileId))
            {
                return false;
            }

            await fileUploadService.DeleteAsync(fileId, ct).ConfigureAwait(false);

            return true;
        }
    }
}

