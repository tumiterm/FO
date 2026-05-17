// <copyright file="IResourceService.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    12-03-2025 20:43 AM
// Purpose:         Defines the IResourceService interface.

using ForekOnline.Domain.ViewModels;

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Defines methods for managing resources, including uploading and retrieving resource data asynchronously.
    /// </summary>
    /// <remarks>This interface provides functionality for handling resource operations, such as uploading
    /// resources with metadata and retrieving a list of all uploaded resources. Implementations of this interface are
    /// expected to handle asynchronous operations and ensure thread safety where applicable.</remarks>
    public interface IResourceService
    {
        /// <summary>
        /// Uploads a resource asynchronously.
        /// </summary>
        /// <param name="model">The resource upload model containing file and metadata.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UploadResourceAsync(ResourceUploadViewModel model);

        /// <summary>
        /// Loads all resources asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<List<ResourceUploadViewModel>> LoadResourcesAsync();
    }
}
