// <copyright file="StudentImportJob.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    22/03/2026 00:00 AM
// Purpose:         Hangfire job handler for importing students into SQL Server

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Hangfire job that reads from the configured <see cref="IStudentDataSource"/>
    /// (API or SQLite) and upserts into the SQL Server <c>FO.Student</c> / <c>FO.Enrollment</c> tables.
    /// </summary>
    public class StudentImportJob : IStudentImportJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StudentImportJob> _logger;

        public StudentImportJob(IServiceScopeFactory scopeFactory, ILogger<StudentImportJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync(StudentImportPayload payload, CancellationToken ct = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            _logger.LogInformation("StudentImportJob started. Source={Source}, SingleIdentity={Identity}",
                payload.Source, payload.SingleIdentity ?? "(bulk)");

            try
            {
                if (payload.Source == "Direct" && payload.DirectData is not null)
                {
                    await ImportDirectAsync(payload, unitOfWork, ct);
                }
                else
                {
                    var dataSource = ResolveDataSource(scope, payload.Source);

                    if (!string.IsNullOrWhiteSpace(payload.SingleIdentity))
                    {
                        await ImportSingleAsync(payload, dataSource, unitOfWork, ct);
                    }
                    else
                    {
                        await ImportBulkAsync(dataSource, unitOfWork, ct);
                    }
                }

                _logger.LogInformation("StudentImportJob completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StudentImportJob failed.");
                throw;
            }
        }

        /// <summary>
        /// Imports a single student submitted directly from the enrollment form.
        /// </summary>
        private async Task ImportDirectAsync(StudentImportPayload payload, IUnitOfWork uow, CancellationToken ct)
        {
            var data = payload.DirectData!;
            var identity = data.IDNumber?.Trim() ?? data.PassportNumber?.Trim() ?? string.Empty;

            var existing = await FindExistingStudentAsync(uow, identity, ct);
            if (existing is not null)
            {
                _logger.LogInformation("Student {Identity} already exists as FoStudent {Id}. Skipping creation.", identity, existing.Id);

                if (payload.CourseId.HasValue)
                {
                    await EnsureEnrollmentAsync(uow, existing.Id, payload.CourseId.Value, ct);
                }
                return;
            }

            var studentNumber = await GenerateStudentNumberAsync(uow, ct);

            var foStudent = new StudentEntity
            {
                Id = Guid.NewGuid(),
                StudentNumber = studentNumber,
                FirstName = data.FirstName,
                MiddleName = data.MiddleName,
                LastName = data.LastName,
                IDNumber = data.IDNumber,
                PassportNumber = data.PassportNumber,
                StudyPermitNumber = data.StudyPermitNumber,
                DateOfBirth = data.DateOfBirth,
                Gender = data.Gender,
                Nationality = data.Nationality,
                Cellphone = data.Cellphone,
                Email = data.Email,
                HighestGrade = data.HighestGrade,
                NameOfSchool = data.NameOfSchool,
                StreetAddressLine1 = data.StreetAddressLine1,
                StreetAddressLine2 = data.StreetAddressLine2,
                AdmissionDate = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                IsActive = true,
                RegistrationSource = payload.OriginalApplicationId.HasValue ? "Application" : "WalkIn",
                OriginalApplicationId = payload.OriginalApplicationId
            };

            await uow.Students.AddAsync(foStudent, ct);

            if (payload.CourseId.HasValue)
            {
                await CreateEnrollmentAsync(uow, foStudent.Id, payload.CourseId.Value, ct);
            }

            await uow.SaveAsync();
            _logger.LogInformation("Created FoStudent {StudentNumber} from direct form data.", studentNumber);
        }

        /// <summary>
        /// Imports a single student from an external data source (API/SQLite).
        /// </summary>
        private async Task ImportSingleAsync(StudentImportPayload payload, IStudentDataSource source, IUnitOfWork uow, CancellationToken ct)
        {
            var student = await source.GetStudentByIdentityAsync(payload.SingleIdentity!, ct);
            if (student is null)
            {
                _logger.LogWarning("Student {Identity} not found in {Source}.", payload.SingleIdentity, source.SourceName);
                return;
            }

            await UpsertStudentAsync(uow, student, source.SourceName, payload.OriginalApplicationId, ct);

            if (payload.CourseId.HasValue)
            {
                var existing = await FindExistingStudentAsync(uow, payload.SingleIdentity!, ct);
                if (existing is not null)
                {
                    await EnsureEnrollmentAsync(uow, existing.Id, payload.CourseId.Value, ct);
                }
            }

            await uow.SaveAsync();
        }

        /// <summary>
        /// Bulk-imports all students from the data source into SQL Server.
        /// </summary>
        /// <summary>
        /// Bulk-imports all students from the data source into SQL Server.
        /// Each record is saved individually so a single failure doesn't block the rest.
        /// </summary>
        private async Task ImportBulkAsync(IStudentDataSource source, IUnitOfWork uow, CancellationToken ct)
        {
            var students = await source.GetAllStudentsAsync(ct);
            _logger.LogInformation("Bulk importing {Count} students from {Source}.", students.Count, source.SourceName);

            var deduplicated = students
                .Where(s => !string.IsNullOrWhiteSpace(s.IDNumber?.Trim() ?? s.PassportNumber?.Trim()))
                .GroupBy(s => (s.IDNumber?.Trim() ?? s.PassportNumber?.Trim())!, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

            _logger.LogInformation("After de-duplication: {Count} unique students to process.", deduplicated.Count);

            int created = 0, updated = 0, skipped = 0, failed = 0;
            var errors = new List<(string Identity, string Error)>();

            foreach (var student in deduplicated)
            {
                using var recordScope = _scopeFactory.CreateScope();
                var recordUow = recordScope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var identity = student.IDNumber?.Trim() ?? student.PassportNumber?.Trim() ?? "unknown";

                try
                {
                    var result = await UpsertStudentAsync(recordUow, student, source.SourceName, null, ct);

                    await recordUow.SaveAsync();

                    switch (result)
                    {
                        case UpsertResult.Created: created++; break;
                        case UpsertResult.Updated: updated++; break;
                        case UpsertResult.Skipped: skipped++; break;
                    }

                    // Log progress every 100 records
                    if ((created + updated + skipped + failed) % 100 == 0)
                    {
                        _logger.LogInformation(
                            "Import progress: Created={Created}, Updated={Updated}, Skipped={Skipped}, Failed={Failed}",
                            created, updated, skipped, failed);
                    }
                }
                catch (Exception ex)
                {
                    failed++;
                    errors.Add((identity, ex.Message));
                    _logger.LogWarning(ex, "Failed to import student {Identity} ({StudentNumber}). Moving to next record.", identity, student.StudentNumber);
                }
            }

            _logger.LogInformation("Bulk import complete. Created={Created}, Updated={Updated}, Skipped={Skipped}, Failed={Failed}",created, updated, skipped, failed);

            if (errors.Count > 0)
            {
                _logger.LogWarning("Failed records ({Count}): {Errors}", errors.Count, string.Join(" | ", errors.Take(20).Select(e => $"{e.Identity}: {e.Error}")));
            }
        }
        #region Helpers

        private async Task<UpsertResult> UpsertStudentAsync(IUnitOfWork uow, Student student, string source, Guid? applicationId, CancellationToken ct)
        {
            var identity = student.IDNumber?.Trim() ?? student.PassportNumber?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(identity))
                return UpsertResult.Skipped;

            var existing = await FindExistingStudentAsync(uow, identity, ct);

            if (existing is not null)
            {
                existing.FirstName = student.FirstName ?? existing.FirstName;
                existing.LastName = student.LastName ?? existing.LastName;
                existing.Email = student.Email ?? existing.Email;
                existing.Cellphone = student.Cellphone ?? existing.Cellphone;
                existing.IsActive = student.IsActive;
                existing.IsDeregistered = student.Deregistered;

                await uow.Students.Update(existing);

                // Upsert enrollments
                if (student.EnrollmentHistory is { Count: > 0 })
                {
                    foreach (var eh in student.EnrollmentHistory)
                    {
                        await EnsureEnrollmentAsync(uow, existing.Id, eh.CourseId, eh, ct);
                    }
                }

                return UpsertResult.Updated;
            }

            var studentNumber = !string.IsNullOrWhiteSpace(student.StudentNumber)
                ? student.StudentNumber
                : await GenerateStudentNumberAsync(uow, ct);

            var foStudent = new StudentEntity
            {
                Id = Guid.NewGuid(),
                StudentNumber = studentNumber,
                FirstName = student.FirstName ?? string.Empty,
                MiddleName = student.MiddleName,
                LastName = student.LastName ?? string.Empty,
                IDNumber = student.IDNumber,
                PassportNumber = student.PassportNumber,
                StudyPermitNumber = student.StudyPermitNumber,
                DateOfBirth = student.DateofBirth,
                Gender = student.Gender,
                Nationality = student.Nationality,
                Language = student.Language,
                AdmissionCategory = student.AdmissionCategory?.ToString(),
                StreetAddressLine1 = student.StreetAddressLine1,
                StreetAddressLine2 = student.StreetAddressLine2,
                Cellphone = student.Cellphone,
                Email = student.Email,
                HighestGrade = student.HighestGrade,
                NameOfSchool = student.NameofSchool,
                AdmissionDate = student.AdmissionDate,
                IsActive = student.IsActive,
                IsDeregistered = student.Deregistered,
                RegistrationSource = source,
                OriginalApplicationId = applicationId
            };

            await uow.Students.AddAsync(foStudent, ct);

            if (student.EnrollmentHistory is { Count: > 0 })
            {
                foreach (var eh in student.EnrollmentHistory)
                {
                    await CreateEnrollmentAsync(uow, foStudent.Id, eh.CourseId, eh, ct);
                }
            }

            return UpsertResult.Created;
        }

        private static async Task<StudentEntity?> FindExistingStudentAsync(IUnitOfWork uow, string identity, CancellationToken ct)
        {
            return await uow.Students.GetAsync(
                s => s.IDNumber == identity || s.PassportNumber == identity,
                asNoTracking: false,
                cancellationToken: ct);
        }

        private static async Task EnsureEnrollmentAsync(IUnitOfWork uow, Guid studentId, Guid courseId, CancellationToken ct)
        {
            await EnsureEnrollmentAsync(uow, studentId, courseId, null, ct);
        }

        private static async Task EnsureEnrollmentAsync(IUnitOfWork uow, Guid studentId, Guid courseId, EnrollmentHistory? eh, CancellationToken ct)
        {
            var exists = await uow.Enrollments.ExistsAsync(
                e => e.StudentId == studentId && e.CourseId == courseId,
                cancellationToken: ct);

            if (exists)
                return;

            await CreateEnrollmentAsync(uow, studentId, courseId, eh, ct);
        }

        private static async Task CreateEnrollmentAsync(IUnitOfWork uow, Guid studentId, Guid courseId, CancellationToken ct)
        {
            await CreateEnrollmentAsync(uow, studentId, courseId, null, ct);
        }

        private static async Task CreateEnrollmentAsync(IUnitOfWork uow, Guid studentId, Guid courseId, EnrollmentHistory? eh, CancellationToken ct)
        {
            var enrollment = new EnrollmentEntity
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                CourseId = courseId,
                CourseTitle = eh?.CourseTitle,
                CourseType = eh?.CourseType,
                EnrollmentStatus = eh?.EnrollmentStatus ?? "Active",
                StartDate = eh?.StartDate ?? DateTime.UtcNow,
                DateCompleted = eh?.DateCompleted,
                IsActive = eh?.IsActive ?? true
            };

            await uow.Enrollments.AddAsync(enrollment, ct);
        }

        private static async Task<string> GenerateStudentNumberAsync(IUnitOfWork uow, CancellationToken ct)
        {
            var year = DateTime.UtcNow.Year;
            var all = await uow.Students.GetAllAsync(
                filter: s => s.StudentNumber.StartsWith($"FIT-{year}-"),
                cancellationToken: ct);

            var nextSeq = all.Count + 1;
            return $"FIT-{year}-{nextSeq:D4}";
        }

        private IStudentDataSource ResolveDataSource(IServiceScope scope, string source)
        {
            var sources = scope.ServiceProvider.GetServices<IStudentDataSource>();
            return sources.FirstOrDefault(s => s.SourceName.Equals(source, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException($"No IStudentDataSource registered for '{source}'.");
        }

        private enum UpsertResult { Created, Updated, Skipped }

        #endregion
    }
}