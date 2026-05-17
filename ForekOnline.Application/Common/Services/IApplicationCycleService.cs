using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides operations for managing application cycles, including creation, configuration, retrieval, and
    /// assignment of programs within each cycle.
    /// </summary>
    /// <remarks>This interface defines asynchronous methods for handling application cycles in an admissions
    /// or enrollment system. Implementations are expected to support operations such as creating and updating cycles,
    /// configuring turnaround times, querying cycles by various criteria, and managing program assignments. All methods
    /// accept a cancellation token to support cooperative cancellation. Thread safety and transactional guarantees
    /// depend on the specific implementation.</remarks>
    public interface IApplicationCycleService
    {
        /// <summary>
        /// Creates a new application cycle asynchronously and validates the provided cycle data.
        /// </summary>
        /// <param name="cycle">The application cycle to be created. Cannot be null and must contain valid data according to business rules.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A ValidationResponse indicating the result of the creation and validation process. Contains validation
        /// errors if the operation fails.</returns>
        Task<ValidationResponse> CreateApplicationCycleAsync(ApplicationCycle cycle, CancellationToken ct = default);

        /// <summary>
        /// Updates the specified application cycle asynchronously and validates the operation.
        /// </summary>
        /// <param name="cycle">The application cycle to update. Cannot be null. The properties of this object determine the update applied.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// whether the update was successful and any validation errors encountered.</returns>
        Task<ValidationResponse> UpdateApplicationCycleAsync(ApplicationCycle cycle, CancellationToken ct = default);

        /// <summary>
        /// Configures the turnaround time for the specified cycle asynchronously.
        /// </summary>
        /// <remarks>If the specified cycle does not exist or the days parameter is invalid, the operation
        /// will fail with appropriate validation errors. This method does not block the calling thread.</remarks>
        /// <param name="cycleId">The unique identifier of the cycle for which the turnaround time is to be set.</param>
        /// <param name="days">The number of days to set as the turnaround time. Must be a positive integer.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// whether the configuration was successful and any validation errors encountered.</returns>
        Task<ValidationResponse> ConfigureTurnaroundTimeAsync(Guid cycleId, int days, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously retrieves a list of application cycles, with options to include deleted or only active
        /// cycles.
        /// </summary>
        /// <param name="includeDeleted">Specifies whether deleted application cycles should be included in the result. Set to <see langword="true"/>
        /// to include deleted cycles; otherwise, only non-deleted cycles are returned.</param>
        /// <param name="onlyActive">Specifies whether only active application cycles should be included. Set to <see langword="true"/> to return
        /// only cycles that are currently active; otherwise, all cycles are returned based on the other filters.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of <see
        /// cref="ApplicationCycle"/> objects matching the specified filters. The list will be empty if no cycles are
        /// found.</returns>
        Task<IReadOnlyList<ApplicationCycle>> GetApplicationCyclesAsync(bool includeDeleted = false, bool onlyActive = false, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously retrieves the application cycle with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the application cycle to retrieve.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the application cycle if found;
        /// otherwise, null.</returns>
        Task<ApplicationCycle?> GetApplicationCycleByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously determines whether the application window associated with the specified cycle identifier is
        /// currently open.
        /// </summary>
        /// <param name="cycleId">The unique identifier of the cycle for which to check the application window's open state.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the
        /// application window is open; otherwise, <see langword="false"/>.</returns>
        Task<bool> IsApplicationWindowOpenAsync(Guid cycleId, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously retrieves the application cycle that includes the specified date, if one exists.
        /// </summary>
        /// <param name="date">The date for which to find the corresponding application cycle.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the application cycle that
        /// includes the specified date, or null if no cycle is found.</returns>
        Task<ApplicationCycle?> GetCycleByDateAsync(DateTime date, CancellationToken ct = default);

        /// <summary>
        /// Assigns the specified programs to the given cycle asynchronously.
        /// </summary>
        /// <remarks>If the operation is canceled via the provided cancellation token, the assignment will
        /// not complete. The method does not return a result; it completes when all assignments are finished.</remarks>
        /// <param name="cycleId">The unique identifier of the cycle to which the programs will be assigned.</param>
        /// <param name="programIds">A collection of unique identifiers representing the programs to assign to the cycle. Cannot be null or
        /// contain duplicate values.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous assignment operation.</returns>
        Task AssignProgramsAsync(Guid cycleId, IEnumerable<Guid> programIds, CancellationToken ct = default);

        /// <summary>
        /// Removes the specified program from the given cycle asynchronously.
        /// </summary>
        /// <param name="cycleId">The unique identifier of the cycle from which the program will be removed.</param>
        /// <param name="programId">The unique identifier of the program to remove.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous remove operation.</returns>
        Task RemoveProgramAsync(Guid cycleId, Guid programId, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously retrieves the collection of programs assigned to the specified cycle.
        /// </summary>
        /// <param name="cycleId">The unique identifier of the cycle for which assigned programs are requested.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of courses assigned
        /// to the specified cycle. The collection will be empty if no programs are assigned.</returns>
        Task<IEnumerable<Course>> GetAssignedProgramsAsync(Guid cycleId, CancellationToken ct = default);
    }
}
