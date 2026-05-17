// <copyright file="FileStorageRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    10-01-2026 18:06 PM
// Purpose:         Defines the FileStorageRepository Repository.

#region Using Directives
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the fileStorage Repository.
    /// </summary>
    public class FileStorageRepository : Repository<FileStorageSetting>, IFileStorage
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public FileStorageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing fileStorage model in the repository.
        /// </summary>
        /// <param name="fileStorage">The fileStorage model to be updated.</param>
        public async Task<FileStorageSetting> UpdateFileStorageSettingAsync(FileStorageSetting fileStorage)
        {
            _context.StorageSettings.Update(fileStorage);

            await _context.SaveChangesAsync();

            return fileStorage;
        }
    }
}
