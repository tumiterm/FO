// <copyright file="StudentService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    18/03/2025 20:27 PM
// Purpose:         Defines the StudentService

#region usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides services for managing and retrieving student-related data, including caching, filtering, and
    /// interacting with external APIs.
    /// </summary>
    /// <remarks>This service is responsible for retrieving student information, managing caching for
    /// performance optimization, and providing various methods to query and manipulate student data. It interacts with
    /// external APIs to fetch data when necessary and supports fallback mechanisms for resilience.</remarks>
    public class StudentService : IStudentService
    {
        #region Private
        private readonly ILogger<StudentService> _logger;
        private readonly IHelperService _helperService;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseAddress;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentCacheStore _cacheStore;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,   
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) 
            }
        };

        private static readonly TimeSpan DefaultSliding = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan ListAbsolute = TimeSpan.FromMinutes(30);
        #endregion

        /// Initializes a new instance of the <see cref="StudentService"/> class.
        /// </summary>
        /// <remarks>This constructor initializes the <see cref="StudentService"/> with the necessary
        /// dependencies. The base API address is retrieved from the application configuration using the <paramref
        /// name="helperService"/>.</remarks>
        /// <param name="logger">The logger instance used to log diagnostic and operational messages.</param>
        /// <param name="helperService">A service that provides utility methods, such as retrieving configuration values.</param>
        /// <param name="cache">The memory cache instance used for caching data to improve performance.</param>
        /// <param name="httpClientFactory">A factory for creating <see cref="HttpClient"/> instances to make HTTP requests.</param>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="cacheStore">The SQLite-backed student cache store for offline resilience.</param>
        /// <param name="scopeFactory">Factory for creating new DI scopes for background work.</param>
        public StudentService(ILogger<StudentService> logger, IHelperService helperService, IMemoryCache cache, IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork, IStudentCacheStore cacheStore, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _helperService = helperService;
            _cache = cache;
            _httpClientFactory = httpClientFactory;
            _apiBaseAddress = _helperService.GetConfigurationValue("AppSettings:ApiBaseAddress", "defaultApiBaseAddress").TrimEnd('/') + "/";
            _unitOfWork = unitOfWork;
            _cacheStore = cacheStore;
            _scopeFactory = scopeFactory;
        }
        #region Public API (Interface Implementation)

        /// <summary>
        /// Retrieves a student by their student number asynchronously.
        /// </summary>
        /// <remarks>This method first attempts to retrieve the student from an in-memory cache. If the
        /// student is not found in the cache,  it makes an HTTP request to fetch the student data. The result is cached
        /// for future requests. If the API is unavailable, falls back to the SQLite cache.</remarks>
        /// <param name="studentNumber">The unique identifier of the student. Cannot be null, empty, or whitespace.</param>
        /// <returns>A <see cref="Student"/> object representing the student with the specified student number,  or <see
        /// langword="null"/> if no student is found.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="studentNumber"/> is null, empty, or consists only of whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the operation fails due to an HTTP error or invalid data returned from the server.</exception>
        public async Task<Student> GetStudentAsync(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
                throw new ArgumentException("Student number cannot be null or empty.", nameof(studentNumber));

            string cacheKey = BuildCacheKey("student", studentNumber);

            if (_cache.TryGetValue(cacheKey, out Student cached))
            {
                _logger.LogDebug("Cache hit: {CacheKey}", cacheKey);
                return cached;
            }

            try
            {
                var student = await SendGetAsync<Student>($"Student", new Dictionary<string, string>
                {
                    ["StudentNumber"] = studentNumber
                });

                if (student == null)
                {
                    _logger.LogWarning("Student not found. StudentNumber={StudentNumber}", studentNumber);
                    return null; 
                }

                SetCache(cacheKey, student);
                return student;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "API unavailable for student {StudentNumber}, falling back to SQLite cache.", studentNumber);
                var fallback = await _cacheStore.GetCachedStudentAsync(studentNumber);
                if (fallback != null)
                {
                    _logger.LogInformation("SQLite cache hit for student {StudentNumber}.", studentNumber);
                    SetCache(cacheKey, fallback);
                    return fallback;
                }
                _logger.LogError(ex, "HTTP failure retrieving student {StudentNumber} and no SQLite cache available.", studentNumber);
                throw new InvalidOperationException("Failed to retrieve student.", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON retrieving student {StudentNumber}", studentNumber);
                throw new InvalidOperationException("Invalid data returned for student.", ex);
            }
        }


        /// <summary>
        /// Retrieves a student by their email address.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The lookup is case-insensitive and trims leading/trailing whitespace from the input.
        /// A lightweight format check is applied before hitting the student list. If multiple
        /// students share the same email, active students are prioritised, then the most
        /// recently admitted is returned.
        /// </para>
        /// <para>
        /// The result is cached under a sliding expiration to avoid repeated full-list scans.
        /// When the upstream API is unavailable, the method transparently falls back to the
        /// SQLite cache via <see cref="GetStudentListAsync"/>.
        /// </para>
        /// </remarks>
        /// <param name="email">
        /// The email address to search for. Cannot be null, empty, or whitespace.
        /// </param>
        /// <returns>
        /// The matching <see cref="Student"/> if found; otherwise <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="email"/> is null, empty, whitespace, or not a
        /// syntactically valid email address.
        /// </exception>
        public async Task<Student?> GetStudentByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email address cannot be null or empty.", nameof(email));

            var trimmed = email.Trim();

            if (!ValidationHelper.IsValidEmailAddress(trimmed))
                throw new ArgumentException($"The value '{trimmed}' is not a valid email address.", nameof(email));

            var normalised = trimmed.ToUpperInvariant();
            string cacheKey = BuildCacheKey("student:email", normalised);

            if (_cache.TryGetValue(cacheKey, out Student? cached))
            {
                _logger.LogDebug("Cache hit: {CacheKey}", cacheKey);
                return cached;
            }

            var students = await GetStudentListAsync();

            var match = students
                .Where(s => !string.IsNullOrWhiteSpace(s.Email)
                          && s.Email.Trim().Equals(trimmed, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(s => s.IsActive)
                .ThenByDescending(s => s.AdmissionDate)
                .FirstOrDefault();

            if (match is null)
            {
                _logger.LogInformation("No student found for email {Email}.", trimmed);

                _cache.Set(cacheKey, (Student?)null, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });

                return null;
            }

            _logger.LogInformation(
                "Student found for email {Email}. StudentNumber={StudentNumber}, Active={IsActive}.",
                trimmed, match.StudentNumber, match.IsActive);

            SetCache(cacheKey, match);
            return match;
        }

        /// <summary>
        /// Asynchronously retrieves a list of students, utilizing caching for improved performance.
        /// </summary>
        /// <remarks>This method first attempts to retrieve the student list from an in-memory cache. If
        /// the cache is empty,  it fetches the data from the primary API and syncs to SQLite. In the event of an API 
        /// failure, the SQLite cache is used as a fallback. The retrieved list is then cached for subsequent calls.</remarks>
        /// <returns>A <see cref="List{Student}"/> containing the students. If no students are available, an empty list is
        /// returned.</returns>
        public async Task<List<Student>> GetStudentListAsync()
        {
            const string listKey = "students:all";

            if (_cache.TryGetValue(listKey, out List<Student> cached))
            {
                _logger.LogDebug("Cache hit: {CacheKey}", listKey);
                return cached;
            }

            List<Student> students;
            try
            {
                students = await SendGetAsync<List<Student>>("Students") ?? new List<Student>();

                if (students.Count > 0)
                {
                    var studentsToSync = students;

                    _ = Task.Run(async () =>
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var scopedCacheStore = scope.ServiceProvider.GetRequiredService<IStudentCacheStore>();

                        try
                        {
                            await scopedCacheStore.SyncStudentsAsync(studentsToSync);
                        }
                        catch (Exception syncEx)
                        {
                            _logger.LogError(syncEx, "Background SQLite sync failed.");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Primary API call failed, falling back to SQLite cache.");

                students = await GetStudentsFromSqliteFallbackAsync();
            }

            _cache.Set(listKey, students, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ListAbsolute
            });

            return students;
        }

        /// <summary>
        /// Retrieves student-related data for the specified student number, optionally including additional related
        /// data.
        /// </summary>
        /// <remarks>The method retrieves the data from a remote service and caches the result for
        /// subsequent calls. If the data is already cached, the cached value is returned. The optional <paramref
        /// name="include"/> parameter is currently not supported by the underlying API but is included for future
        /// extensibility.</remarks>
        /// <param name="studentNumber">The unique identifier for the student. This value cannot be null, empty, or consist only of whitespace.</param>
        /// <param name="include">An optional array of strings specifying additional related data to include in the response. If null or
        /// empty, no additional data is included.</param>
        /// <returns>A <see cref="StudentRelatedDataViewModel"/> containing the student's data and enrollment history.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="studentNumber"/> is null, empty, or consists only of whitespace.</exception>
        public async Task<StudentRelatedDataViewModel> GetStudentRelatedDataAsync(string studentNumber, string[] include = null)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
                throw new ArgumentException("Student number cannot be null or empty.", nameof(studentNumber));

            string cacheKey = BuildCacheKey("student:related", studentNumber, include == null ? "" : string.Join(",", include));

            if (_cache.TryGetValue(cacheKey, out StudentRelatedDataViewModel cached))
                return cached;

            var query = new Dictionary<string, string>
            {
                ["StudentNumber"] = studentNumber
            };
            if (include is { Length: > 0 })
            {
                query["include"] = string.Join(",", include);
            }

            var student = await SendGetAsync<Student>("Student", query);

            var vm = new StudentRelatedDataViewModel
            {
                Student = student,
                EnrollmentHistory = student?.EnrollmentHistory
            };

            SetCache(cacheKey, vm);
            return vm;
        }

        /// <summary>
        /// Retrieves a paginated list of students based on the specified filter criteria.
        /// </summary>
        /// <remarks>The method caches the results for subsequent calls with the same filter and
        /// pagination parameters. If caching is enabled and a cached result exists, the cached data is
        /// returned.</remarks>
        /// <param name="filters">An optional dictionary of key-value pairs representing the filter criteria.  The keys correspond to the
        /// filter fields, and the values specify the filter values.  If <see langword="null"/>, no filters are applied.</param>
        /// <param name="page">The page number to retrieve. Must be a positive integer. Defaults to 1.</param>
        /// <param name="pageSize">The number of students to retrieve per page. Must be a positive integer. Defaults to 10.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of students  matching the
        /// specified filters and pagination settings. If no students match the criteria, an empty list is returned.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="page"/> or <paramref name="pageSize"/> is less than 1.</exception>
        public async Task<List<Student>> GetStudentsByFilterAsync(Dictionary<string, string> filters = null, int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                throw new ArgumentException("Page and pageSize must be positive integers.");

            string serializedFilters = filters == null
                ? "none"
                : string.Join("&", filters.OrderBy(k => k.Key).Select(kv => $"{kv.Key}:{kv.Value}"));

            string cacheKey = BuildCacheKey("students:filter", $"p{page}", $"s{pageSize}", serializedFilters);

            if (_cache.TryGetValue(cacheKey, out List<Student> cached))
                return cached;

            var query = new Dictionary<string, string>
            {
                ["page"] = page.ToString(),
                ["pageSize"] = pageSize.ToString()
            };

            if (filters != null)
            {
                foreach (var kv in filters)
                    query[kv.Key] = kv.Value;
            }

            var result = await SendGetAsync<List<Student>>("Students", query) ?? new List<Student>();

            SetCache(cacheKey, result);
            return result;
        }

        /// <summary>
        /// Retrieves statistical information about a student based on their student number.
        /// </summary>
        /// <remarks>The method uses caching to improve performance. If the requested data is available in
        /// the cache, it is returned directly. Otherwise, the data is retrieved, processed, and stored in the cache for
        /// future use.</remarks>
        /// <param name="studentNumber">The unique identifier for the student. This value cannot be null, empty, or consist only of whitespace.</param>
        /// <returns>A dictionary containing statistical data about the student. The dictionary includes the following keys:
        /// <list type="bullet"> <item> <description><c>"EnrollmentCount"</c>: The total number of enrollments for the
        /// student.</description> </item> <item> <description><c>"ActiveEnrollments"</c>: The number of currently
        /// active enrollments for the student.</description> </item> </list> If the student does not exist, an empty
        /// dictionary is returned.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="studentNumber"/> is null, empty, or consists only of whitespace.</exception>
        public async Task<Dictionary<string, object>> GetStudentStatisticsAsync(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
                throw new ArgumentException("Student number cannot be null or empty.", nameof(studentNumber));

            string cacheKey = BuildCacheKey("student:stats", studentNumber);

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, object> cached))
                return cached;

            var student = await GetStudentAsync(studentNumber);
            if (student == null)
                return new Dictionary<string, object>();

            var stats = new Dictionary<string, object>
            {
                ["EnrollmentCount"] = student.EnrollmentHistory?.Count ?? 0,
                ["ActiveEnrollments"] = student.EnrollmentHistory?.Count(e => e.IsActive) ?? 0
            };

            SetCache(cacheKey, stats);
            return stats;
        }

        /// <summary>
        /// Retrieves a summary of student information based on the provided student number.
        /// </summary>
        /// <remarks>The method attempts to retrieve the student summary from a cache. If the data is not
        /// cached, it fetches the student information from the data source, constructs the summary, and stores it in
        /// the cache for future use.</remarks>
        /// <param name="studentNumber">The unique identifier for the student. This value cannot be null, empty, or consist only of whitespace.</param>
        /// <returns>A dictionary containing key-value pairs that summarize the student's information. The dictionary includes:
        /// <list type="bullet"> <item><description><c>StudentId</c>: The unique identifier of the
        /// student.</description></item> <item><description><c>FullName</c>: The full name of the student, combining
        /// first and last names.</description></item> <item><description><c>EnrollmentStatus</c>: The enrollment status
        /// of the student, or "N/A" if unavailable.</description></item> <item><description><c>Active</c>: A boolean
        /// indicating whether the student is currently active.</description></item> </list> If the student does not
        /// exist, an empty dictionary is returned.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="studentNumber"/> is null, empty, or consists only of whitespace.</exception>
        public async Task<Dictionary<string, object>> GetStudentSummaryAsync(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
                throw new ArgumentException("Student number cannot be null or empty.", nameof(studentNumber));

            string cacheKey = BuildCacheKey("student:summary", studentNumber);

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, object> cached))
                return cached;

            var student = await GetStudentAsync(studentNumber);
            if (student == null)
                return new Dictionary<string, object>();

            var firstEnrollment = student.EnrollmentHistory?.FirstOrDefault();

            var summary = new Dictionary<string, object>
            {
                ["StudentId"] = student.StudentId,
                ["FullName"] = $"{student.FirstName} {student.LastName}".Trim(),
                ["EnrollmentStatus"] = firstEnrollment?.EnrollmentStatus ?? "N/A",
                ["Active"] = student.IsActive
            };

            SetCache(cacheKey, summary);
            return summary;
        }

        /// <summary>
        /// Retrieves a dictionary of specified properties for a student identified by their student number.
        /// </summary>
        /// <remarks>The method uses caching to optimize performance. If the requested data is available
        /// in the cache, it is returned directly. Otherwise, the student data is retrieved, and the specified
        /// properties are extracted and cached for future use.</remarks>
        /// <param name="studentNumber">The unique identifier of the student. Cannot be null, empty, or whitespace.</param>
        /// <param name="studentProperties">A colon-separated list of property names to retrieve. If null or empty, a default set of properties is
        /// returned.</param>
        /// <returns>A dictionary containing the requested properties and their values for the specified student. If the student
        /// does not exist, an empty dictionary is returned.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="studentNumber"/> is null, empty, or consists only of whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown if one or more properties specified in <paramref name="studentProperties"/> do not exist on the
        /// student object.</exception>
        public async Task<Dictionary<string, object>> GetStudentWithPropertiesAsync(string studentNumber, string studentProperties = null)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
                throw new ArgumentException("Student number cannot be null or empty.", nameof(studentNumber));

            string propertySet = string.IsNullOrWhiteSpace(studentProperties) ? "default" : studentProperties;
            string cacheKey = BuildCacheKey("student:props", studentNumber, propertySet);

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, object> cached))
                return cached;

            var student = await GetStudentAsync(studentNumber);
            if (student == null)
                return new Dictionary<string, object>();

            string[] props;
            if (string.IsNullOrWhiteSpace(studentProperties))
            {
                props = new[] { "StudentId", "FirstName", "LastName", "Email", "IsActive" };
            }
            else
            {
                props = studentProperties.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }

            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var type = typeof(Student);
            var available = type.GetProperties().ToDictionary(p => p.Name, p => p);

            foreach (var p in props)
            {
                if (!available.TryGetValue(p, out var propInfo))
                {
                    string list = string.Join(", ", available.Keys.OrderBy(k => k));
                    throw new InvalidOperationException($"Property '{p}' does not exist. Available: {list}");
                }
                result[p] = propInfo.GetValue(student);
            }

            SetCache(cacheKey, result);
            return result;
        }

        /// <summary>
        /// Retrieves a list of students enrolled in the specified course, with an option to filter by active
        /// enrollments.
        /// </summary>
        /// <remarks>The method uses caching to optimize performance. If the requested data is available
        /// in the cache, it is returned directly. Otherwise, the method retrieves the data, filters it based on the
        /// specified criteria, and stores the result in the cache.</remarks>
        /// <param name="courseId">The unique identifier of the course. Must not be <see cref="Guid.Empty"/>.</param>
        /// <param name="onlyActive">A boolean value indicating whether to include only active enrollments.  <see langword="true"/> to include
        /// only active enrollments; otherwise, <see langword="false"/> to include all enrollments.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Student"/>
        /// objects  enrolled in the specified course. The list will be empty if no students match the criteria.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="courseId"/> is <see cref="Guid.Empty"/>.</exception>
        public async Task<List<Student>> GetStudentsByCourseAsync(Guid courseId, bool onlyActive = true)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("CourseId cannot be empty.", nameof(courseId));

            string cacheKey = BuildCacheKey("students:course", courseId.ToString(), onlyActive ? "active" : "all");

            if (_cache.TryGetValue(cacheKey, out List<Student> cached))
                return cached;

            var students = await GetStudentListAsync();

            var filtered = students
                .Where(s => s.EnrollmentHistory?.Any(e =>
                    e.CourseId == courseId &&
                    (!onlyActive || (e.IsActive && string.Equals(e.EnrollmentStatus, "Active", StringComparison.OrdinalIgnoreCase)))
                ) == true)
                .ToList();

            SetCache(cacheKey, filtered);
            return filtered;
        }

        /// <summary>
        /// Retrieves statistical data for a specific course, including enrollment counts and statuses.
        /// </summary>
        /// <remarks>The method uses caching to improve performance. If the statistics for the specified
        /// course are already cached,  the cached data is returned. Otherwise, the statistics are calculated by
        /// analyzing the enrollment history of students.</remarks>
        /// <param name="courseId">The unique identifier of the course for which statistics are retrieved. Must not be <see
        /// cref="Guid.Empty"/>.</param>
        /// <returns>A dictionary containing statistical data for the course. The keys represent different metrics: <list
        /// type="bullet"> <item> <term>"TotalEnrolled"</term> <description>The total number of students who have ever
        /// enrolled in the course.</description> </item> <item> <term>"Active"</term> <description>The number of
        /// students currently active in the course.</description> </item> <item> <term>"Completed"</term>
        /// <description>The number of students who have completed the course.</description> </item> <item>
        /// <term>"DroppedOut"</term> <description>The number of students who have dropped out of the
        /// course.</description> </item> </list></returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="courseId"/> is <see cref="Guid.Empty"/>.</exception>
        public async Task<Dictionary<string, int>> GetCourseStatisticsAsync(Guid courseId)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("CourseId cannot be empty.", nameof(courseId));

            string cacheKey = BuildCacheKey("course:stats", courseId.ToString());

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, int> cached))
                return cached;

            var students = await GetStudentListAsync();

            int totalEnrolled = 0;
            int droppedOut = 0;
            int completed = 0;
            int active = 0;

            foreach (var s in students)
            {
                if (s.EnrollmentHistory == null) continue;

                foreach (var e in s.EnrollmentHistory.Where(e => e.CourseId == courseId))
                {
                    totalEnrolled++;
                    if (e.IsActive) active++;
                    if (string.Equals(e.EnrollmentStatus, "Dropped Out", StringComparison.OrdinalIgnoreCase)) droppedOut++;
                    if (string.Equals(e.EnrollmentStatus, "Completed", StringComparison.OrdinalIgnoreCase)) completed++;
                }
            }

            var stats = new Dictionary<string, int>
            {
                ["TotalEnrolled"] = totalEnrolled,
                ["Active"] = active,
                ["Completed"] = completed,
                ["DroppedOut"] = droppedOut
            };

            SetCache(cacheKey, stats);
            return stats;
        }

        /// <summary>
        /// Asynchronously retrieves a dummy student object with pre-populated data.
        /// </summary>
        /// <remarks>This method is intended for testing or demonstration purposes and always returns the
        /// same dummy student data. The returned student object includes a single enrollment history record.</remarks>
        /// <param name="studentNumber">The student number associated with the dummy student. This parameter is not used in the method logic but is
        /// included for consistency with potential real-world scenarios.</param>
        /// <returns>A <see cref="Student"/> object containing pre-populated dummy data, including personal details, contact
        /// information, and enrollment history.</returns>
        public async Task<Student> GetDummyStudentAsync(string studentNumber)
        {
            await Task.CompletedTask;
            return new Student
            {
                StudentId = Guid.NewGuid(),
                StudentNumber = "FIT-DUMMY",
                AdmissionDate = DateTime.UtcNow,
                FirstName = "Thabo",
                MiddleName = "Mr Itu",
                LastName = "Bestman",
                IDNumber = "ID1234567899",
                StudyPermitNumber = "SP123456",
                PassportNumber = "P123456",
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = eGender.Male,
                PlaceOfBirth = "City A",
                Nationality = "South Africa",
                Language = "English",
                AdmissionCategory = eAdmissionCategory.FullTime,
                StreetAddressLine1 = "123 Main St",
                StreetAddressLine2 = "Apt 4B",
                Cellphone = "1234567890",
                Email = "ifoliphant@forekinstitute.co.za",
                HighestGrade = "Grade 12",
                NameOfSchool = "Dummy School",
                IsActive = true,
                Deregistered = false,
                EnrollmentHistory = new List<EnrollmentHistory>
                {
                    new()
                    {
                        EnrollmentId = Guid.NewGuid(),
                        StudentId = Guid.NewGuid(),
                        CourseId = Guid.NewGuid(),
                        CourseTitle = "IT",
                        CourseType = "Software Dev",
                        EnrollmentStatus = "Completed",
                        StartDate = new DateTime(2020, 9, 1),
                        IsActive = true
                    }
                }
            };
        }

        /// <summary>
        /// Asynchronously pre-populates the student list cache.
        /// </summary>
        /// <remarks>This method retrieves the student list and ensures it is cached for future use.  If
        /// an error occurs during the operation, the exception is logged, but the method does not rethrow it.</remarks>
        /// <returns></returns>
        public async Task PrepopulateCacheAsync()
        {
            try
            {
                _logger.LogInformation("Pre-populating student list cache.");
                _ = await GetStudentListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pre-population failed.");
            }
        }

        #endregion

        #region Additional (New) Public Methods (Consider adding to IStudentService)

        /// <summary>
        /// Retrieves a list of all active students.
        /// </summary>
        /// <remarks>An active student is determined based on the <see cref="Student.IsActive"/> property.
        /// The returned list is read-only and will be empty if no active students are found.</remarks>
        /// <returns>A read-only list of <see cref="Student"/> objects representing the active students.</returns>
        public async Task<IReadOnlyList<Student>> GetActiveStudentsAsync()
        {
            var list = await GetStudentListAsync();
            return list.Where(s => s.IsActive).ToList();
        }

        /// <summary>
        /// Retrieves a list of students filtered by their enrollment status.
        /// </summary>
        /// <remarks>The method filters students based on their enrollment history. If no students match
        /// the specified status, the returned list will be empty.</remarks>
        /// <param name="status">The enrollment status to filter students by. This value is case-insensitive and cannot be null, empty, or
        /// whitespace.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of students
        /// whose enrollment history includes the specified status.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="status"/> is null, empty, or consists only of whitespace.</exception>
        public async Task<IReadOnlyList<Student>> GetStudentsByEnrollmentStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status required.", nameof(status));

            var list = await GetStudentListAsync();
            var result = list
                .Where(s => s.EnrollmentHistory?.Any(e =>
                    string.Equals(e.EnrollmentStatus, status, StringComparison.OrdinalIgnoreCase)) == true)
                .ToList();
            return result;
        }

        /// <summary>
        /// Retrieves a paginated list of students along with the total count of students.
        /// </summary>
        /// <remarks>This method retrieves all students internally and calculates the paginated result
        /// based on the specified <paramref name="page"/> and <paramref name="pageSize"/>. The total count represents
        /// the total number of students available, regardless of pagination.</remarks>
        /// <param name="page">The page number to retrieve. Must be greater than or equal to 1.</param>
        /// <param name="pageSize">The number of students to include per page. Must be greater than or equal to 1.</param>
        /// <returns>A tuple containing a read-only list of students for the specified page and the total count of students. The
        /// first item in the tuple is the paginated list of students, and the second item is the total count.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="page"/> or <paramref name="pageSize"/> is less than 1.</exception>
        public async Task<(IReadOnlyList<Student> Students, int TotalCount)> GetStudentsPagedAsync(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
                throw new ArgumentException("Page and pageSize must be positive.");

            var list = await GetStudentListAsync();
            int total = list.Count;
            var pageData = list
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return (pageData, total);
        }

        /// <summary>
        /// Invalidates the cache for student-related data.
        /// </summary>
        /// <remarks>This method removes cached entries associated with the specified student number,
        /// including detailed properties, summary, and statistics. If no student number is provided, the global cache
        /// for all students is cleared. Use this method to ensure that stale or outdated data is removed from the
        /// cache.</remarks>
        /// <param name="studentNumber">The student number whose cache entries should be invalidated. If <see langword="null"/> or whitespace, the
        /// global cache for all students is invalidated.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task InvalidateStudentCacheAsync(string studentNumber = null)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                _cache.Remove("students:all");
                _logger.LogInformation("Invalidated global students list cache.");
            }
            else
            {
                string prefix = BuildCacheKey("student", studentNumber);
                _cache.Remove(prefix);
                _cache.Remove(BuildCacheKey("student:props", studentNumber, "default"));
                _cache.Remove(BuildCacheKey("student:summary", studentNumber));
                _cache.Remove(BuildCacheKey("student:stats", studentNumber));
                _logger.LogInformation("Invalidated caches for student {StudentNumber}", studentNumber);
            }
            return Task.CompletedTask;
        }

        #endregion

        /// <summary>
        /// Determines which students from the provided API set (or raw identity numbers) already exist
        /// as ForekOnline students by comparing ID/Passport numbers against Applications in the database.
        /// Returns distinct matched students and a count. Supply either <paramref name="apiStudents"/> OR
        /// <paramref name="idOrPassportNumbers"/> (or both). Duplicates (same ID/Passport) are removed.
        /// </summary>
        /// <param name="apiStudents">Optional collection of students retrieved from the API.</param>
        /// <param name="idOrPassportNumbers">Optional collection of raw ID/Passport numbers from the API.</param>
        /// <returns>A tuple containing the distinct matched students and the count.</returns>
        /// <exception cref="ArgumentException">Thrown if both sources are null or empty.</exception>
        public async Task<(IReadOnlyList<Student> Students, int Count)> GetForekOnlineStudentsAsync(IEnumerable<Student> apiStudents = null, IEnumerable<string> idOrPassportNumbers = null)
        {
            var inputIdentities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (apiStudents != null)
            {
                foreach (var s in apiStudents)
                {
                    if (!string.IsNullOrWhiteSpace(s.IDNumber))
                        inputIdentities.Add(s.IDNumber.Trim());
                    if (!string.IsNullOrWhiteSpace(s.PassportNumber))
                        inputIdentities.Add(s.PassportNumber.Trim());
                }
            }

            if (idOrPassportNumbers != null)
            {
                foreach (var id in idOrPassportNumbers)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                        inputIdentities.Add(id.Trim());
                }
            }

            if (inputIdentities.Count == 0)
                throw new ArgumentException("No identity numbers provided (apiStudents and idOrPassportNumbers are both empty).");

            var applications = await _unitOfWork.Applications.GetAllAsync();

            string[] candidateProps = { "IDNumber", "PassportNumber", "IDPass", "StudentNumber" };

            var existingIdentities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var app in applications)
            {
                var appType = app.GetType();
                foreach (var propName in candidateProps)
                {
                    var prop = appType.GetProperty(propName);
                    if (prop == null) continue;
                    var raw = prop.GetValue(app) as string;
                    if (string.IsNullOrWhiteSpace(raw)) continue;
                    existingIdentities.Add(raw.Trim());
                }
            }

            var matchedIdentities = inputIdentities.Where(existingIdentities.Contains).ToList();

            var resultStudents = new Dictionary<string, Student>(StringComparer.OrdinalIgnoreCase);

            if (apiStudents != null)
            {
                foreach (var s in apiStudents)
                {
                    var idPrimary = !string.IsNullOrWhiteSpace(s.IDNumber) ? s.IDNumber.Trim()
                                   : !string.IsNullOrWhiteSpace(s.PassportNumber) ? s.PassportNumber.Trim()
                                   : null;

                    if (idPrimary == null) continue;
                    if (matchedIdentities.Contains(idPrimary) && !resultStudents.ContainsKey(idPrimary))
                        resultStudents[idPrimary] = s;
                }
            }

            if (resultStudents.Count == 0 && apiStudents == null)
            {
                foreach (var id in matchedIdentities)
                {
                    if (!resultStudents.ContainsKey(id))
                    {
                        resultStudents[id] = new Student
                        {
                            StudentId = Guid.Empty,
                            StudentNumber = null,
                            IDNumber = id,
                            PassportNumber = id
                        };
                    }
                }
            }

            var finalList = resultStudents.Values.ToList();

            _logger.LogInformation("Matched {Count} ForekOnline student(s) based on identity comparison.", finalList.Count);

            return (finalList, finalList.Count);
        }

        /// <summary>
        /// Performs an advanced search for student enrollments based on course title, course type, and year, returning
        /// a paged list of matching results.
        /// </summary>
        /// <remarks>Results are ordered by enrollment start date in descending order, then by course
        /// title and student name. Only students with at least one enrollment are included in the search. Filtering is
        /// case-insensitive for course title and course type.</remarks>
        /// <param name="courseTitle">The course title to filter enrollments by. If null or empty, no filtering is applied on course title.</param>
        /// <param name="courseType">The course type to filter enrollments by. If null or empty, no filtering is applied on course type.</param>
        /// <param name="year">The enrollment year to filter results by. If null, results are not filtered by year.</param>
        /// <param name="page">The page number of results to retrieve. Must be greater than or equal to 1; values less than 1 are treated
        /// as 1.</param>
        /// <param name="pageSize">The number of results per page. Must be between 5 and 200; values outside this range are clamped to the
        /// nearest valid value.</param>
        /// <returns>A read-only list of student enrollment search results matching the specified criteria. The list contains up
        /// to the specified page size and may be empty if no results are found.</returns>
        public async Task<IReadOnlyList<AdvancedStudentSearchResultViewModel>> AdvancedStudentSearchAsync(string? courseTitle, string? courseType, int? year, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 5, 200);

            var titleTerm = (courseTitle ?? string.Empty).Trim();
            var typeTerm = (courseType ?? string.Empty).Trim();

            var students = await GetStudentListAsync();

            IEnumerable<AdvancedStudentSearchResultViewModel> query =
                students
                    .Where(s => s.EnrollmentHistory is { Count: > 0 })
                    .SelectMany(s => (s.EnrollmentHistory ?? new List<EnrollmentHistory>())
                        .Where(e => e != null)
                        .Select(e => new AdvancedStudentSearchResultViewModel(
                            StudentNumber: s.StudentNumber ?? string.Empty,
                            FullName: $"{s.FirstName} {s.LastName}".Trim(),
                            CourseTitle: e.CourseTitle ?? string.Empty,
                            CourseType: e.CourseType ?? string.Empty,
                            StartDate: e.StartDate,
                            IsActive: e.IsActive)));

            if (!string.IsNullOrWhiteSpace(titleTerm))
            {
                query = query.Where(x => x.CourseTitle.Contains(titleTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(typeTerm))
            {
                query = query.Where(x => x.CourseType.Contains(typeTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (year.HasValue)
            {
                query = query.Where(x => x.StartDate != DateTime.MinValue && x.StartDate.Year == year.Value);
            }

            query = query
                .OrderByDescending(x => x.StartDate)
                .ThenBy(x => x.CourseTitle)
                .ThenBy(x => x.FullName);

            var paged = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return paged;
        }

        /// <summary>
        /// Retrieves placement details for a student, including placement information if requested.
        /// </summary>
        /// <param name="studentNumber">The unique student number identifying the student whose placement details are to be retrieved. Cannot be
        /// null, empty, or whitespace.</param>
        /// <param name="includePlacement">Specifies whether to include placement information in the result. If <see langword="true"/>, placement
        /// details are included; otherwise, only student details are returned.</param>
        /// <returns>A <see cref="StudentPrintDetailsViewModel"/> containing the student's details and, if requested, their
        /// placement information; returns <see langword="null"/> if the student number is invalid or the student is not
        /// found.</returns>
        public async Task<StudentPrintDetailsViewModel?> GetStudentPlacementDetailsAsync(string studentNumber, bool includePlacement = true)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                return null;
            }

            var student = await GetStudentAsync(studentNumber);
            if (student is null)
            {
                return null;
            }

            if (!includePlacement)
            {
                return new StudentPrintDetailsViewModel(student, null);
            }

            var fullName = $"{student.FirstName} {student.LastName}";

            var placements = await _unitOfWork.Placement.GetAllAsync(includeProperties: new[] { nameof(Company)/*, nameof(Company.Address), nameof(Company.Contact)*/ });
            var placement = placements.FirstOrDefault(p => p.IsActive && p.Student == fullName);

            return new StudentPrintDetailsViewModel(student, placement);
        }

        #region HTTP + Helpers

        /// <summary>
        /// Sends an asynchronous HTTP GET request to the specified relative path and deserializes the response content
        /// into the specified type.
        /// </summary>
        /// <remarks>This method automatically includes a JWT token in the Authorization header of the
        /// request. If the response status code indicates failure, an <see cref="HttpRequestException"/> is
        /// thrown.</remarks>
        /// <typeparam name="T">The type to which the response content will be deserialized.</typeparam>
        /// <param name="relativePath">The relative path of the endpoint to send the GET request to.</param>
        /// <param name="query">An optional dictionary of query parameters to include in the request. Keys and values will be URL-encoded.
        /// Null or empty values are ignored.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The deserialized response content of type <typeparamref name="T"/>, or the default value of <typeparamref
        /// name="T"/> if the response content is empty or the endpoint returns a 404 status code.</returns>
        /// <exception cref="HttpRequestException">Thrown if the request fails with a non-success HTTP status code.</exception>
        private async Task<T> SendGetAsync<T>(string relativePath, IDictionary<string, string> query = null, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient(nameof(StudentService));
            client.BaseAddress ??= new Uri(_apiBaseAddress);

            string token = await _helperService.GenerateJwtToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var sb = new StringBuilder(relativePath);
            if (query != null && query.Count > 0)
            {
                sb.Append('?');
                sb.Append(string.Join("&", query
                    .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                    .Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}")));
            }

            var url = sb.ToString();
            _logger.LogDebug("GET {Url}", url);

            using var response = await client.GetAsync(url, ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation("GET {Url} returned 404.", url);
                return default;
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("GET {Url} failed. Status={Status} Body={Body}", url, response.StatusCode, body);
                throw new HttpRequestException($"Request failed: {(int)response.StatusCode} {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync(ct);

            if (string.IsNullOrWhiteSpace(content))
                return default;

            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }

        /// <summary>
        /// Constructs a cache key by concatenating non-empty, trimmed string parts with a colon (':') separator.
        /// </summary>
        /// <param name="parts">An array of string parts to include in the cache key. Null, empty, or whitespace-only strings are ignored.</param>
        /// <returns>A cache key string composed of the valid parts, separated by colons. Returns an empty string if no valid
        /// parts are provided.</returns>
        private static string BuildCacheKey(params string[] parts)
            => string.Join(':', parts.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()));

        /// <summary>
        /// Stores the specified value in the cache with the given key.
        /// </summary>
        /// <remarks>The cached value will use a sliding expiration policy, which resets the expiration
        /// timer each time the cached item is accessed. The default sliding expiration duration is used.</remarks>
        /// <typeparam name="T">The type of the value to store in the cache.</typeparam>
        /// <param name="key">The unique key used to identify the cached value. Cannot be null or empty.</param>
        /// <param name="value">The value to store in the cache. Cannot be null.</param>
        private void SetCache<T>(string key, T value)
        {
            _cache.Set(key, value, new MemoryCacheEntryOptions
            {
                SlidingExpiration = DefaultSliding
            });
        }

        /// <summary>
        /// Attempts to retrieve students from the SQLite fallback cache.
        /// If the SQLite cache is also empty, returns the hardcoded fallback list.
        /// </summary>
        /// <returns>A list of students from SQLite cache or the hardcoded fallback.</returns>
        private async Task<List<Student>> GetStudentsFromSqliteFallbackAsync()
        {
            try
            {
                var sqliteStudents = await _cacheStore.GetCachedStudentsAsync();
                if (sqliteStudents.Count > 0)
                {
                    var syncInfo = await _cacheStore.GetLastSyncInfoAsync();
                    _logger.LogInformation(
                        "Using SQLite fallback cache: {Count} students, last synced {SyncTime}.",
                        sqliteStudents.Count,
                        syncInfo?.LastSyncUtc.ToString("O") ?? "unknown");
                    return sqliteStudents;
                }

                _logger.LogWarning("SQLite cache is empty. Returning hardcoded fallback list.");
            }
            catch (Exception sqliteEx)
            {
                _logger.LogError(sqliteEx, "SQLite fallback also failed. Returning hardcoded fallback list.");
            }

            return GetFallbackStudentList();
        }

        /// <summary>
        /// Provides a fallback list of students with predefined data.
        /// </summary>
        /// <remarks>This method returns a hardcoded list of students, each with detailed information such
        /// as  personal details, enrollment history, and contact information. It is intended to be used  as a fallback
        /// in scenarios where no dynamic student data is available.</remarks>
        /// <returns>A <see cref="List{T}"/> of <see cref="Student"/> objects containing predefined student data.</returns>
        private List<Student> GetFallbackStudentList()
        {
            _unitOfWork.Applications.GetAllAsync().Wait();

            return new List<Student>
            {
                new()
                {
                    StudentId = Guid.NewGuid(),
                    StudentNumber = "FIT-DUMMY",
                    AdmissionDate = DateTime.UtcNow,
                    FirstName = "Girl",
                    MiddleName = "Mr Itu",
                    LastName = "Mom",
                    IDNumber = "ID1234567899",
                    StudyPermitNumber = "SP123456",
                    PassportNumber = "P123456",
                    DateOfBirth = new DateTime(2000,1,1),
                    Gender = eGender.Male,
                    PlaceOfBirth = "City A",
                    Nationality = "South Africa",
                    Language = "English",
                    AdmissionCategory = eAdmissionCategory.FullTime,
                    StreetAddressLine1 = "123 Main St",
                    StreetAddressLine2 = "Apt 4B",
                    Cellphone = "1234567890",
                    Email = "ifoliphant@forekinstitute.co.za",
                    HighestGrade = "Grade 12",
                    NameOfSchool = "Dummy School",
                    IsActive = true,
                    Deregistered = false,
                    Code = "DUMMYCODE",
                    Name = "Dummy Student",
                    HasDisability = false,
                    RegistrationSource = "Fallback",
                    StudyPermitExpiry = null,
                    RowVersion = new byte[] { 0x00 },
                    AlternativePhone = null,
                    IsDeleted = false,
                    DateDeleted = null,
                    CreatedBy = "System",
                    DateCreated = DateTime.UtcNow,
                    UserModified = "System",
                    DateModified = DateTime.UtcNow,
                    DeregistrationDate = null,
                    UserCreated = "System",
                    EnrollmentHistory = new List<EnrollmentHistory>
                    {
                        new()
                        {
                            EnrollmentId = Guid.NewGuid(),
                            StudentId = Guid.NewGuid(),
                            CourseId = Guid.NewGuid(),
                            CourseTitle = "IT",
                            CourseType = "Software Dev",
                            EnrollmentStatus = "Completed",
                            StartDate = new DateTime(2020,9,1),
                            IsActive = true
                        }
                    }
                },
                new()
                {
                    StudentId = Guid.NewGuid(),
                    StudentNumber = "FITDUMMY2",
                    AdmissionDate = DateTime.UtcNow,
                    FirstName = "Boy",
                    MiddleName = "G",
                    LastName = "Dad",
                    IDNumber = "ID6543215556",
                    StudyPermitNumber = "SP654321",
                    PassportNumber = "P654321",
                    DateOfBirth = new DateTime(1999,5,15),
                    Gender = eGender.Female,
                    PlaceOfBirth = "Nkandla",
                    Nationality = "South African",
                    Language = "Zulu",
                    AdmissionCategory = eAdmissionCategory.PartTime,
                    StreetAddressLine1 = "456 Elm St",
                    StreetAddressLine2 = "Suite 2A",
                    Cellphone = "098-765-4321",
                    Email = "fortuneismaname@gmail.com",
                    HighestGrade = "Grade 11",
                    NameOfSchool = "School B",
                    IsActive = true,
                    Deregistered = false,
                    Code = "DUMMYCODE",
                    Name = "Dummy Student",
                    HasDisability = false,
                    RegistrationSource = "Fallback",
                    StudyPermitExpiry = null,
                    RowVersion = new byte[] { 0x00 },
                    AlternativePhone = null,
                    IsDeleted = false,
                    DateDeleted = null,
                    CreatedBy = "System",
                    DateCreated = DateTime.UtcNow,
                    UserModified = "System",
                    DateModified = DateTime.UtcNow,
                    DeregistrationDate = null,
                    UserCreated = "System",
                    EnrollmentHistory = new List<EnrollmentHistory>
                    {
                        new()
                        {
                            EnrollmentId = Guid.NewGuid(),
                            StudentId = Guid.NewGuid(),
                            CourseId = Guid.NewGuid(),
                            CourseTitle = "Course 202",
                            CourseType = "Type B",
                            EnrollmentStatus = "In Progress",
                            StartDate = new DateTime(2021,1,15),
                            IsActive = true
                        }
                    }
                }
            };
        }

        #endregion
    }
}