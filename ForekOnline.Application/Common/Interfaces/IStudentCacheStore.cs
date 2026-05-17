// <copyright file="IStudentCacheStore.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    14/03/2026 00:00 AM
// Purpose:         Defines the contract for the SQLite student cache store.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Provides methods to read from and write to the local SQLite student cache.
    /// </summary>
    public interface IStudentCacheStore
    {
        /// <summary>
        /// Replaces all cached students with the provided API data (full sync).
        /// </summary>
        /// <param name="students">The student list retrieved from the API.</param>
        /// <returns>A task representing the asynchronous sync operation.</returns>
        Task SyncStudentsAsync(List<Student> students);

        /// <summary>
        /// Retrieves all cached students from SQLite, mapped back to domain <see cref="Student"/> objects.
        /// </summary>
        /// <returns>A list of students from the local cache, or an empty list if none exist.</returns>
        Task<List<Student>> GetCachedStudentsAsync();

        /// <summary>
        /// Retrieves a single cached student by student number.
        /// </summary>
        /// <param name="studentNumber">The student number to look up.</param>
        /// <returns>The cached student, or null if not found.</returns>
        Task<Student> GetCachedStudentAsync(string studentNumber);

        /// <summary>
        /// Gets metadata about the last sync operation.
        /// </summary>
        /// <returns>The sync metadata, or null if no sync has occurred.</returns>
        Task<SyncMetadata> GetLastSyncInfoAsync();
    }
}