using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides operations for managing application cycles, including creation, retrieval, update, and program
    /// assignment functionality.
    /// </summary>
    /// <remarks>ApplicationCycleService supports asynchronous operations for configuring turnaround times,
    /// creating and updating cycles, retrieving cycles by various criteria, and assigning programs to cycles. All
    /// methods are designed to handle validation and error scenarios gracefully, returning informative responses or
    /// default values when appropriate. Thread safety is ensured for service methods, but callers should be aware that
    /// underlying data persistence and retrieval depend on the injected unit of work and logger
    /// implementations.</remarks>
    public class ApplicationCycleService : IApplicationCycleService
    {
        #region Fields
        private readonly IUnitOfWork _context;
        private readonly ILogger<ApplicationCycleService> _logger;
        #endregion

        /// <summary>
        /// Initializes a new instance of the ApplicationCycleService class with the specified unit of work and logger.
        /// </summary>
        /// <param name="context">The unit of work used to access and manage application cycle data. Cannot be null.</param>
        /// <param name="logger">The logger used for recording diagnostic and operational information. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if context or logger is null.</exception>
        public ApplicationCycleService(IUnitOfWork context, ILogger<ApplicationCycleService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Configures the turnaround time, in days, for the specified application cycle asynchronously.
        /// </summary>
        /// <remarks>If the specified cycle does not exist or is deleted, the operation returns a
        /// validation response indicating the cycle was not found. If the update fails to persist, a failure message is
        /// returned. Unexpected errors are logged and reported in the response message.</remarks>
        /// <param name="cycleId">The unique identifier of the application cycle to update. Cannot be empty.</param>
        /// <param name="days">The number of days to set as the turnaround time. Must be a non-negative value.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A ValidationResponse indicating the result of the operation. Contains a message describing success or
        /// failure.</returns>
        public async Task<ValidationResponse> ConfigureTurnaroundTimeAsync(Guid cycleId, int days, CancellationToken ct = default)
        {
            if (cycleId == Guid.Empty)
            {
                return new ValidationResponse("CycleId is required.");
            }

            if (days < 0)
            {
                return new ValidationResponse("Turnaround days must be a non-negative value.");
            }

            try
            {
                var cycle = await _context.ApplicationCycle
                    .GetAsync(c => c.Id == cycleId && !c.IsDeleted, cancellationToken: ct)
                    .ConfigureAwait(false);

                if (cycle is null)
                {
                    return new ValidationResponse("Application cycle not found.");
                }

                cycle.TurnaroundDays = days;
                cycle.DateModified = DateTimeOffset.UtcNow;

                await _context.ApplicationCycle.UpdateApplicationCycleAsync(cycle).ConfigureAwait(false);

                var saved = await _context.SaveAsync().ConfigureAwait(false);

                if (saved > 0)
                {
                    return new ValidationResponse
                    {
                        Message = "Turnaround time updated."
                    };
                }

                return new ValidationResponse("Failed to persist turnaround time changes.");
            }
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    _logger.LogWarning(ex, "Concurrency conflict configuring turnaround time. CycleId {CycleId}", cycleId);
            //    return new ValidationResponse("This cycle was updated by another user. Refresh and try again.");
            //}
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error configuring turnaround time for cycle {CycleId}", cycleId);
                return new ValidationResponse("Unexpected error occurred while configuring turnaround time.");
            }
        }

        /// <summary>
        /// Creates a new application cycle and validates its properties before persisting it asynchronously.
        /// </summary>
        /// <remarks>If an active cycle already exists for the same academic year and application period,
        /// or if the cycle dates overlap with an existing cycle, the operation will fail with a validation error. The
        /// method trims and defaults certain properties of the cycle as needed. The operation is performed
        /// asynchronously and can be cancelled via the provided cancellation token.</remarks>
        /// <param name="cycle">The application cycle to create. Must not be null and must have valid properties, including a positive
        /// academic year, valid opening and closing dates, and non-negative turnaround days.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A ValidationResponse indicating the result of the operation. Returns a successful response if the cycle is
        /// created; otherwise, returns a response containing validation errors or failure reasons.</returns>
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

        /// <summary>
        /// Asynchronously retrieves the application cycle with the specified identifier.
        /// </summary>
        /// <remarks>Returns null if the specified identifier is empty or if an error occurs during
        /// retrieval. The result is not tracked by the context.</remarks>
        /// <param name="id">The unique identifier of the application cycle to retrieve. Must not be empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the application cycle if found;
        /// otherwise, null.</returns>
        public async Task<ApplicationCycle?> GetApplicationCycleByIdAsync(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            try
            {
                return await _context.ApplicationCycle
                    .GetAsync(c => c.Id == id, asNoTracking: true, cancellationToken: ct)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving application cycle by id {CycleId}", id);
                return null;
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of application cycles, optionally including deleted or only active cycles.
        /// </summary>
        /// <remarks>The returned list is ordered by academic year descending, then by active status
        /// descending, application period ascending, and opening date ascending. If an error occurs during retrieval,
        /// the method logs the error and returns an empty list.</remarks>
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
        /// Asynchronously retrieves the application cycle that is active for the specified date.
        /// </summary>
        /// <remarks>If no application cycle is active on the specified date, or if an error occurs during
        /// retrieval, the method returns null. The operation does not track changes to the returned entity.</remarks>
        /// <param name="date">The date for which to find the corresponding application cycle. Must not be the default value.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the application cycle active on
        /// the specified date, or null if no cycle is found or if the date is the default value.</returns>
        public async Task<ApplicationCycle?> GetCycleByDateAsync(DateTime date, CancellationToken ct = default)
        {
            if (date == default)
            {
                return null;
            }

            try
            {
                var snapshot = await _context.ApplicationCycle
                    .GetAllAsync(
                        filter: c => !c.IsDeleted && c.OpensAt <= date && c.Closes >= date,
                        orderBy: q => q.OrderByDescending(x => x.IsActive)
                                       .ThenByDescending(x => x.AcademicYear)
                                       .ThenByDescending(x => x.OpensAt),
                        asNoTracking: true,
                        cancellationToken: ct)
                    .ConfigureAwait(false);

                return snapshot.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cycle by date {Date}", date);
                return null;
            }
        }

        /// <summary>
        /// Determines asynchronously whether the application window for the specified cycle is currently open.
        /// </summary>
        /// <remarks>The application window is considered open if the cycle exists, is active, and the
        /// current UTC time falls between the cycle's opening and closing times. If the cycle does not exist, is
        /// deleted, inactive, or an error occurs, the method returns <see langword="false"/>.</remarks>
        /// <param name="cycleId">The unique identifier of the application cycle to evaluate. Must not be empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the
        /// application window is open for the specified cycle; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> IsApplicationWindowOpenAsync(Guid cycleId, CancellationToken ct = default)
        {
            if (cycleId == Guid.Empty)
            {
                return false;
            }

            try
            {
                var cycle = await _context.ApplicationCycle
                    .GetAsync(c => c.Id == cycleId && !c.IsDeleted, asNoTracking: true, cancellationToken: ct)
                    .ConfigureAwait(false);

                if (cycle is null)
                {
                    return false;
                }

                if (!cycle.IsActive)
                {
                    return false;
                }

                var now = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;

                return cycle.OpensAt <= now && cycle.Closes >= now;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating application window open state for cycle {CycleId}", cycleId);
                return false;
            }
        }

        /// <summary>
        /// Updates the specified application cycle asynchronously and returns the result of the validation.
        /// </summary>
        /// <param name="cycle">The application cycle to update. Cannot be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A ValidationResponse indicating whether the update was successful and containing any validation errors.</returns>
        public async Task<ValidationResponse> UpdateApplicationCycleAsync(ApplicationCycle cycle, CancellationToken ct = default)
        {
            return await UpdateApplicationCycleCoreAsync(cycle, ct);
        }

        #region Programs
        public Task<IEnumerable<Course>> GetAssignedProgramsAsync(Guid cycleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Course>> GetAssignedProgramsAsync(Guid cycleId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
        public Task AssignProgramsAsync(Guid cycleId, IEnumerable<Guid> programIds)
        {
            throw new NotImplementedException();
        }

        public Task AssignProgramsAsync(Guid cycleId, IEnumerable<Guid> programIds, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveProgramAsync(Guid cycleId, Guid programId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveProgramAsync(Guid cycleId, Guid programId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private

        /// <summary>
        /// Validates and updates an existing application cycle with the specified details asynchronously.
        /// </summary>
        /// <remarks>The method checks for overlapping cycles and ensures only one active cycle exists per
        /// academic year and application period. If validation fails or an error occurs, the response contains an
        /// appropriate error message. The update is performed only if all validations pass.</remarks>
        /// <param name="cycle">The application cycle to update. Must not be null and must contain valid values for Id, AcademicYear,
        /// OpensAt, Closes, ApplicationPeriod, Name, Description, TurnaroundDays, and other relevant fields.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the update operation.</param>
        /// <returns>A ValidationResponse indicating the result of the update operation. Returns a success message if the cycle
        /// is updated, or an error message if validation fails or no changes are saved.</returns>
        private async Task<ValidationResponse> UpdateApplicationCycleCoreAsync(ApplicationCycle cycle, CancellationToken ct)
        {
            if (cycle is null)
            {
                return new ValidationResponse("Cycle cannot be null.");
            }

            if (cycle.Id == Guid.Empty)
            {
                return new ValidationResponse("Cycle Id is required.");
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
                    .GetAsync(c => c.Id == cycle.Id && !c.IsDeleted, cancellationToken: ct)
                    .ConfigureAwait(false);

                if (existing is null)
                {
                    return new ValidationResponse("Application cycle not found.");
                }
                var others = await _context.ApplicationCycle
                                    .GetAllAsync(
                                        filter: c => !c.IsDeleted && c.Id != cycle.Id,
                                        cancellationToken: ct)
                                    .ConfigureAwait(false);

                var overlaps = others.Any(c =>
                    c.AcademicYear == cycle.AcademicYear &&
                    string.Equals((c.ApplicationPeriod ?? string.Empty).Trim(), cycle.ApplicationPeriod, StringComparison.OrdinalIgnoreCase) &&
                    (cycle.OpensAt < c.Closes && cycle.Closes > c.OpensAt));

                if (overlaps)
                {
                    return new ValidationResponse("The cycle dates overlap with an existing cycle for the same year and period.");
                }

                if (cycle.IsActive)
                {
                    var activeExists = others.Any(c =>
                        c.AcademicYear == cycle.AcademicYear &&
                        string.Equals((c.ApplicationPeriod ?? string.Empty).Trim(), cycle.ApplicationPeriod, StringComparison.OrdinalIgnoreCase) &&
                        c.IsActive);

                    if (activeExists)
                    {
                        return new ValidationResponse("Only one active cycle is allowed for the same year and application period.");
                    }
                }
                existing.Description = cycle.Description;
                existing.AcademicYear = cycle.AcademicYear;
                existing.OpensAt = cycle.OpensAt;
                existing.Closes = cycle.Closes;
                existing.IsActive = cycle.IsActive;
                existing.ApplicationPeriod = cycle.ApplicationPeriod;
                existing.TurnaroundDays = cycle.TurnaroundDays;
                existing.Code = string.IsNullOrWhiteSpace(cycle.Code)
                    ? existing.Code
                    : cycle.Code.Trim();
                existing.Name = cycle.Name;
                existing.DateModified = DateTimeOffset.UtcNow;
                existing.UserModified = cycle.UserModified;

                await _context.ApplicationCycle.UpdateApplicationCycleAsync(existing).ConfigureAwait(false);

                var saved = await _context.SaveAsync().ConfigureAwait(false);
                if (saved > 0)
                {
                    return new ValidationResponse
                    {
                        Message = "Application cycle updated."
                    };
                }

                return new ValidationResponse("No changes were saved.");
            }
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    _logger.LogWarning(ex, "Concurrency conflict updating application cycle {CycleId}", cycle.Id);
            //    return new ValidationResponse("This cycle was updated by another user. Refresh and try again.");
            //}
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating application cycle {CycleId}", cycle.Id);
                return new ValidationResponse("Unexpected error occurred while updating the application cycle.");
            }
           
        }

        #endregion
    }
}
