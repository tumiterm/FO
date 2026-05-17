// <copyright file="InAppNotificationRepository.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    21/04/2026 21:28 PM
// Purpose:         Defines the InAppNotificationRepository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Provides data access functionality for managing <see cref="InAppNotification"/> entities.
    /// </summary>
    public class InAppNotificationRepository : Repository<InAppNotification>, IInAppNotification
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="InAppNotificationRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public InAppNotificationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the count of unread notifications for a specific user.
        /// </summary>
        /// <param name="userId">The recipient user's ID.</param>
        /// <returns>The count of unread notifications.</returns>
        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.InAppNotifications
                .CountAsync(n => n.RecipientUserId == userId && !n.IsRead);
        }

        /// <summary>
        /// Marks a notification as read.
        /// </summary>
        /// <param name="notificationId">The notification ID.</param>
        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.InAppNotifications
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadUtc = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Marks all notifications as read for a specific user.
        /// </summary>
        /// <param name="userId">The recipient user's ID.</param>
        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var unread = await _context.InAppNotifications
                .Where(n => n.RecipientUserId == userId && !n.IsRead)
                .ToListAsync();

            var now = DateTimeHelper.GetCurrentSastDateTimeOffset().DateTime;
            foreach (var notification in unread)
            {
                notification.IsRead = true;
                notification.ReadUtc = now;
            }

            await _context.SaveChangesAsync();
        }
    }
}