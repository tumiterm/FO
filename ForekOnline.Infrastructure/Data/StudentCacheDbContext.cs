// <copyright file="StudentCacheDbContext.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    14/03/2026 21:12 PM
// Purpose:         Defines the StudentCacheDbContext for SQLite-based API data caching.

#region Using
using ForekOnline.Domain.Entities;
using Microsoft.EntityFrameworkCore;
#endregion

namespace ForekOnline.Infrastructure.Data
{
    /// <summary>
    /// A lightweight SQLite database context used to cache student data from the external API.
    /// This provides offline resilience when the API is unavailable.
    /// </summary>
    public class StudentCacheDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StudentCacheDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to configure the database context.</param>
        public StudentCacheDbContext(DbContextOptions<StudentCacheDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the cached students from the external API.
        /// </summary>
        public DbSet<CachedStudent> Students { get; set; }

        /// <summary>
        /// Gets or sets the cached enrollment history records.
        /// </summary>
        public DbSet<CachedEnrollmentHistory> EnrollmentHistories { get; set; }

        /// <summary>
        /// Gets or sets the sync metadata for tracking last successful sync.
        /// </summary>
        public DbSet<SyncMetadata> SyncMetadata { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CachedStudent>(entity =>
            {
                entity.HasKey(e => e.StudentId);
                entity.HasIndex(e => e.StudentNumber).IsUnique();
                entity.HasIndex(e => e.IDNumber);
                entity.HasIndex(e => e.PassportNumber);
                entity.HasIndex(e => e.IsActive);

                entity.HasMany(e => e.EnrollmentHistory)
                    .WithOne()
                    .HasForeignKey(e => e.CachedStudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CachedEnrollmentHistory>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);
                entity.HasIndex(e => e.CourseId);
                entity.HasIndex(e => e.IsActive);
            });

            modelBuilder.Entity<SyncMetadata>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}
