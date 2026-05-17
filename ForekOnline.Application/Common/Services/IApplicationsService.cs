// <copyright file="IApplicationsService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2025 13:37 PM
// Purpose:         Defines the IApplicationsService class

#region Usings
using ForekOnline.Application.Common.Validations;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Defines a contract for managing applications, submissions, dashboard data, status transitions,  rejections, and
    /// academic calendar events.
    /// </summary>
    /// <remarks>This interface provides methods for retrieving and managing application data, processing
    /// submissions,  handling status transitions, managing rejections, and interacting with academic calendar events. 
    /// It supports asynchronous operations and allows for cancellation via <see cref="CancellationToken"/>  where
    /// applicable. Implementations of this interface should ensure thread safety and proper validation  of input
    /// parameters.</remarks>
    public interface IApplicationsService
    {
        #region Dashboard

        /// <summary>
        /// Asynchronously retrieves the data required for the applications dashboard.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation, with a result of type <see cref="ApplicationsDashboardViewModel"/> 
        /// containing the dashboard data.
        /// </returns>
        Task<ApplicationsDashboardViewModel> GetDashboardData();

        /// <summary>
        /// Asynchronously retrieves a summary of the dashboard, including key metrics and data points.
        /// </summary>
        /// <remarks>The returned <see cref="DashboardSummaryViewModel"/> includes aggregated data and
        /// metrics  relevant to the dashboard's current state. Callers can use the <paramref name="ct"/> parameter  to
        /// cancel the operation if needed.</remarks>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a  <see
        /// cref="DashboardSummaryViewModel"/> object with the dashboard summary data.</returns>
        Task<DashboardSummaryViewModel> GetDashboardSummaryAsync(CancellationToken ct = default);

        #endregion

        #region Applications

        /// <summary>
        /// Asynchronously retrieves a read-only list of application view models.
        /// </summary>
        /// <remarks>The returned list is read-only and reflects the current state of the applications at
        /// the time of retrieval.</remarks>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of  <see
        /// cref="ApplicationsViewModel"/> objects representing the applications.</returns>
        Task<IReadOnlyList<ApplicationsViewModel>> GetApplicationsAsync(CancellationToken ct = default);

        /// <summary>
        /// Retrieves the application data for editing based on the specified application ID.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application to retrieve.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ApplyViewModel"/> 
        /// representing the application data, or <see langword="null"/> if the application is not found.</returns>
        Task<ApplyViewModel?> GetApplicationForEditAsync(Guid applicationId, CancellationToken ct = default);

        /// <summary>
        /// Converts the specified course identifier to its string representation asynchronously.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course to convert.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the string representation of the
        /// course identifier.</returns>
        Task<string> ConvertCourseIdToStringAsync(Guid courseId, CancellationToken ct = default);
        #endregion

        #region Submissions

        /// <summary>
        /// Builds an application entity based on the provided application model.
        /// </summary>
        /// <param name="model">The application model containing the data required to build the application.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A tuple containing the validation response and the resulting application entity.  The application entity
        /// will be <see langword="null"/> if the validation fails.</returns>
        Task<(ValidationResponse Result, Domain.Entities.Application? Entity)> BuildApplicationAsync(ApplyViewModel model, CancellationToken ct = default);

        /// <summary>
        /// Validates whether the application limits for a given applicant and academic year have been exceeded.
        /// </summary>
        /// <remarks>This method performs a validation check to ensure that the number of applications
        /// submitted by the applicant  does not exceed the specified limit for the given academic year. The validation
        /// process may involve querying  external data sources or services.</remarks>
        /// <param name="idOrPassport">The unique identifier or passport number of the applicant. This value cannot be null or empty.</param>
        /// <param name="academicYear">The academic year for which the validation is performed. Must be a positive integer.</param>
        /// <param name="max">The maximum number of applications allowed for the specified academic year. Defaults to 3 if not provided.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating whether the application limits have been exceeded,  along with
        /// any relevant validation details.</returns>
        Task<ValidationResponse> ValidateApplicationLimitsAsync(string idOrPassport, int academicYear, int max = 3, CancellationToken ct = default);

        /// <summary>
        /// Determines whether the specified application is a duplicate of an existing application.
        /// </summary>
        /// <remarks>This method performs a duplication check based on the application's properties. The
        /// exact criteria for duplication         depend on the implementation and may involve comparing specific
        /// fields or querying a data store.</remarks>
        /// <param name="application">The application to check for duplication. Must not be <see langword="null"/>.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns><see langword="true"/> if the specified application is a duplicate; otherwise, <see langword="false"/>.</returns>
        Task<bool> IsDuplicateApplicationAsync(Domain.Entities.Application application, CancellationToken ct = default);

        /// <summary>
        /// Uploads files associated with the specified application to the storage system.
        /// </summary>
        /// <param name="application">The application entity containing the files to be uploaded. Cannot be <see langword="null"/>.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the upload operation, including any validation
        /// errors or success status.</returns>
        Task<ValidationResponse> UploadFilesAsync(Domain.Entities.Application application, CancellationToken ct = default);

        /// <summary>
        /// Saves the specified application to the data store asynchronously.
        /// </summary>
        /// <remarks>This method validates the application before saving it. If validation fails, the
        /// returned <see cref="ValidationResponse"/>  will contain details about the validation errors, and the
        /// application will not be saved.</remarks>
        /// <param name="application">The application entity to be saved. Cannot be <see langword="null"/>.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the save operation, including any validation
        /// errors encountered.</returns>
        Task<ValidationResponse> SaveApplicationAsync(Domain.Entities.Application application, CancellationToken ct = default);

        /// <summary>
        /// Sends submission notifications for the specified application.
        /// </summary>
        /// <remarks>This method sends notifications related to the submission of the specified
        /// application.  The operation can be canceled by passing a cancellation token.</remarks>
        /// <param name="application">The application for which submission notifications will be sent. Cannot be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation. Optional.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the notification process, including any
        /// validation errors or success status.</returns>
        Task<ValidationResponse> SendSubmissionNotificationsAsync(Domain.Entities.Application application, CancellationToken ct = default);
        #endregion

        #region Status Transition

        /// <summary>
        /// Processes an approved application and performs the necessary actions to transition its status.
        /// </summary>
        /// <param name="application">The approved application to be processed. Cannot be <see langword="null"/>.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while processing the application.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the processing, including any validation errors
        /// or success status.</returns>
        Task<ValidationResponse> ProcessApprovedApplicationAsync(Domain.Entities.Application application, CancellationToken ct = default);

        /// <summary>
        /// Sends an aptitude test invitation to the specified application.
        /// </summary>
        /// <remarks>This method is asynchronous and should be awaited. Ensure that the provided <paramref
        /// name="dateTime"/> is in a format that can be parsed successfully. The operation may fail if the application
        /// is in an invalid state or if the provided date and time are not valid.</remarks>
        /// <param name="application">The application entity representing the candidate to whom the invitation will be sent. Cannot be null.</param>
        /// <param name="dateTime">The date and time for the aptitude test, in a valid string format. Must not be null or empty.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the operation, including any validation errors
        /// or success status.</returns>
        Task<ValidationResponse> SendAptitudeInvitationAsync(Domain.Entities.Application application, string dateTime, CancellationToken ct = default);

        /// <summary>
        /// Sends a rejection email to the applicant based on the provided application details and reason.
        /// </summary>
        /// <remarks>This method is asynchronous and will not block the calling thread. Ensure that the
        /// provided application contains valid email details.</remarks>
        /// <param name="application">The application entity containing the applicant's details. Cannot be <see langword="null"/>.</param>
        /// <param name="reason">The reason for the rejection, which will be included in the email. Cannot be <see langword="null"/> or
        /// empty.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the email-sending operation, including any
        /// validation errors or success status.</returns>
        Task<ValidationResponse> SendRejectionMailAsync(Domain.Entities.Application application, string reason, CancellationToken ct = default);
        #endregion

        #region Rejection

        /// <summary>
        /// Saves the rejection details for an application asynchronously.
        /// </summary>
        /// <remarks>This method validates the provided rejection details before saving them. If
        /// validation fails, the returned  <see cref="ValidationResponse"/> will contain the relevant error
        /// information.</remarks>
        /// <param name="rejection">The <see cref="ApplicationRejection"/> object containing the details of the rejection to be saved.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns>A <see cref="ValidationResponse"/> indicating the result of the save operation, including any validation
        /// errors if the operation fails.</returns>
        Task<ValidationResponse> SaveApplicationRejectionAsync(ApplicationRejection rejection, CancellationToken ct = default);

        /// <summary>
        /// Determines whether the rejection form has been submitted for the specified applicant.
        /// </summary>
        /// <remarks>This method is asynchronous and returns a task that completes once the submission
        /// status is determined.</remarks>
        /// <param name="applicantId">The unique identifier of the applicant.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><see langword="true"/> if the rejection form has been submitted for the specified applicant; otherwise, <see
        /// langword="false"/>.</returns>
        Task<bool> IsRejectionFormSubmittedAsync(Guid applicantId, CancellationToken ct = default);
        #endregion

        #region Academic Calendar Events
        /// <summary>
        /// Retrieves a list of calendar events within the specified date range.
        /// </summary>
        /// <remarks>The method retrieves events that overlap with the specified date range. If both
        /// <paramref name="start"/> and <paramref name="end"/> are <see langword="null"/>, all available events are
        /// retrieved.</remarks>
        /// <param name="start">The start date and time of the range to retrieve events for. Specify <see langword="null"/> to include
        /// events from the earliest available date.</param>
        /// <param name="end">The end date and time of the range to retrieve events for. Specify <see langword="null"/> to include events
        /// up to the latest available date.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. The default value
        /// is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of <see
        /// cref="CalendarEventViewModel"/> objects representing the calendar events within the specified range.  If no
        /// events are found, the list will be empty.</returns>
        Task<IReadOnlyList<CalendarEventViewModel>> GetCalendarEventsAsync(DateTime? start, DateTime? end, CancellationToken ct = default);

        /// <summary>
        /// Creates a new calendar event asynchronously and returns the unique identifier of the created event.
        /// </summary>
        /// <param name="form">The form data containing details of the calendar event to be created.</param>
        /// <param name="createdBy">The identifier of the user who is creating the event. Cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier of the created calendar event.</returns>
        Task<Guid> CreateEventAsync(CalendarEventFormViewModel form, string createdBy, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing calendar event with the provided details.
        /// </summary>
        /// <remarks>This method updates the event details in the system. Ensure that the provided form
        /// contains valid data  and that the user identifier is correctly specified. The operation respects the
        /// cancellation token  if provided.</remarks>
        /// <param name="form">The form containing the updated details of the calendar event. Cannot be null.</param>
        /// <param name="modifiedBy">The identifier of the user making the modification. Cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation. Optional.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateEventAsync(CalendarEventFormViewModel form, string modifiedBy, CancellationToken ct = default);

        /// <summary>
        /// Deletes an event with the specified identifier.
        /// </summary>
        /// <remarks>If the specified event does not exist, the operation completes without throwing an
        /// exception.</remarks>
        /// <param name="eventId">The unique identifier of the event to delete.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteEventAsync(Guid eventId, CancellationToken ct = default);
        #endregion

    }
}
