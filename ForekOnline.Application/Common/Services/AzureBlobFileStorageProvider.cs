// <copyright file="AzureBlobFileStorageProvider.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    10/01/2026 21:16:27 PM
// Purpose:         Defines the AzureBlobFileStorageProvider class

#region Using Directives
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using System.Text.Json;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides an implementation of the IFileStorageProvider interface that stores and manages files using Azure Blob
    /// Storage.
    /// </summary>
    /// <remarks>This provider enables asynchronous upload, download, and deletion of files in Azure Blob
    /// Storage, and manages associated document records in a backing data store. Configuration values for the Azure
    /// Storage connection string and container name must be supplied via the provided helper service. Presigned URL
    /// generation is not supported by this provider. All operations are performed asynchronously and support
    /// cancellation via CancellationToken.</remarks>
    public class AzureBlobFileStorageProvider : IFileStorageProvider
    {
        #region Private Fields
        private const string Provider = "AzureBlob";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHelperService _helperService;

        private readonly string _connectionString;
        private readonly string _containerName;
        #endregion

        /// <summary>
        /// Initializes a new instance of the AzureBlobFileStorageProvider class using the specified unit of work and
        /// helper service.
        /// </summary>
        /// <remarks>This constructor retrieves the Azure Storage connection string and container name
        /// from the provided helper service. Both configuration values must be present for the provider to function
        /// correctly.</remarks>
        /// <param name="unitOfWork">The unit of work instance used to manage database transactions and operations.</param>
        /// <param name="helperService">The helper service used to retrieve configuration values required for Azure Blob Storage access.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the unitOfWork or helperService parameter is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the required Azure Storage connection string or container name configuration values are missing or
        /// empty.</exception>
        public AzureBlobFileStorageProvider(IUnitOfWork unitOfWork, IHelperService helperService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));

            _connectionString = _helperService.GetConfigurationValue("AzureStorage:ConnectionString", string.Empty);
            _containerName = _helperService.GetConfigurationValue("AzureStorage:Containers:Documents", string.Empty);

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("AzureStorage:ConnectionString is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_containerName))
            {
                throw new InvalidOperationException("AzureStorage:Containers:Documents is not configured.");
            }
        }

        /// <summary>
        /// Gets the name of the data provider associated with the current instance.
        /// </summary>
        public string ProviderName => Provider;

        /// <summary>
        /// Asynchronously deletes the specified file and marks its associated document as deleted.
        /// </summary>
        /// <remarks>If the specified file does not exist or is already marked as deleted, the method
        /// completes without performing any action. The associated document is marked as deleted even if the underlying
        /// blob does not exist.</remarks>
        /// <param name="fileId">The unique identifier of the file to delete. Must be a valid GUID string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the delete operation.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="fileId"/> is null, empty, or not a valid GUID.</exception>
        public async Task DeleteAsync(string fileId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException("fileId is required.", nameof(fileId));

            if (!Guid.TryParse(fileId, out var documentId))
                throw new ArgumentException("fileId must be a valid Guid.", nameof(fileId));

            var stored = await _unitOfWork.StoredDocument
                .GetAsync(
                    d => d.Id == documentId && !d.IsDeleted,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (stored is null)
                return;

            var containerClient = new BlobContainerClient(_connectionString, _containerName);
            var blobClient = containerClient.GetBlobClient(stored.ProviderKey);

            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            stored.IsDeleted = true;
            stored.ModifiedOn = DateTimeOffset.UtcNow;
            stored.ModifiedBy = "system";

            await _unitOfWork.StoredDocument.AddAsync(stored, cancellationToken);
            await _unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Asynchronously downloads a file by its unique identifier and returns its content and metadata.
        /// </summary>
        /// <param name="fileId">The unique identifier of the file to download. Must be a valid GUID string.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the download operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="DownloadFileResponse"/> with the file's content stream and associated metadata.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="fileId"/> is null, empty, or not a valid GUID.</exception>
        /// <exception cref="FileNotFoundException">Thrown if no file with the specified <paramref name="fileId"/> exists.</exception>
        /// <exception cref="NotImplementedException">Thrown if the method is not implemented.</exception>
        public async Task<DownloadFileResponse> DownloadAsync(string fileId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileId))
                throw new ArgumentException("fileId is required.", nameof(fileId));

            if (!Guid.TryParse(fileId, out var documentId))
                throw new ArgumentException("fileId must be a valid Guid.", nameof(fileId));

            var stored = await _unitOfWork.StoredDocument
                .GetAsync(
                    d => d.Id == documentId && !d.IsDeleted,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (stored is null)
                throw new FileNotFoundException("The requested file does not exist.");

            var containerClient = new BlobContainerClient(_connectionString, _containerName);
            var blobClient = containerClient.GetBlobClient(stored.ProviderKey);

            var download = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            return new DownloadFileResponse(
                FileStream: download.Value.Content,
                FileName: stored.FileName,
                FileSizeBytes: stored.FileSizeBytes,
                ContentType: stored.ContentType);

            throw new NotImplementedException("DownloadAsync is not implemented yet.");
        }

        /// <summary>
        /// Asynchronously generates a presigned URL that provides temporary access to the specified file.
        /// </summary>
        /// <remarks>This method is not implemented for Azure Blob storage and will always throw a
        /// NotSupportedException. Presigned URL generation requires access to storage account credentials, which are
        /// not available in this context.</remarks>
        /// <param name="fileId">The unique identifier of the file for which to generate the presigned URL. Cannot be null or empty.</param>
        /// <param name="expiryInMinutes">The duration, in minutes, for which the presigned URL will remain valid. Must be a positive integer.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the presigned URL as a string.</returns>
        /// <exception cref="NotSupportedException">Always thrown to indicate that presigned URL generation is not supported for Azure Blob storage.</exception>
        public async Task<string> GeneratePresignedUrlAsync(string fileId, int expiryInMinutes, CancellationToken cancellationToken = default)
        {
            // Azure SAS generation not implemented here because it needs StorageSharedKeyCredential (account key),
            throw new NotSupportedException("Presigned URL generation is not implemented for AzureBlob yet.");
        }

        /// <summary>
        /// Asynchronously uploads a file to the storage provider and creates a corresponding document record.
        /// </summary>
        /// <remarks>The uploaded file is stored in the configured storage container, and a new document
        /// record is created in the data store. The file stream position is reset to the beginning if it is seekable.
        /// The method does not overwrite existing files; a unique identifier is generated for each upload.</remarks>
        /// <param name="request">The file upload request containing the file stream, file name, content type, optional metadata, and other
        /// upload parameters. Cannot be null. The FileStream and FileName properties must be set.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the upload operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an UploadFileResponse with
        /// details about the uploaded file, including its identifier, upload timestamp, provider name, file size, file
        /// URL, and any additional metadata.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the request parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the FileStream or FileName property of the request is null or empty.</exception>
        public async Task<UploadFileResponse> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (request.FileStream is null) throw new ArgumentException("FileStream is required.", nameof(request));
            if (string.IsNullOrWhiteSpace(request.FileName)) throw new ArgumentException("FileName is required.", nameof(request));

            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(request.FileName);
            var blobName = string.IsNullOrWhiteSpace(extension)
                ? fileId.ToString("D")
                : $"{fileId:D}{extension}";

            var containerClient = new BlobContainerClient(_connectionString, _containerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            var blobClient = containerClient.GetBlobClient(blobName);

            if (request.FileStream.CanSeek)
            {
                request.FileStream.Position = 0;
            }

            var headers = new BlobHttpHeaders
            {
                ContentType = request.ContentType
            };

            await blobClient.UploadAsync(
               request.FileStream,
               new BlobUploadOptions
               {
                   HttpHeaders = headers
               },
               cancellationToken).ConfigureAwait(false);

            var metadata = request.Metadata is null ? null : JsonSerializer.Serialize(request.Metadata);

            var row = new StoredDocument
            {
                Id = fileId,
                ProviderName = ProviderName,
                ProviderKey = blobName,
                FileName = request.FileName,
                ContentType = request.ContentType,
                FileSizeBytes = TryGetLength(request.FileStream),
                MetadataJson = metadata,
                ExpiryDate = request.ExpiryDate,
                IsDeleted = false,
                IsActive = true,
                CreatedOn = DateTimeOffset.UtcNow,
                CreatedBy = "system"
            };

            await _unitOfWork.StoredDocument.AddAsync(row, cancellationToken);
            await _unitOfWork.SaveAsync().ConfigureAwait(false);

            return new UploadFileResponse(
               FileId: fileId.ToString("D"),
               UploadTimestamp: DateTimeOffset.UtcNow,
               ProviderName: ProviderName,
               FileSizeBytes: row.FileSizeBytes,
               FileUrl: blobClient.Uri.AbsoluteUri,
               AdditionalMetadata: request.Metadata);

        }

        #region Private Methods

        /// <summary>
        /// Attempts to retrieve the length of the specified stream.
        /// </summary>
        /// <remarks>If the stream does not support seeking or is null, this method returns 0. No
        /// exception is thrown for non-seekable or null streams.</remarks>
        /// <param name="stream">The stream from which to obtain the length. The stream must support seeking to determine its length.</param>
        /// <returns>The length of the stream in bytes if the stream is not null and supports seeking; otherwise, 0.</returns>
        private static long TryGetLength(Stream stream)
        {
            if (stream is null) return 0;
            if (stream.CanSeek) return stream.Length;
            return 0;
        }
        #endregion
    }
}
