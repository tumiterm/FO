// <copyright file="ApplicationCalendarService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    030/01/2026 01:26 AM
// Purpose:         Defines the ApplicationCalendarService 

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.ViewModels;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Provides methods for managing application calendar events, including creation, retrieval, updating, and deletion
    /// of events within the application's calendar system.
    /// </summary>
    /// <remarks>This service is intended for use in scenarios where application-level calendar event
    /// management is required, such as scheduling, displaying, or modifying events. All operations are asynchronous and
    /// require appropriate data models for event input and output. Thread safety is determined by the underlying
    /// dependencies and is not guaranteed by this service itself.</remarks>
    public class ApplicationCalendarService : IApplicationCalendarService
    {
        #region Private Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHelperService _helperService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the ApplicationCalendarService class with the specified unit of work and
        /// helper service.
        /// </summary>
        /// <param name="unitOfWork">The unit of work instance used to manage data persistence and transactions.</param>
        /// <param name="helperService">The helper service instance that provides auxiliary operations required by the service.</param>
        /// <exception cref="ArgumentNullException">Thrown if unitOfWork is null.</exception>
        public ApplicationCalendarService(IUnitOfWork unitOfWork, IHelperService helperService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _helperService = helperService;
        }

        /// <summary>
        /// Asynchronously creates a new calendar event using the specified form data and creator information.
        /// </summary>
        /// <param name="form">The form data containing details of the calendar event to create. Cannot be null. The form's Title property
        /// must not be null or whitespace.</param>
        /// <param name="createdBy">The identifier of the user creating the event. If null or whitespace, a default value is used.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier of the
        /// newly created calendar event.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="form"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the Title property of <paramref name="form"/> is null or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the event could not be persisted to the data store.</exception>
        public async Task<Guid> CreateEventAsync(CalendarEventFormViewModel form, string createdBy, CancellationToken ct = default)
        {
            if (form is null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            if (string.IsNullOrWhiteSpace(form.Title))
            {
                throw new ArgumentException("Title is required.", nameof(form.Title));
            }

            var evt = new ApplicationEvent
            {
                EventId = Guid.NewGuid(),
                Title = form.Title.Trim(),
                StartUtc = EnsureUtc(form.StartUtc),
                EndUtc = form.EndUtc.HasValue ? EnsureUtc(form.EndUtc.Value) : null,
                AllDay = form.AllDay,
                Category = string.IsNullOrWhiteSpace(form.Category) ? null : form.Category.Trim(),
                ColorHex = string.IsNullOrWhiteSpace(form.ColorHex) ? null : form.ColorHex.Trim(),
                Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim(),
                IsActive = true,
                CreatedOnUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime,
                CreatedBy = string.IsNullOrWhiteSpace(createdBy) ? "System" : createdBy.Trim()
            };

            await _unitOfWork.ApplicationEvent.AddAsync(evt).ConfigureAwait(false);

            var saved = await _unitOfWork.SaveAsync().ConfigureAwait(false);
            if (saved <= 0)
            {
                throw new InvalidOperationException("Failed to persist calendar event.");
            }

            return evt.EventId;
        }
        
        /// <summary>
        /// Asynchronously deletes a calendar event identified by the specified event ID.
        /// </summary>
        /// <param name="eventId">The unique identifier of the calendar event to delete. Cannot be an empty GUID.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="eventId"/> is an empty GUID.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the delete operation fails.</exception>
        public async Task DeleteEventAsync(Guid eventId, CancellationToken ct = default)
        {
            if (eventId == Guid.Empty)
            {
                throw new ArgumentException("EventId is required.", nameof(eventId));
            }

            var evt = await _unitOfWork.ApplicationEvent.GetAsync(e => e.EventId == eventId).ConfigureAwait(false);
            if (evt is null)
            {
                return;
            }

            var ok = await _unitOfWork.ApplicationEvent.RemoveAsync(evt).ConfigureAwait(false);
            if (!ok)
            {
                throw new InvalidOperationException("Delete failed.");
            }

            await _unitOfWork.SaveAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves a list of calendar events that are active and fall within the specified date range.
        /// </summary>
        /// <remarks>If both <paramref name="start"/> and <paramref name="end"/> are provided, only events
        /// that overlap with the specified range are returned. If either parameter is null, all active events are
        /// returned without date filtering.</remarks>
        /// <param name="start">The start of the date range, in UTC. Only events occurring on or after this date are included. If null, no
        /// lower bound is applied.</param>
        /// <param name="end">The end of the date range, in UTC. Only events occurring before this date are included. If null, no upper
        /// bound is applied.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A read-only list of calendar event view models representing the active events within the specified date
        /// range, ordered by start time. The list is empty if no events match the criteria.</returns>
        public async Task<IReadOnlyList<CalendarEventViewModel>> GetCalendarEventsAsync(DateTime? start, DateTime? end, CancellationToken ct = default)
        {
            var all = await _unitOfWork.ApplicationEvent.GetAllAsync().ConfigureAwait(false);

            if (start.HasValue && end.HasValue)
            {
                var startUtc = EnsureUtc(start.Value);
                var endUtc = EnsureUtc(end.Value);

                all = all
                    .Where(e => e.IsActive &&
                                e.StartUtc < endUtc &&
                                (e.EndUtc ?? e.StartUtc) >= startUtc)
                    .ToList();
            }
            else
            {
                all = all.Where(e => e.IsActive).ToList();
            }

            return all
                .OrderBy(e => e.StartUtc)
                .Select(e => new CalendarEventViewModel
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    StartUtc = e.StartUtc,
                    EndUtc = e.EndUtc,
                    AllDay = e.AllDay,
                    Category = e.Category,
                    ColorHex = e.ColorHex,
                    Description = e.Description
                })
                .ToList();
        }

        /// <summary>
        /// Asynchronously updates an existing calendar event with the specified form data.
        /// </summary>
        /// <remarks>The event is updated with the values provided in the form. Only existing events can
        /// be updated; attempting to update a non-existent event will result in an exception.</remarks>
        /// <param name="form">The form data containing updated values for the calendar event. Must not be null, and must specify a valid
        /// event identifier.</param>
        /// <param name="modifiedBy">The identifier of the user or system making the modification. If null or whitespace, defaults to "System".</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="form"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="form.EventId"/> is <see cref="Guid.Empty"/>.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if no calendar event with the specified event identifier is found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the update operation fails to persist changes.</exception>
        public async Task UpdateEventAsync(CalendarEventFormViewModel form, string modifiedBy, CancellationToken ct = default)
        {
            if (form is null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            if (form.EventId == Guid.Empty)
            {
                throw new ArgumentException("EventId is required.", nameof(form.EventId));
            }

            var evt = await _unitOfWork.ApplicationEvent.GetAsync(e => e.EventId == form.EventId).ConfigureAwait(false);
            if (evt is null)
            {
                throw new KeyNotFoundException("Event not found.");
            }

            evt.Title = string.IsNullOrWhiteSpace(form.Title) ? evt.Title : form.Title.Trim();
            evt.StartUtc = EnsureUtc(form.StartUtc);
            evt.EndUtc = form.EndUtc.HasValue ? EnsureUtc(form.EndUtc.Value) : null;
            evt.AllDay = form.AllDay;
            evt.Category = string.IsNullOrWhiteSpace(form.Category) ? null : form.Category.Trim();
            evt.ColorHex = string.IsNullOrWhiteSpace(form.ColorHex) ? null : form.ColorHex.Trim();
            evt.Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim();
            evt.ModifiedOnUtc = _helperService.GetCurrentTime();
            evt.ModifiedBy = string.IsNullOrWhiteSpace(modifiedBy) ? "System" : modifiedBy.Trim();

            await _unitOfWork.ApplicationEvent.UpdateApplicationEventAsync(evt).ConfigureAwait(false);

            var saved = await _unitOfWork.SaveAsync().ConfigureAwait(false);
            if (saved <= 0)
            {
                throw new InvalidOperationException("Failed to update calendar event.");
            }
        }

        #region Private Methods 

        /// <summary>
        /// Converts the specified <see cref="DateTime"/> value to Coordinated Universal Time (UTC) if it is not already
        /// in UTC.
        /// </summary>
        /// <remarks>If <paramref name="input"/> has an unspecified <see cref="DateTimeKind"/>, the method
        /// treats it as a UTC value without conversion. If <paramref name="input"/> is local time, it is converted to
        /// UTC using the system's local time zone settings.</remarks>
        /// <param name="input">The date and time value to convert. If the value is already in UTC, it is returned unchanged.</param>
        /// <returns>A <see cref="DateTime"/> value that represents the same moment in time as <paramref name="input"/>,
        /// expressed as UTC.</returns>
        private static DateTime EnsureUtc(DateTime input)
        {
            return input.Kind switch
            {
                DateTimeKind.Utc => input,
                DateTimeKind.Local => input.ToUniversalTime(),
                _ => DateTime.SpecifyKind(input, DateTimeKind.Utc)
            };
        }
        #endregion
    }
}
