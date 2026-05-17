// <copyright file="INotification.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    18-10-2025 15:39 PM
// Purpose:         Defines the INotification interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Represents a service for managing notification events, including operations to update notification details.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to notification events.</remarks>
    public interface INotification : IRepository<NotificationEvent>
    {
       /// <summary>
       /// Updates the specified notification event in the system.
       /// </summary>
       /// <param name="notificationEvent">The <see cref="NotificationEvent"/> instance containing the updated event details. The instance must not be
       /// <see langword="null"/>.</param>
       /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
       /// cref="NotificationEvent"/> instance.</returns>
        Task<NotificationEvent> UpdateNotificationEventAsync(NotificationEvent notificationEvent);
    }
}
