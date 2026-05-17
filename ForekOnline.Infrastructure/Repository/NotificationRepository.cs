// <copyright file="NotificationRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    18-10-2025 15:45 PM
// Purpose:         Defines the NotificationRepository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Provides data access functionality for managing <see cref="NotificationEvent"/> entities.
    /// </summary>
    /// <remarks>This repository is responsible for performing operations on <see cref="NotificationEvent"/>
    /// entities within the underlying database context. It extends the base repository functionality and implements the
    /// <see cref="INotification"/> interface.</remarks>
    public class NotificationRepository : Repository<NotificationEvent>, INotification
    {
        /// <summary>
        /// Represents the database context used for interacting with the application's data store.
        /// </summary>
        /// <remarks>This field is read-only and is intended to provide access to the database context
        /// within the class. It should be initialized through dependency injection or during the construction of the
        /// containing class.</remarks>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRepository"/> class with the specified database
        /// context.
        /// </summary>
        /// <param name="context">The <see cref="ApplicationDbContext"/> used to access the database. This parameter cannot be <see
        /// langword="null"/>.</param>
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing notification event in the database.
        /// </summary>
        /// <remarks>This method updates the provided notification event in the database and saves the
        /// changes asynchronously. Ensure that the <paramref name="notificationEvent"/> instance is tracked by the
        /// database context before calling this method.</remarks>
        /// <param name="notificationEvent">The <see cref="NotificationEvent"/> instance to update. The instance must already exist in the database.</param>
        /// <returns>The updated <see cref="NotificationEvent"/> instance.</returns>
        public async Task<NotificationEvent> UpdateNotificationEventAsync(NotificationEvent notificationEvent)
        {
            _context.NotificationEvents.Update(notificationEvent);

            await _context.SaveChangesAsync();

            return notificationEvent;
        }


    }
}
