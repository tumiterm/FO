// <copyright file="INotificationContentBlock.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    18-10-2025 15:41 PM
// Purpose:         Defines the INotificationContentBlock interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Represents a repository for managing <see cref="NotificationContentBlock"/> entities  and provides functionality
    /// to update the address of a notification event.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to include additional operations 
    /// specific to <see cref="NotificationContentBlock"/> entities.</remarks>
    public interface INotificationContentBlock : IRepository<NotificationContentBlock>
    {
        /// <summary>
        /// Updates the specified notification content block with new data.
        /// </summary>
        /// <param name="notificationEvent">The <see cref="NotificationContentBlock"/> instance containing the updated data.  This parameter cannot be
        /// <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated  <see
        /// cref="NotificationContentBlock"/> instance.</returns>
        Task<NotificationContentBlock> UpdateNotificationContentBlockAsync(NotificationContentBlock notificationEvent);
    }
}
