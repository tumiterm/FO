// <copyright file="StudentCacheRefreshService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Downloads a complete student snapshot directly from the legacy API and replaces the SQLite cache.
    /// This deliberately bypasses SQL Server and the in-memory student cache.
    /// </summary>
    public sealed class StudentCacheRefreshService : IStudentCacheRefreshService
    {
        private const int MaximumConcurrentDetailRequests = 8;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHelperService _helperService;
        private readonly IStudentCacheStore _cacheStore;
        private readonly ILogger<StudentCacheRefreshService> _logger;
        private readonly string _apiBaseAddress;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters = { new JsonStringEnumConverter() }
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

            var detailFailures = new ConcurrentQueue<string>();
            using var gate = new SemaphoreSlim(MaximumConcurrentDetailRequests);

            var detailTasks = students.Select(async student =>
            {
                if (string.IsNullOrWhiteSpace(student.StudentNumber))
                {
                    return (Student: student, DetailLoaded: false);
                }

                await gate.WaitAsync(cancellationToken);
                try
                {
                    var path = $"Student?StudentNumber={WebUtility.UrlEncode(student.StudentNumber)}";
                    var detailed = await GetAsync<Student>(client, path, cancellationToken);
                    if (detailed == null)
                    {
                        detailFailures.Enqueue($"{student.StudentNumber}: no detail record returned");
                        return (Student: student, DetailLoaded: false);
                    }

                    return (Student: detailed, DetailLoaded: true);
                }
                catch (Exception ex) when (ex is HttpRequestException or JsonException)
                {
                    detailFailures.Enqueue($"{student.StudentNumber}: {ex.Message}");
                    return (Student: student, DetailLoaded: false);
                }
                finally
                {
                    gate.Release();
                }
            });

            var detailedStudents = await Task.WhenAll(detailTasks);

            if (!detailFailures.IsEmpty)
            {
                var sample = string.Join("; ", detailFailures.Take(10));
                throw new InvalidOperationException(
                    $"The cache was not changed because {detailFailures.Count} student detail request(s) failed. " +
                    $"Examples: {sample}");
            }

            students = detailedStudents.Select(result => result.Student).ToList();
            var detailRecordsLoaded = detailedStudents.Count(result => result.DetailLoaded);

            var duplicateStudentNumbers = students
                .Where(student => !string.IsNullOrWhiteSpace(student.StudentNumber))
                .GroupBy(student => student.StudentNumber.Trim(), StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .Take(10)
                .ToList();

            if (duplicateStudentNumbers.Count > 0)
            {
                throw new InvalidOperationException(
                    $"The API snapshot contains duplicate student numbers: {string.Join(", ", duplicateStudentNumbers)}. " +
                    "The existing SQLite cache was preserved.");
            }

            await _cacheStore.SyncStudentsAsync(students, allowEnrollmentHistoryReduction: true);
            var status = await _cacheStore.GetStatusAsync(cancellationToken);

            _logger.LogInformation(
                "Direct API to SQLite refresh completed. Students={Students}, EnrollmentHistories={Enrollments}, Details={Details}.",
                status.StudentCount,
                status.EnrollmentHistoryCount,
                detailRecordsLoaded);

            return new StudentCacheRefreshResult
            {
                StudentCount = status.StudentCount,
                EnrollmentHistoryCount = status.EnrollmentHistoryCount,
                DetailRecordsLoaded = detailRecordsLoaded,
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
