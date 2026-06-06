// <copyright file="StudentCacheRepository.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    14/03/2026 22:00 PM
// Purpose:         Implements the SQLite student cache repository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Repository for the SQLite-backed student cache context.
    /// Encapsulates all data access for <see cref="StudentCacheDbContext"/>.
    /// </summary>
    public class StudentCacheRepository : IStudentCacheRepository
    {
        private readonly StudentCacheDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentCacheRepository"/> class.
        /// </summary>
        /// <param name="db">The SQLite student cache database context.</param>
        public StudentCacheRepository(StudentCacheDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<CachedStudent>> GetAllStudentsAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Students
                .Include(s => s.EnrollmentHistory)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<CachedStudent?> GetStudentByNumberAsync(string studentNumber, CancellationToken cancellationToken = default)
        {
            return await _db.Students
                .Include(s => s.EnrollmentHistory)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudentNumber == studentNumber, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task ReplaceAllAsync(List<CachedStudent> students, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            _db.EnrollmentHistories.RemoveRange(_db.EnrollmentHistories);
            _db.Students.RemoveRange(_db.Students);
            await _db.SaveChangesAsync(cancellationToken);

            await _db.Students.AddRangeAsync(students, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<SyncMetadata?> GetSyncMetadataAsync(string entityName, CancellationToken cancellationToken = default)
        {
            return await _db.SyncMetadata
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.EntityName == entityName, cancellationToken);
        }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public async Task<(int StudentCount, int EnrollmentHistoryCount)> GetCountsAsync(CancellationToken cancellationToken = default)
        {
            var studentCount = await _db.Students.CountAsync(cancellationToken);
            var enrollmentHistoryCount = await _db.EnrollmentHistories.CountAsync(cancellationToken);
            return (studentCount, enrollmentHistoryCount);
        }

        /// <inheritdoc/>
        public string GetDatabasePath()
            => _db.Database.GetDbConnection().DataSource;

        public async Task UpsertSyncMetadataAsync(SyncMetadata metadata, CancellationToken cancellationToken = default)
        {
            var existing = await _db.SyncMetadata
                .FirstOrDefaultAsync(m => m.EntityName == metadata.EntityName, cancellationToken);

            if (existing == null)
            {
                _db.SyncMetadata.Add(metadata);
            }
            else
            {
                existing.LastSyncUtc = metadata.LastSyncUtc;
                existing.RecordCount = metadata.RecordCount;
                existing.WasSuccessful = metadata.WasSuccessful;
            }

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}