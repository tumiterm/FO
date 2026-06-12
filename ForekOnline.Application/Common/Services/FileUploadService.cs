// <copyright file="FileUploadService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    07/02/2025 12:06:14 PM
// Purpose:         Defines the FileUploadService class

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.ViewModels;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides file upload, download, deletion, and presigned URL generation services using a configurable file
    /// storage provider.
    /// </summary>
    /// <remarks>This class resolves the appropriate file storage provider based on file metadata and
    /// delegates file operations accordingly. It supports asynchronous operations for uploading, downloading, and
    /// deleting files, as well as generating presigned URLs for secure, time-limited access. Instances of this class
    /// are thread-safe and intended for use in multi-tenant or provider-agnostic scenarios.</remarks>
    public sealed class FileUploadService : IFileUploadService
    {
        #region Fields
        private readonly IFileStorageProviderResolver _providerResolver;
        private readonly IStoredDocumentLookup _storedDocumentLookup;
        private readonly IFileStorageSettingsService _fileStorageSettingsService;
        private readonly ITenantContext _tenantContext;
        #endregion'

        /// <summary>
        /// Initializes a new instance of the FileUploadService class with the specified storage provider resolver,
        /// stored document lookup, and file storage settings service.
        /// </summary>
        /// <param name="providerResolver">The resolver used to select the appropriate file storage provider based on configuration or context. Cannot
        /// be null.</param>
        /// <param name="storedDocumentLookup">The service used to look up stored documents by their identifiers. Cannot be null.</param>
        /// <param name="fileStorageSettingsService">The service that provides access to file storage settings and configuration. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if providerResolver, storedDocumentLookup, or fileStorageSettingsService is null.</exception>
        public FileUploadService(IFileStorageProviderResolver providerResolver, IStoredDocumentLookup storedDocumentLookup, IFileStorageSettingsService fileStorageSettingsService, ITenantContext tenantContext)
        {
            _providerResolver = providerResolver ?? throw new ArgumentNullException(nameof(providerResolver));
            _storedDocumentLookup = storedDocumentLookup ?? throw new ArgumentNullException(nameof(storedDocumentLookup));
            _fileStorageSettingsService = fileStorageSettingsService ?? throw new ArgumentNullException(nameof(fileStorageSettingsService));
            _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        }

        /// <summary>
        /// Asynchronously deletes the file identified by the specified file ID.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file to delete. Must be a valid GUID in string format and cannot be null,
        /// empty, or whitespace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the delete operation.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="ArgumentException">Thrown if fileId is null, empty, consists only of white-space characters, or is not a valid GUID.</exception>
        public async Task DeleteAsync(string fileId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileId)) throw new ArgumentException("fileId is required.", nameof(fileId));
            if (!Guid.TryParse(fileId, out var guid)) throw new ArgumentException("fileId must be a valid Guid.", nameof(fileId));

            var providerName = await _storedDocumentLookup.GetProviderNameAsync(guid, cancellationToken).ConfigureAwait(false);
            var provider = _providerResolver.Resolve(providerName);

            await provider.DeleteAsync(fileId, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously downloads a file with the specified identifier.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file to download. Must be a valid GUID in string format and cannot be null,
        /// empty, or whitespace.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the download operation.</param>
        /// <returns>A task that represents the asynchronous download operation. The task result contains a <see
        /// cref="DownloadFileResponse"/> with the downloaded file data.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="fileId"/> is null, empty, consists only of white-space characters, or is not a
        /// valid GUID.</exception>
        public async Task<DownloadFileResponse> DownloadAsync(string fileId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileId)) throw new ArgumentException("fileId is required.", nameof(fileId));
            if (!Guid.TryParse(fileId, out var guid)) throw new ArgumentException("fileId must be a valid Guid.", nameof(fileId));

            var providerName = await _storedDocumentLookup.GetProviderNameAsync(guid, cancellationToken).ConfigureAwait(false);
            var provider = _providerResolver.Resolve(providerName);

            return await provider.DownloadAsync(fileId, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Generates a presigned URL that allows temporary access to the specified file.
        /// </summary>
        /// <remarks>The generated presigned URL can be used to access the file without authentication
        /// until it expires. If presigned URLs are disabled in the storage settings, the method returns an empty
        /// string.</remarks>
        /// <param name="fileId">The unique identifier of the file for which to generate the presigned URL. Must be a valid GUID string.</param>
        /// <param name="expiryInMinutes">The duration, in minutes, for which the presigned URL will remain valid. Must be greater than zero.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation. Optional.</param>
        /// <returns>A presigned URL as a string that grants temporary access to the file. Returns an empty string if presigned
        /// URLs are not enabled for the current storage settings.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="fileId"/> is null, empty, or not a valid GUID.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="expiryInMinutes"/> is less than or equal to zero.</exception>
        public async Task<string> GeneratePresignedUrlAsync(string fileId, int expiryInMinutes, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileId)) throw new ArgumentException("fileId is required.", nameof(fileId));
            if (!Guid.TryParse(fileId, out var guid)) throw new ArgumentException("fileId must be a valid Guid.", nameof(fileId));
            if (expiryInMinutes <= 0) throw new ArgumentOutOfRangeException(nameof(expiryInMinutes));

            var settings = await _fileStorageSettingsService
                .ResolveAsync(_tenantContext.TenantId, documentType: null, cancellationToken)
                .ConfigureAwait(false);

            if (settings is null || !settings.EnablePresignedUrls)
            {
                return string.Empty;
            }

            var providerName = await _storedDocumentLookup.GetProviderNameAsync(guid, cancellationToken).ConfigureAwait(false);
            var provider = _providerResolver.Resolve(providerName);

            return await provider.GeneratePresignedUrlAsync(fileId, expiryInMinutes, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously uploads a file using the specified upload request and returns the result of the upload
        /// operation.
        /// </summary>
        /// <remarks>If the <c>ProviderHint</c> property of <paramref name="request"/> is not specified,
        /// the provider is resolved automatically based on tenant and document type settings.</remarks>
        /// <param name="request">The upload request containing file data, metadata, and upload options. Cannot be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the upload operation. The default value is <see
        /// cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous upload operation. The task result contains an <see
        /// cref="UploadFileResponse"/> describing the outcome of the upload.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is null.</exception>
        public async Task<UploadFileResponse> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            request = request with { TenantId = request.TenantId ?? _tenantContext.TenantId };
            var providerHint = request.ProviderHint;

            if (string.IsNullOrWhiteSpace(providerHint))
            {
                var setting = await _fileStorageSettingsService
                    .ResolveAsync(request.TenantId, request.DocumentType, cancellationToken)
                    .ConfigureAwait(false);

                providerHint = setting?.ProviderType;
            }

            var provider = _providerResolver.Resolve(providerHint);

            return await provider.UploadAsync(request, cancellationToken).ConfigureAwait(false);
        }

    }
}
