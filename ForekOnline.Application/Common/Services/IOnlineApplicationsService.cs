using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Service interface for managing online applications, including student number generation, application limits, course choice validation, and application cycle management.
    /// /// </summary>
    public interface IOnlineApplicationsService
    {
        /// <summary>
        /// Asynchronously generates a unique student number for the specified applicant and course cycle.
        /// </summary>
        /// <param name="applicantId">The unique identifier of the applicant for whom the student number is to be generated.</param>
        /// <param name="cycleId">The unique identifier of the application cycle associated with the student number.</param>
        /// <param name="courseType">The type of course for which the student number is being generated.</param>
        /// <param name="fundingType">A character representing the funding type for the course. The value must correspond to a valid funding
        /// category.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// the outcome of the student number generation, including any validation errors.</returns>
        Task<ValidationResponse> GenerateStudentNumberAsync(Guid applicantId, Guid cycleId, eCourseType courseType, char fundingType, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously checks whether the specified applicant has reached the application limit for the given cycle.
        /// </summary>
        /// <param name="applicantId">The unique identifier of the applicant whose application limit is being checked.</param>
        /// <param name="cycleId">The unique identifier of the cycle against which the application limit is evaluated.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// whether the applicant has reached the application limit.</returns>
        Task<ValidationResponse> CheckApplicationLimitAsync(Guid applicantId, Guid cycleId, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously validates the selected course choices and returns the result of the validation.
        /// </summary>
        /// <param name="firstChoiceCourseId">The unique identifier of the first-choice course to validate. Can be null if no first choice is selected.</param>
        /// <param name="secondChoiceCourseId">The unique identifier of the second-choice course to validate. Can be null if no second choice is selected.</param>
        /// <param name="thirdChoiceCourseId">The unique identifier of the third-choice course to validate. Can be null if no third choice is selected.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the validation operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// whether the course choices are valid and any validation errors.</returns>
        Task<ValidationResponse> ValidateCourseChoicesAsync(Guid? firstChoiceCourseId, Guid? secondChoiceCourseId, Guid? thirdChoiceCourseId, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously creates a new application cycle and validates its data.
        /// </summary>
        /// <param name="cycle">The application cycle to create. Must not be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// whether the creation was successful and any validation errors encountered.</returns>
        Task<ValidationResponse> CreateApplicationCycleAsync(ApplicationCycle cycle, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously updates the specified application cycle and returns the result of the validation.
        /// </summary>
        /// <param name="cycle">The application cycle to update. Cannot be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// the outcome of the update operation.</returns>
        Task<ValidationResponse> UpdateApplicationCycleAsync(ApplicationCycle cycle, CancellationToken ct = default);

        /// <summary>
        /// Configures the turnaround time for the specified cycle asynchronously.
        /// </summary>
        /// <param name="cycleId">The unique identifier of the cycle for which to set the turnaround time.</param>
        /// <param name="days">The number of days to set as the turnaround time. Must be a non-negative integer.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// the outcome of the configuration.</returns>
        Task<ValidationResponse> ConfigureTurnaroundTimeAsync(Guid cycleId, int days, CancellationToken ct = default);

        /// <summary>
        /// Processes all overdue applications for the specified cycle asynchronously.
        /// </summary>
        /// <param name="cycleId">The unique identifier of the cycle to process. If null, all cycles with overdue applications are processed.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// the outcome of the processing.</returns>
        Task<ValidationResponse> ProcessOverdueApplicationsAsync(Guid? cycleId = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves application cycles, with optional filtering.
        /// </summary>
        /// <param name="includeDeleted">If true, include deleted cycles; otherwise returns only non-deleted cycles.</param>
        /// <param name="onlyActive">If true, returns only active cycles.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of application cycles.</returns>
        Task<IReadOnlyList<ApplicationCycle>> GetApplicationCyclesAsync(bool includeDeleted = false, bool onlyActive = false, CancellationToken ct = default);

        /// <summary>
        /// Enqueues an online application submission for processing in the background.
        /// </summary>
        /// <param name="onlineApplicationUserId">The unique identifier of the user submitting the online application.</param>
        /// <param name="cycleId">The unique identifier of the application cycle to which the submission belongs.</param>
        /// <param name="courseType">The type of course for which the application is being submitted.</param>
        /// <param name="fundingType">A character representing the funding type for the application. The value must correspond to a valid funding
        /// type code.</param>
        /// <param name="createdBy">The identifier of the user or system that initiated the submission. Cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a ValidationResponse indicating
        /// the outcome of the enqueue operation.</returns>
        Task<ValidationResponse> EnqueueOnlineApplicationSubmissionAsync(Guid onlineApplicationUserId, Guid cycleId, eCourseType courseType, char fundingType, string createdBy, CancellationToken ct = default);

    }
}
