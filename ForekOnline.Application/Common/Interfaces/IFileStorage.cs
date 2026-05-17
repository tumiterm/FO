// <copyright file="IAddress.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    10-01-2026 17:57 MM
// Purpose:         Defines the IFileStorage interface.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for managing and updating fileStorageSetting entities.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to fileStorageSetting management.</remarks>
    public interface IFileStorage : IRepository<FileStorageSetting>
    {
        /// <summary>
        /// Updates the specified fileStorageSetting in the system and returns the updated fileStorageSetting.
        /// </summary>
        /// <param name="fileStorageSetting">The <see cref="FileStorageSetting"/> object containing the updated fileStorageSetting details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="FileStorageSetting"/> object.</returns>
        Task<FileStorageSetting> UpdateFileStorageSettingAsync(FileStorageSetting fileStorageSetting);
    }
}
