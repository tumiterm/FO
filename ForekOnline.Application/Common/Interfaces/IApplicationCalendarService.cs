using ForekOnline.Domain.ViewModels;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines methods for managing calendar events within an application, including retrieving, creating, updating,
    /// and deleting events.
    /// </summary>
    /// <remarks>Implementations of this interface provide asynchronous operations for interacting with
    /// calendar event data. All methods support cancellation via a <see cref="CancellationToken"/> parameter. The
    /// interface is intended to be used by components that require access to application-specific calendar
    /// functionality.</remarks>
    public interface IApplicationCalendarService
    {
        /// <summary>
        /// Asynchronously retrieves a list of calendar events that occur within the specified date range.
        /// </summary>
        /// <param name="start">The start date and time of the range to search for events, or null to indicate no lower bound.</param>
        /// <param name="end">The end date and time of the range to search for events, or null to indicate no upper bound.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of calendar
        /// event view models that fall within the specified date range. The list is empty if no events are found.</returns>
        Task<IReadOnlyList<CalendarEventViewModel>> GetCalendarEventsAsync(DateTime? start, DateTime? end, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously creates a new calendar event using the specified event details and creator information.
        /// </summary>
        /// <param name="form">The form data containing the details of the calendar event to create. Cannot be null.</param>
        /// <param name="createdBy">The identifier of the user creating the event. Cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier of the
        /// newly created event.</returns>
        Task<Guid> CreateEventAsync(CalendarEventFormViewModel form, string createdBy, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously updates an existing calendar event with the specified details.
        /// </summary>
        /// <param name="form">The form model containing the updated event information. Cannot be null.</param>
        /// <param name="modifiedBy">The identifier of the user making the modification. Cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the update operation.</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task UpdateEventAsync(CalendarEventFormViewModel form, string modifiedBy, CancellationToken ct = default);

        /// <summary>
        /// Asynchronously deletes the event with the specified identifier.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event to delete.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the delete operation.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteEventAsync(Guid eventId, CancellationToken ct = default);
    }
}
