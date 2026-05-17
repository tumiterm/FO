// <copyright file="IInAppNotification.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/04/2026 21:26 PM
// Purpose:         Defines the IInAppNotification interface

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Provides data access functionality for managing in-app notifications.
    /// </summary>
    public interface IInAppNotification : IRepository<InAppNotification>
    {
        /// <summary>
        /// Gets the count of unread notifications for a specific user.
        /// </summary>
        /// <param name="userId">The recipient user's ID.</param>
        /// <returns>The count of unread notifications.</returns>
        Task<int> GetUnreadCountAsync(Guid userId);

        /// <summary>
        /// Marks a notification as read.
        /// </summary>
        /// <param name="notificationId">The notification ID.</param>
        Task MarkAsReadAsync(Guid notificationId);

        /// <summary>
        /// Marks all notifications as read for a specific user.
        /// </summary>
        /// <param name="userId">The recipient user's ID.</param>
        Task MarkAllAsReadAsync(Guid userId);
    }
}