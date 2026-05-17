// <copyright file="NotificationContentBlockRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    18-10-2025 15:50 PM
// Purpose:         Defines the NotificationContentBlockRepository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Provides data access functionality for managing notification content blocks in the application's data store.
    /// </summary>
    /// <remarks>This repository is responsible for performing CRUD operations on <see
    /// cref="NotificationContentBlock"/> entities,  leveraging the provided <see cref="ApplicationDbContext"/> for
    /// database interactions. It extends the base  <see cref="Repository{T}"/> class and implements the <see
    /// cref="INotificationContentBlock"/> interface.</remarks>
    public class NotificationContentBlockRepository : Repository<NotificationContentBlock>, INotificationContentBlock
    {
        /// <summary>
        /// Represents the database context used for interacting with the application's data store.
        /// </summary>
        /// <remarks>This field is read-only and is intended to provide access to the database context
        /// within the class. It should be initialized through dependency injection or during the construction of the
        /// containing class.</remarks>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationContentBlockRepository"/> class  with the specified
        /// database context.
        /// </summary>
        /// <remarks>This repository provides data access functionality for notification content blocks, 
        /// leveraging the provided <see cref="ApplicationDbContext"/> to perform database operations.</remarks>
        /// <param name="context">The <see cref="ApplicationDbContext"/> instance used to interact with the database.  This parameter cannot
        /// be <see langword="null"/>.</param>
        public NotificationContentBlockRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing notification content block in the database.
        /// </summary>
        /// <remarks>This method updates the specified notification content block in the database and
        /// saves the changes asynchronously. Ensure that the provided <paramref name="notification"/> object is
        /// properly initialized and represents an existing record in the database.</remarks>
        /// <param name="notification">The <see cref="NotificationContentBlock"/> instance to update.  The instance must already exist in the
        /// database.</param>
        /// <returns>The updated <see cref="NotificationContentBlock"/> instance.</returns>
        public async Task<NotificationContentBlock> UpdateNotificationContentBlockAsync(NotificationContentBlock notification)
        {
            _context.NotificationContentBlocks.Update(notification);

            await _context.SaveChangesAsync();

            return notification;
        }
    }
}
