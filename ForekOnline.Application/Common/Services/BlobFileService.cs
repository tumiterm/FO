// <copyright file="BlobFileService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    16/02/2025 06:25 AM
// Purpose:         Defines the BlobFileService class

#region Usings
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides functionality to handle file storage operations in Azure Blob Storage,
    /// including uploading and downloading files.
    /// </summary>
    public class BlobFileService : IBlobFileService
    {
        #region Fields
        private readonly IHelperService _helperService;
        private readonly string _connectionString;
        #endregion

        /// <summary>
        /// Uploads a file to Azure Blob Storage.
        /// </summary>
        /// <returns>The URL of the uploaded profile image.</returns>
        /// <exception cref="ArgumentException">Thrown when the profile image is null or empty.</exception>
        /// <exception cref="ApplicationException">Thrown when an error occurs during upload.</exception>
        public BlobFileService(IHelperService helperService)
        {
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _connectionString = _helperService.GetConfigurationValue("AzureStorage:ConnectionString", string.Empty);
        }

        /// <summary>
        /// Uploads a file to Azure Blob Storage.
        /// </summary>
        /// <param name="file">The file file to upload.</param>
        /// <returns>The URL of the uploaded file.</returns>
        /// <exception cref="ArgumentException">Thrown when the file is null or empty.</exception>
        /// <exception cref="ApplicationException">Thrown when an error occurs during upload.</exception>
        public async Task<string> UploadAttachmentAsync(IFormFile file, string containerName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("file is required.", nameof(file));

            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var blobContainerClient = new BlobContainerClient(_connectionString, containerName);

            var blobClient = blobContainerClient.GetBlobClient(uniqueFileName);

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    await blobClient.UploadAsync(memoryStream);
                }

                return blobClient.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while uploading the file.", ex);
            }
        }

        /// <summary>
        /// Downloads a file from Azure Blob Storage.
        /// </summary>
        /// <param name="blobName">The name of the blob to download.</param>
        /// <returns>A stream containing the downloaded file.</returns>
        /// <exception cref="ArgumentException">Thrown when the blob name is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the requested blob does not exist.</exception>
        public async Task<Stream> DownloadAttachmentAsync(string blobName, string containerName)
        {
            if (string.IsNullOrEmpty(blobName))
                throw new ArgumentException("Blob name is required.", nameof(blobName));

            var blobContainerClient = new BlobContainerClient(_connectionString, containerName);

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                var downloadInfo = await blobClient.DownloadAsync();

                return downloadInfo.Value.Content;
            }
            else
            {
                throw new FileNotFoundException("The requested image does not exist.");
            }
        }

        /// <summary>
        /// Deletes a file from Azure Blob Storage if it exists.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <param name="containerName">The name of the blob container.</param>
        /// <returns>Returns <c>true</c> if the file was deleted, <c>false</c> if the file was not found.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="fileName"/> is null or empty.</exception>
        /// <exception cref="ApplicationException">Thrown when an error occurs while deleting the file.</exception>
        public async Task<bool> DeleteFileAsync(string fileName, string containerName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required.", nameof(fileName));

            var blobContainerClient = new BlobContainerClient(_connectionString, containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            try
            {
                var response = await blobClient.DeleteIfExistsAsync();
                return response.Value;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while deleting the file.", ex);
            }
        }
    }
}
