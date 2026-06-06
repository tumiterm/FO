// <copyright file="StudentImportSchemaService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

using ForekOnline.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ForekOnline.Infrastructure.Data
{
    /// <summary>
    /// Applies narrowly scoped compatibility updates to the legacy student tables.
    /// </summary>
    public sealed class StudentImportSchemaService : IStudentImportSchemaService
    {
        internal const string EnsureStudentImportSchemaSql = """
            DECLARE @schemaLockResult int;

            EXEC @schemaLockResult = sys.sp_getapplock
                @Resource = N'ForekOnline.StudentImport.SchemaCompatibility',
                @LockMode = N'Exclusive',
                @LockOwner = N'Session',
                @LockTimeout = 60000;

            IF @schemaLockResult < 0
                THROW 51000, 'Could not acquire the student import schema compatibility lock.', 1;

            BEGIN TRY
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.columns AS columns
                    INNER JOIN sys.tables AS tables ON tables.object_id = columns.object_id
                    INNER JOIN sys.schemas AS schemas ON schemas.schema_id = tables.schema_id
                    WHERE schemas.name = N'Academics'
                      AND tables.name = N'Students'
                      AND columns.name = N'GuardianId'
                      AND columns.is_nullable = 0
                )
                BEGIN
                    DECLARE @guardianForeignKey sysname;

                    SELECT TOP (1) @guardianForeignKey = foreignKeys.name
                    FROM sys.foreign_keys AS foreignKeys
                    INNER JOIN sys.foreign_key_columns AS foreignKeyColumns
                        ON foreignKeyColumns.constraint_object_id = foreignKeys.object_id
                    INNER JOIN sys.columns AS columns
                        ON columns.object_id = foreignKeyColumns.parent_object_id
                       AND columns.column_id = foreignKeyColumns.parent_column_id
                    WHERE foreignKeys.parent_object_id = OBJECT_ID(N'[Academics].[Students]')
                      AND columns.name = N'GuardianId';

                    IF @guardianForeignKey IS NOT NULL
                    BEGIN
                        DECLARE @dropForeignKeySql nvarchar(max) =
                            N'ALTER TABLE [Academics].[Students] DROP CONSTRAINT ' + QUOTENAME(@guardianForeignKey) + N';';

                        EXEC sys.sp_executesql @dropForeignKeySql;
                    END

                    ALTER TABLE [Academics].[Students]
                        ALTER COLUMN [GuardianId] uniqueidentifier NULL;

                    IF NOT EXISTS
                    (
                        SELECT 1
                        FROM sys.foreign_key_columns AS foreignKeyColumns
                        INNER JOIN sys.columns AS columns
                            ON columns.object_id = foreignKeyColumns.parent_object_id
                           AND columns.column_id = foreignKeyColumns.parent_column_id
                        WHERE foreignKeyColumns.parent_object_id = OBJECT_ID(N'[Academics].[Students]')
                          AND columns.name = N'GuardianId'
                    )
                    BEGIN
                        ALTER TABLE [Academics].[Students] WITH CHECK
                            ADD CONSTRAINT [FK_Students_Guardians_GuardianId]
                            FOREIGN KEY ([GuardianId]) REFERENCES [dbo].[Guardians] ([GuardianId]);
                    END
                END

                -- Older deployments created this FK against the obsolete Students.StudentId
                -- compatibility column. Imports use StudentEntity.Id, so migrate any existing
                -- references and recreate the relationship against the actual entity key.
                DECLARE @legacyEnrollmentForeignKey sysname;

                SELECT TOP (1) @legacyEnrollmentForeignKey = foreignKeys.name
                FROM sys.foreign_keys AS foreignKeys
                INNER JOIN sys.foreign_key_columns AS foreignKeyColumns
                    ON foreignKeyColumns.constraint_object_id = foreignKeys.object_id
                INNER JOIN sys.columns AS parentColumns
                    ON parentColumns.object_id = foreignKeyColumns.parent_object_id
                   AND parentColumns.column_id = foreignKeyColumns.parent_column_id
                INNER JOIN sys.columns AS referencedColumns
                    ON referencedColumns.object_id = foreignKeyColumns.referenced_object_id
                   AND referencedColumns.column_id = foreignKeyColumns.referenced_column_id
                WHERE foreignKeys.parent_object_id = OBJECT_ID(N'[Academics].[EnrollmentHistory]')
                  AND foreignKeys.referenced_object_id = OBJECT_ID(N'[Academics].[Students]')
                  AND parentColumns.name = N'StudentId'
                  AND referencedColumns.name = N'StudentId';

                IF @legacyEnrollmentForeignKey IS NOT NULL
                BEGIN
                    DECLARE @dropEnrollmentForeignKeySql nvarchar(max) =
                        N'ALTER TABLE [Academics].[EnrollmentHistory] DROP CONSTRAINT '
                        + QUOTENAME(@legacyEnrollmentForeignKey) + N';';

                    EXEC sys.sp_executesql @dropEnrollmentForeignKeySql;

                    UPDATE enrollmentHistory
                    SET enrollmentHistory.StudentId = students.Id
                    FROM [Academics].[EnrollmentHistory] AS enrollmentHistory
                    INNER JOIN [Academics].[Students] AS students
                        ON students.StudentId = enrollmentHistory.StudentId
                    WHERE enrollmentHistory.StudentId <> students.Id;
                END

                IF OBJECT_ID(N'[Academics].[EnrollmentHistory]', N'U') IS NOT NULL
                   AND OBJECT_ID(N'[Academics].[Students]', N'U') IS NOT NULL
                   AND NOT EXISTS
                   (
                       SELECT 1
                       FROM sys.foreign_key_columns AS foreignKeyColumns
                       INNER JOIN sys.columns AS parentColumns
                           ON parentColumns.object_id = foreignKeyColumns.parent_object_id
                          AND parentColumns.column_id = foreignKeyColumns.parent_column_id
                       INNER JOIN sys.columns AS referencedColumns
                           ON referencedColumns.object_id = foreignKeyColumns.referenced_object_id
                          AND referencedColumns.column_id = foreignKeyColumns.referenced_column_id
                       WHERE foreignKeyColumns.parent_object_id = OBJECT_ID(N'[Academics].[EnrollmentHistory]')
                         AND foreignKeyColumns.referenced_object_id = OBJECT_ID(N'[Academics].[Students]')
                         AND parentColumns.name = N'StudentId'
                         AND referencedColumns.name = N'Id'
                   )
                BEGIN
                    ALTER TABLE [Academics].[EnrollmentHistory] WITH CHECK
                        ADD CONSTRAINT [FK_EnrollmentHistory_Students]
                        FOREIGN KEY ([StudentId]) REFERENCES [Academics].[Students] ([Id])
                        ON DELETE CASCADE;
                END

                EXEC sys.sp_releaseapplock
                    @Resource = N'ForekOnline.StudentImport.SchemaCompatibility',
                    @LockOwner = N'Session';
            END TRY
            BEGIN CATCH
                EXEC sys.sp_releaseapplock
                    @Resource = N'ForekOnline.StudentImport.SchemaCompatibility',
                    @LockOwner = N'Session';
                THROW;
            END CATCH
            """;

        private readonly ApplicationDbContext _db;
        private readonly ILogger<StudentImportSchemaService> _logger;

        public StudentImportSchemaService(
            ApplicationDbContext db,
            ILogger<StudentImportSchemaService> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task EnsureCompatibleAsync(CancellationToken ct = default)
        {
            if (!_db.Database.IsSqlServer())
            {
                return;
            }

            var rowsAffected = await _db.Database.ExecuteSqlRawAsync(EnsureStudentImportSchemaSql, ct);
            _logger.LogInformation(
                "Verified student import schema compatibility. RowsAffected={RowsAffected}.",
                rowsAffected);
        }
    }
}
