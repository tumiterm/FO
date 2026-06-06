// <copyright file="IStudentCacheRepository.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    14/03/2026 00:00 AM
// Purpose:         Defines the repository contract for the SQLite student cache.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Provides data access methods for the SQLite-backed student cache.
    /// This is a focused repository for the cache context — not part of the main <see cref="IUnitOfWork"/>.
    /// </summary>
    public interface IStudentCacheRepository
    {
        /// <summary>
        /// Retrieves all cached students with their enrollment history.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>A read-only list of cached students.</returns>
        Task<IReadOnlyList<CachedStudent>> GetAllStudentsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a single cached student by student number, including enrollment history.
        /// </summary>
        /// <param name="studentNumber">The student number to look up.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>The cached student, or null if not found.</returns>
        Task<CachedStudent?> GetStudentByNumberAsync(string studentNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Replaces all cached student data with the provided list in a single transaction.
        /// </summary>
        /// <param name="students">The full list of cached students to persist.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        Task ReplaceAllAsync(List<CachedStudent> students, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the sync metadata for a given entity name.
        /// </summary>
        /// <param name="entityName">The entity name to look up (e.g., "Student").</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>The sync metadata, or null if no sync has occurred.</returns>
        Task<SyncMetadata?> GetSyncMetadataAsync(string entityName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or updates the sync metadata for a given entity.
        /// </summary>
        /// <param name="metadata">The sync metadata to upsert.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        Task UpsertSyncMetadataAsync(SyncMetadata metadata, CancellationToken cancellationToken = default);

        /// <summary>Gets current SQLite student and enrollment row counts.</summary>
        Task<(int StudentCount, int EnrollmentHistoryCount)> GetCountsAsync(CancellationToken cancellationToken = default);

        /// <summary>Gets the configured SQLite database path.</summary>
        string GetDatabasePath();
    }
}