// <copyright file="StudentCacheStore.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    14/03/2026 00:00 AM
// Purpose:         Implements the SQLite-backed student cache store.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Utility;
using ForekOnline.Domain.Entities;
using Microsoft.Extensions.Logging;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// SQLite-backed implementation of <see cref="IStudentCacheStore"/>.
    /// Handles full sync (replace-all) and read operations for offline resilience.
    /// </summary>
    public class StudentCacheStore : IStudentCacheStore
    {
        private readonly IStudentCacheRepository _repository;
        private readonly ILogger<StudentCacheStore> _logger;

        public StudentCacheStore(IStudentCacheRepository repository, ILogger<StudentCacheStore> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task SyncStudentsAsync(List<Student> students)
        {
            if (students == null || students.Count == 0)
            {
                _logger.LogWarning("SyncStudentsAsync called with empty list — skipping to preserve existing cache.");
                return;
            }

            try
            {
                var now = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

                var cachedStudents = students.Select(s => new CachedStudent
                {
                    StudentId = s.StudentId,
                    StudentNumber = s.StudentNumber ?? Helper.RandomStringGenerator(6),
                    AdmissionDate = s.AdmissionDate,
                    FirstName = s.FirstName ?? "Unknown",
                    MiddleName = s.MiddleName ?? "N/A",
                    LastName = s.LastName ?? "Unknown",
                    IDNumber = s.IDNumber ?? "N/A",
                    StudyPermitNumber = s.StudyPermitNumber ?? "N/A",
                    PassportNumber = s.PassportNumber ?? "N/A",
                    DateofBirth = s.DateOfBirth,
                    Gender = s.Gender,
                    PlaceofBirth = s.PlaceOfBirth ?? "N/A",
                    Nationality = s.Nationality ?? "N/A",
                    Language = s.Language ?? "N/A",
                    AdmissionCategory = s.AdmissionCategory,
                    StreetAddressLine1 = s.StreetAddressLine1 ?? "N/A",
                    StreetAddressLine2 = s.StreetAddressLine2 ?? "N/A",
                    Cellphone = s.Cellphone ?? "0000000000",
                    Email = s.Email ?? "N/A",
                    HighestGrade = s.HighestGrade ?? "Unknown",
                    NameofSchool = s.NameOfSchool ?? "N/A",
                    IsActive = s.IsActive,
                    Deregistered = s.Deregistered,
                    RegistrationSource = s.RegistrationSource ?? "Unknown",
                    Deregistrered = s.Deregistered,
                    LastSyncedUtc = now,
                    EnrollmentHistory = (s.EnrollmentHistory ?? new List<EnrollmentHistory>()).Select(e => new CachedEnrollmentHistory
                    {
                        EnrollmentId = e.EnrollmentId,
                        CachedStudentId = s.StudentId,
                        StudentId = e.StudentId,
                        CourseId = e.CourseId,
                        CourseTitle = e.CourseTitle,
                        CourseType = e.CourseType,
                        EnrollmentStatus = e.EnrollmentStatus,
                        StartDate = e.StartDate,
                        IsActive = e.IsActive,
                        DateCompleted = e.DateCompleted
                    }).ToList()
                }).ToList();

                await _repository.ReplaceAllAsync(cachedStudents);

                await _repository.UpsertSyncMetadataAsync(new SyncMetadata
                {
                    EntityName = "Student",
                    LastSyncUtc = now,
                    RecordCount = students.Count,
                    WasSuccessful = true
                });

                _logger.LogInformation("SQLite cache synced with {Count} students at {Time}.", students.Count, now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync students to SQLite cache.");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<List<Student>> GetCachedStudentsAsync()
        {
            var cached = await _repository.GetAllStudentsAsync();
            return cached.Select(MapToDomain).ToList();
        }

        /// <inheritdoc/>
        public async Task<Student> GetCachedStudentAsync(string studentNumber)
        {
            var cached = await _repository.GetStudentByNumberAsync(studentNumber);
            return cached == null ? null : MapToDomain(cached);
        }

        /// <inheritdoc/>
        public async Task<SyncMetadata> GetLastSyncInfoAsync()
        {
            return await _repository.GetSyncMetadataAsync("Student");
        }

        /// <summary>
        /// Maps a <see cref="CachedStudent"/> back to the domain <see cref="Student"/> model.
        /// </summary>
        private static Student MapToDomain(CachedStudent cached)
        {
            return new Student
            {
                StudentId = cached.StudentId,
                StudentNumber = cached.StudentNumber,
                AdmissionDate = cached.AdmissionDate,
                FirstName = cached.FirstName,
                MiddleName = cached.MiddleName,
                LastName = cached.LastName,
                IDNumber = cached.IDNumber,
                StudyPermitNumber = cached.StudyPermitNumber,
                PassportNumber = cached.PassportNumber,
                DateOfBirth = cached.DateofBirth,
                Gender = cached.Gender,
                PlaceOfBirth = cached.PlaceofBirth,
                Nationality = cached.Nationality,
                Language = cached.Language,
                AdmissionCategory = cached.AdmissionCategory,
                StreetAddressLine1 = cached.StreetAddressLine1,
                StreetAddressLine2 = cached.StreetAddressLine2,
                Cellphone = cached.Cellphone,
                Email = cached.Email,
                HighestGrade = cached.HighestGrade,
                NameOfSchool = cached.NameofSchool,
                IsActive = cached.IsActive,
                Deregistered = cached.Deregistered,
                RegistrationSource = cached.RegistrationSource,
                EnrollmentHistory = cached.EnrollmentHistory?.Select(e => new EnrollmentHistory
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    CourseTitle = e.CourseTitle,
                    CourseType = e.CourseType,
                    EnrollmentStatus = e.EnrollmentStatus,
                    StartDate = e.StartDate,
                    IsActive = e.IsActive,
                    DateCompleted = e.DateCompleted
                }).ToList()
            };
        }
    }
}