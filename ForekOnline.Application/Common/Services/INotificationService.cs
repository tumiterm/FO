// <copyright file="INotificationService.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    18/10/2025 15:29 PM
// Purpose:         Defines the INotificationService interface

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Services
{
    /// <summary>
    /// Defines a contract for a service that manages notification events.
    /// </summary>
    /// <remarks>This interface provides methods for retrieving notification events based on specific
    /// criteria, such as user roles and time constraints. Implementations of this interface are expected to handle the
    /// logic for determining which notifications are active and relevant.</remarks>
    public interface INotificationService
    {
        /// <summary>
        /// Retrieves a list of active notification events based on the specified user role and the current UTC time.
        /// </summary>
        /// <param name="userRole">The role of the user for whom the active notification events are being retrieved.  Can be <see
        /// langword="null"/> to retrieve events not specific to any role.</param>
        /// <param name="utcNow">The current UTC time used to determine which notification events are considered active.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of  <see
        /// cref="NotificationEvent"/> objects representing the active notification events. The list will be empty  if
        /// no active events are found.</returns>
        Task<IReadOnlyList<NotificationEvent>> GetActiveAsync(string? userRole, DateTime utcNow);

        /// <summary>
        /// Retrieves a notification event by its unique identifier.
        /// </summary>
        /// <remarks>This method performs an asynchronous lookup for a notification event based on the
        /// provided identifier. If no matching event is found, the method returns <see langword="null"/>.</remarks>
        /// <param name="id">The unique identifier of the notification event to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the  <see
        /// cref="NotificationEvent"/> if found; otherwise, <see langword="null"/>.</returns>
        Task<NotificationEvent?> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new notification event with the specified content blocks.
        /// </summary>
        /// <remarks>This method creates a new notification event and associates it with the provided
        /// content blocks.  The operation is performed asynchronously and may be canceled using the provided <paramref
        /// name="ct"/>.</remarks>
        /// <param name="ev">The notification event to create. Must not be <see langword="null"/>.</param>
        /// <param name="blocks">A collection of content blocks to associate with the notification event. Must not be <see langword="null"/>
        /// or empty.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see
        /// cref="NotificationEvent"/>.</returns>
        Task<NotificationEvent> CreateAsync(NotificationEvent ev, IEnumerable<NotificationContentBlock> blocks, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing notification event with the specified content blocks.
        /// </summary>
        /// <remarks>This method updates the specified notification event by associating it with the
        /// provided content blocks.  Ensure that the event and content blocks are valid and meet any required
        /// constraints before calling this method.</remarks>
        /// <param name="ev">The notification event to update. Cannot be <see langword="null"/>.</param>
        /// <param name="blocks">A collection of content blocks to associate with the notification event. Cannot be <see langword="null"/> or
        /// empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>The updated <see cref="NotificationEvent"/> if the operation succeeds; otherwise, <see langword="null"/> if
        /// the update fails.</returns>
        Task<NotificationEvent?> UpdateAsync(NotificationEvent ev, IEnumerable<NotificationContentBlock> blocks, CancellationToken ct = default);

        /// <summary>
        /// Deactivates the entity with the specified identifier.
        /// </summary>
        /// <remarks>The method will return <see langword="false"/> if the entity with the specified
        /// identifier does not exist or is already deactivated.</remarks>
        /// <param name="id">The unique identifier of the entity to deactivate.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns><see langword="true"/> if the entity was successfully deactivated; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default);
    }
}
