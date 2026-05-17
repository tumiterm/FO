// <copyright file="IBlobFileService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    16/02/2025 06:22 AM
// Purpose:         Defines the IBlobFileService interface

using Microsoft.AspNetCore.Http;

namespace ForekOnline.Application.Common.Services
{
    public interface IBlobFileService
    {
        /// <summary>
        /// Uploads a file to Azure Blob Storage.
        /// </summary>
        /// <param name="file">The file file to upload.</param>
        /// <returns>The URL of the uploaded file.</returns>
        /// <exception cref="ArgumentException">Thrown when the profile image is null or empty.</exception>
        /// <exception cref="ApplicationException">Thrown when an error occurs during upload.</exception>
        Task<string> UploadAttachmentAsync(IFormFile fileName, string containerName);

        /// <summary>
        /// Downloads a profile image from Azure Blob Storage.
        /// </summary>
        /// <param name="blobName">The name of the blob to download.</param>
        /// <returns>A stream containing the downloaded profile image.</returns>
        /// <exception cref="ArgumentException">Thrown when the blob name is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the requested blob does not exist.</exception>
        Task<Stream> DownloadAttachmentAsync(string blobName, string containerName);

        /// <summary>
        /// Deletes a file from Azure Blob Storage if it exists.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <param name="containerName">The name of the blob container.</param>
        /// <returns>Returns <c>true</c> if the file was deleted, <c>false</c> if the file was not found.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="fileName"/> is null or empty.</exception>
        /// <exception cref="ApplicationException">Thrown when an error occurs while deleting the file.</exception>
        Task<bool> DeleteFileAsync(string fileName, string containerName);
    }
}
