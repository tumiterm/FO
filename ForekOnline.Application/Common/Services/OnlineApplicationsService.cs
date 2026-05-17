using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides operations for managing online application cycles, including creation, validation, retrieval, and
    /// processing of applications.
    /// </summary>
    /// <remarks>The OnlineApplicationsService exposes asynchronous methods for handling application cycles
    /// and related applicant operations. It supports validation, configuration, and retrieval of cycles, as well as
    /// processing overdue applications and generating student numbers. All operations are designed to be cancellable
    /// via a cancellation token and return validation responses or lists as appropriate. Errors encountered during
    /// operations are logged and typically result in informative responses rather than exceptions.</remarks>
    public class OnlineApplicationsService : IOnlineApplicationsService
    {
        #region Fields
        private readonly ILogger<OnlineApplicationsService> _logger;
        private readonly IUnitOfWork _context;
        private readonly IApplicationCycleService _cycleService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the OnlineApplicationsService class with the specified logger and unit of work
        /// context.
        /// </summary>
        /// <param name="logger">The logger used to record diagnostic and operational messages for the service. Cannot be null.</param>
        /// <param name="context">The unit of work context that manages data access operations for online applications. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if logger or context is null.</exception>
        public OnlineApplicationsService(ILogger<OnlineApplicationsService> logger, IUnitOfWork context, IApplicationCycleService cycleService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cycleService = cycleService ?? throw new ArgumentNullException(nameof(cycleService));
        }

        public Task<ValidationResponse> CheckApplicationLimitAsync(Guid applicantId, Guid cycleId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ValidationResponse> ConfigureTurnaroundTimeAsync(Guid cycleId, int days, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new application cycle asynchronously after validating the provided cycle details.
        /// </summary>
        /// <remarks>The method checks for overlapping cycles and ensures only one active cycle exists per
        /// academic year and application period. If validation fails or an error occurs, the response will contain an
        /// appropriate error message. The operation is performed asynchronously and can be cancelled using the provided
        /// cancellation token.</remarks>
        /// <param name="cycle">The application cycle to create. Must not be null and must contain valid academic year, period, open and
        /// close dates, and turnaround days.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A ValidationResponse indicating the result of the operation. Returns a successful response if the cycle is
        /// created; otherwise, returns a response containing validation or error messages.</returns>
        public async Task<ValidationResponse> CreateApplicationCycleAsync(ApplicationCycle cycle, CancellationToken ct = default)
        {
            if (cycle is null)
            {
                return new ValidationResponse("Cycle cannot be null.");
            }

            if (cycle.AcademicYear <= 0)
            {
                return new ValidationResponse("AcademicYear is required.");
            }

            if (cycle.OpensAt == default || cycle.Closes == default)
            {
                return new ValidationResponse("OpensAt and Closes are required.");
            }

            if (cycle.OpensAt >= cycle.Closes)
            {
                return new ValidationResponse("OpensAt must be earlier than Closes.");
            }

            cycle.ApplicationPeriod = string.IsNullOrWhiteSpace(cycle.ApplicationPeriod)
                ? "Intake"
                : cycle.ApplicationPeriod.Trim();

            cycle.Name = string.IsNullOrWhiteSpace(cycle.Name)
                ? $"{cycle.AcademicYear} {cycle.ApplicationPeriod}".Trim()
                : cycle.Name.Trim();

            cycle.Description = string.IsNullOrWhiteSpace(cycle.Description)
                ? null
                : cycle.Description.Trim();

            if (cycle.TurnaroundDays < 0)
            {
                return new ValidationResponse("TurnaroundDays cannot be negative.");
            }

            try
            {
                var existing = await _context.ApplicationCycle
                    .GetAllAsync(filter: c => !c.IsDeleted, cancellationToken: ct)
                    .ConfigureAwait(false);

                // Overlap check (by date-range)
                var overlaps = existing.Any(c =>
                    c.AcademicYear == cycle.AcademicYear &&
                    string.Equals((c.ApplicationPeriod ?? string.Empty).Trim(), cycle.ApplicationPeriod, StringComparison.OrdinalIgnoreCase) &&
                    (cycle.OpensAt < c.Closes && cycle.Closes > c.OpensAt));

                if (overlaps)
                {
                    return new ValidationResponse("The cycle dates overlap with an existing cycle for the same year and period.");
                }
                if (cycle.IsActive)
                {
                    var activeExists = existing.Any(c =>
                        c.AcademicYear == cycle.AcademicYear &&
                        string.Equals((c.ApplicationPeriod ?? string.Empty).Trim(), cycle.ApplicationPeriod, StringComparison.OrdinalIgnoreCase) &&
                        c.IsActive);

                    if (activeExists)
                    {
                        return new ValidationResponse("Only one active cycle is allowed for the same year and application period.");
                    }
                }

                cycle.Id = cycle.Id == Guid.Empty ? Guid.NewGuid() : cycle.Id;
                cycle.Code = string.IsNullOrWhiteSpace(cycle.Code)
                    ? $"{cycle.AcademicYear}-{cycle.ApplicationPeriod}".Replace(" ", string.Empty).ToUpperInvariant()
                    : cycle.Code.Trim();

                cycle.DateCreated = cycle.DateCreated == default ? DateTimeOffset.UtcNow : cycle.DateCreated;
                cycle.DateModified = DateTimeOffset.UtcNow;

                var added = await _context.ApplicationCycle.AddAsync(cycle, ct).ConfigureAwait(false);

                if (added is null)
                {
                    return new ValidationResponse("Failed to create application cycle.");
                }
                var saved = await _context.SaveAsync().ConfigureAwait(false);

                if (saved > 0)
                {
                    return new ValidationResponse();
                }

                return new ValidationResponse("Failed to persist application cycle.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating application cycle for year {AcademicYear} period {ApplicationPeriod}", cycle.AcademicYear, cycle.ApplicationPeriod);
                return new ValidationResponse("Unexpected error occurred while creating the application cycle.");
            }
        }

        public async Task<ValidationResponse> GenerateStudentNumberAsync(Guid applicantId, Guid cycleId, EnumRegistry.eCourseType courseType, char fundingType, CancellationToken ct = default)
        {
            if (applicantId == Guid.Empty)
            {
                return new ValidationResponse("applicantId is required.");
            }

            if (cycleId == Guid.Empty)
            {
                return new ValidationResponse("cycleId is required.");
            }

            fundingType = char.ToUpperInvariant(fundingType);

            if (!char.IsLetterOrDigit(fundingType))
            {
                return new ValidationResponse("fundingType must be a letter or digit.");
            }

            var cycle = await _context.ApplicationCycle.GetAsync(
                filter: c => c.Id == cycleId && !c.IsDeleted,
                cancellationToken: ct).ConfigureAwait(false);

            if (cycle is null)
            {
                return new ValidationResponse("Application cycle not found.");
            }

            var user = await _context.OnlineApplicantUser.GetAsync(
                filter: u => u.Id == applicantId && !u.IsDeleted,
                cancellationToken: ct).ConfigureAwait(false);

            if (user is null)
            {
                return new ValidationResponse("Online application user not found.");
            }

            if (!string.IsNullOrWhiteSpace(user.StudentNumber))
            {
                return new ValidationResponse($"Student number already generated: {user.StudentNumber}");
            }

            var year2 = (cycle.AcademicYear % 100).ToString("00", CultureInfo.InvariantCulture);
            var courseCode = GetCourseTypeCode(courseType);

            var prefix = $"FIT{year2}{courseCode}{fundingType}";

            var existing = await _context.OnlineApplicantUser.GetAllAsync(
                filter: u => !u.IsDeleted &&
                             u.StudentNumber != null &&
                             u.StudentNumber.StartsWith(prefix, StringComparison.OrdinalIgnoreCase),
                cancellationToken: ct).ConfigureAwait(false);

            var maxSeq = 0;

            foreach (var existingUser in existing)
            {
                if (TryParseSequence(existingUser.StudentNumber, prefix, out var seq) && seq > maxSeq)
                {
                    maxSeq = seq;
                }
            }

            var nextSeq = maxSeq + 1;

            if (nextSeq > 9999)
            {
                return new ValidationResponse($"Student number range exhausted for prefix '{prefix}'.");
            }

            user.StudentNumber = prefix + nextSeq.ToString("0000", CultureInfo.InvariantCulture);

            await _context.OnlineApplicantUser.UpdateOnlineApplicantUserAsync(user).ConfigureAwait(false);

            var saved = await _context.SaveAsync().ConfigureAwait(false);

            return saved > 0
                ? new ValidationResponse()
                : new ValidationResponse("Failed to generate student number.");
        }

        public Task<ValidationResponse> ProcessOverdueApplicationsAsync(Guid? cycleId = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ValidationResponse> UpdateApplicationCycleAsync(ApplicationCycle cycle, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ValidationResponse> ValidateCourseChoicesAsync(Guid? firstChoiceCourseId, Guid? secondChoiceCourseId, Guid? thirdChoiceCourseId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously retrieves a list of application cycles, optionally including deleted or only active cycles.
        /// </summary>
        /// <remarks>If an error occurs during retrieval, the method logs the error and returns an empty
        /// list instead of throwing an exception.</remarks>
        /// <param name="includeDeleted">A value indicating whether deleted application cycles should be included in the result. Set to <see
        /// langword="true"/> to include deleted cycles; otherwise, only non-deleted cycles are returned.</param>
        /// <param name="onlyActive">A value indicating whether only active application cycles should be included. Set to <see langword="true"/>
        /// to return only active cycles; otherwise, both active and inactive cycles are returned.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A read-only list of <see cref="ApplicationCycle"/> objects matching the specified criteria. Returns an empty
        /// list if no cycles are found or if an error occurs.</returns>
        public async Task<IReadOnlyList<ApplicationCycle>> GetApplicationCyclesAsync(bool includeDeleted = false, bool onlyActive = false, CancellationToken ct = default)
        {
            try
            {
                IReadOnlyList<ApplicationCycle> cycles = await _context.ApplicationCycle
                    .GetAllAsync(
                        filter: c => (includeDeleted || !c.IsDeleted) && (!onlyActive || c.IsActive),
                        orderBy: q => q.OrderByDescending(x => x.AcademicYear)
                                       .ThenByDescending(x => x.IsActive)
                                       .ThenBy(x => x.ApplicationPeriod)
                                       .ThenBy(x => x.OpensAt),
                        cancellationToken: ct)
                    .ConfigureAwait(false);

                return cycles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving application cycles.");
                return Array.Empty<ApplicationCycle>();
            }
        }

        /// <summary>
        /// Enqueues a new online application submission for background processing.
        /// </summary>
        /// <remarks>The submission is added to a background job queue for asynchronous processing. This
        /// method does not process the application itself, but only schedules it for later handling.</remarks>
        /// <param name="onlineApplicationUserId">The unique identifier of the user submitting the online application. Must not be empty.</param>
        /// <param name="cycleId">The unique identifier of the application cycle. Must not be empty.</param>
        /// <param name="courseType">The type of course for which the application is being submitted.</param>
        /// <param name="fundingType">A character representing the funding type for the application.</param>
        /// <param name="createdBy">The username or identifier of the user creating the submission entry.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A ValidationResponse indicating whether the submission was successfully enqueued. If the operation fails or
        /// required parameters are missing, the response contains an error message.</returns>
        public async Task<ValidationResponse> EnqueueOnlineApplicationSubmissionAsync(Guid onlineApplicationUserId, Guid cycleId, eCourseType courseType, char fundingType, string createdBy, CancellationToken ct = default)
        {
            if (onlineApplicationUserId == Guid.Empty) return new ValidationResponse("onlineApplicationUserId is required.");
            if (cycleId == Guid.Empty) return new ValidationResponse("cycleId is required.");

            var payloadJson = JsonSerializer.Serialize(new
            {
                OnlineApplicationUserId = onlineApplicationUserId,
                CycleId = cycleId,
                CourseType = courseType,
                FundingType = fundingType
            });

            var item = new BackgroundJobQueueItem
            {
                Id = Guid.NewGuid(),
                Queue = "onlineapps",
                JobType = "OnlineApplicationSubmission",
                PayloadJson = payloadJson,
                Status = "Pending",
                Attempts = 0,
                DateCreated = DateTimeOffset.UtcNow,
                DateModified = DateTimeOffset.UtcNow,
                UserCreated = createdBy,
                UserModified = createdBy,
                IsDeleted = false
            };
            await _context.BackgroundJobQueue.AddAsync(item, ct).ConfigureAwait(false);

            var saved = await _context.SaveAsync().ConfigureAwait(false);
            return saved > 0 ? new ValidationResponse() : new ValidationResponse("Failed to enqueue submission.");
        }

        #region Private Helpers
        private static string GetCourseTypeCode(EnumRegistry.eCourseType courseType) =>
           courseType switch
           {
               EnumRegistry.eCourseType.OccupationalTrade => "OT",
               EnumRegistry.eCourseType.OccupationalNonTrade => "ON",
               EnumRegistry.eCourseType.ShortSkills => "SS",
               EnumRegistry.eCourseType.Nated => "NA",
               EnumRegistry.eCourseType.TradeTest => "TT",
               _ => "XX"
           };

        private static bool TryParseSequence(string? studentNumber, string prefix, out int seq)
        {
            seq = 0;

            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                return false;
            }

            if (!studentNumber.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var suffix = studentNumber.Substring(prefix.Length);

            return int.TryParse(suffix, NumberStyles.None, CultureInfo.InvariantCulture, out seq);
        }
        #endregion
    }
}
