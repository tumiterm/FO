// <copyright file="StudentCacheRefreshService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Serialization;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Downloads student records directly from the legacy API and replaces the SQLite cache
    /// with every record that can be loaded successfully. Failed records are skipped independently.
    /// This deliberately bypasses SQL Server and the in-memory student cache.
    /// </summary>
    public sealed class StudentCacheRefreshService : IStudentCacheRefreshService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHelperService _helperService;
        private readonly IStudentCacheStore _cacheStore;
        private readonly ILogger<StudentCacheRefreshService> _logger;
        private readonly string _apiBaseAddress;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters =
            {
                new LegacyEnumJsonConverter<eGender>(eGender.Other),
                new LegacyEnumJsonConverter<eAdmissionCategory>(eAdmissionCategory.FullTime),
                new LegacyEnumJsonConverter<eRelationship>(eRelationship.Other),
                new LegacyEnumJsonConverterFactory()
            }
        };

        public StudentCacheRefreshService(
            IHttpClientFactory httpClientFactory,
            IHelperService helperService,
            IStudentCacheStore cacheStore,
            ILogger<StudentCacheRefreshService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _helperService = helperService;
            _cacheStore = cacheStore;
            _logger = logger;
            _apiBaseAddress = helperService
                .GetConfigurationValue("AppSettings:ApiBaseAddress", "defaultApiBaseAddress")
                .TrimEnd('/') + "/";
        }

        /// <inheritdoc/>
        public async Task<StudentCacheRefreshResult> RefreshFromApiAsync(CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient(nameof(StudentCacheRefreshService));
            client.BaseAddress ??= new Uri(_apiBaseAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await _helperService.GenerateJwtToken());

            var students = await GetAsync<List<Student>>(client, "Students", cancellationToken)
                ?? throw new InvalidOperationException("The legacy API returned no student payload.");

            if (students.Count == 0)
            {
                throw new InvalidOperationException(
                    "The legacy API returned zero students. The existing SQLite cache was preserved.");
            }

            var detailFailures = new List<string>();
            var detailedStudents = new List<Student>(students.Count);

            // Process one student at a time so an upstream failure is isolated to that
            // record and does not prevent later students from reaching SQLite.
            foreach (var student in students)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(student.StudentNumber))
                {
                    detailFailures.Add("Student with no student number was skipped");
                    _logger.LogWarning(
                        "Skipping student {StudentId} during SQLite refresh because the student number is missing.",
                        student.StudentId);
                    continue;
                }

                try
                {
                    var path = $"Student?StudentNumber={WebUtility.UrlEncode(student.StudentNumber)}";
                    var detailed = await GetAsync<Student>(client, path, cancellationToken);
                    if (detailed == null)
                    {
                        detailFailures.Add($"{student.StudentNumber}: no detail record returned");
                        _logger.LogWarning(
                            "Skipping student {StudentNumber} because the legacy API returned no detail record.",
                            student.StudentNumber);
                        continue;
                    }

                    detailedStudents.Add(detailed);
                }
                catch (Exception ex) when (ex is HttpRequestException or JsonException)
                {
                    detailFailures.Add($"{student.StudentNumber}: {ex.Message}");
                    _logger.LogWarning(
                        ex,
                        "Skipping student {StudentNumber} after its legacy API detail request failed.",
                        student.StudentNumber);
                }
            }

            if (detailedStudents.Count == 0)
            {
                var sample = string.Join("; ", detailFailures.Take(10));
                throw new InvalidOperationException(
                    $"No student detail records could be loaded, so the existing SQLite cache was preserved. " +
                    $"Skipped {detailFailures.Count} record(s). Examples: {sample}");
            }

            var uniqueStudentNumbers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            students = new List<Student>(detailedStudents.Count);
            foreach (var detailedStudent in detailedStudents)
            {
                var studentNumber = detailedStudent.StudentNumber?.Trim();
                if (string.IsNullOrWhiteSpace(studentNumber))
                {
                    detailFailures.Add("Detail record with no student number was skipped");
                    _logger.LogWarning(
                        "Skipping detail record {StudentId} because its student number is missing.",
                        detailedStudent.StudentId);
                    continue;
                }

                if (!uniqueStudentNumbers.Add(studentNumber))
                {
                    detailFailures.Add($"{studentNumber}: duplicate student number was skipped");
                    _logger.LogWarning(
                        "Skipping duplicate student {StudentNumber} during SQLite refresh.",
                        studentNumber);
                    continue;
                }

                detailedStudent.StudentNumber = studentNumber;
                students.Add(detailedStudent);
            }

            if (students.Count == 0)
            {
                var sample = string.Join("; ", detailFailures.Take(10));
                throw new InvalidOperationException(
                    $"No valid student records remained to save, so the existing SQLite cache was preserved. " +
                    $"Skipped {detailFailures.Count} record(s). Examples: {sample}");
            }

            var detailRecordsLoaded = students.Count;
            await _cacheStore.SyncStudentsAsync(students, allowEnrollmentHistoryReduction: true);
            var status = await _cacheStore.GetStatusAsync(cancellationToken);

            _logger.LogInformation(
                "Direct API to SQLite refresh completed. Students={Students}, EnrollmentHistories={Enrollments}, Details={Details}, Skipped={Skipped}.",
                status.StudentCount,
                status.EnrollmentHistoryCount,
                detailRecordsLoaded,
                detailFailures.Count);

            return new StudentCacheRefreshResult
            {
                StudentCount = status.StudentCount,
                EnrollmentHistoryCount = status.EnrollmentHistoryCount,
                DetailRecordsLoaded = detailRecordsLoaded,
                SkippedRecordCount = detailFailures.Count,
                SkippedRecordExamples = detailFailures.Take(10).ToList(),
                CompletedUtc = DateTime.UtcNow
            };
        }

        private static async Task<T?> GetAsync<T>(HttpClient client, string relativePath, CancellationToken cancellationToken)
        {
            using var response = await client.GetAsync(relativePath, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Legacy API request failed with {(int)response.StatusCode} {response.ReasonPhrase}.");
            }

            return string.IsNullOrWhiteSpace(content)
                ? default
                : JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
    }
}
