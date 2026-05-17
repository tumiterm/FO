// <copyright file="ApplicationEventRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-12-2025 18:37 PM
// Purpose:         Defines the ApplicationEvent Repository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Provides a repository for managing <see cref="ApplicationEvent"/> entities, including operations for retrieving,
    /// updating, and persisting data.
    /// </summary>
    /// <remarks>This class extends the <see cref="Repository{T}"/> base class to provide additional
    /// functionality specific to <see cref="ApplicationEvent"/> entities. It also implements the <see
    /// cref="IApplicationEvent"/> interface to ensure adherence to the application's event-related contract.</remarks>
    public class ApplicationEventRepository : Repository<ApplicationEvent>, IApplicationEvent
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationEventRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ApplicationEventRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing ApplicationEvent model in the repository.
        /// </summary>
        /// <param name="applicationEvent">The ApplicationEvent model to be updated.</param>
        public async Task<ApplicationEvent> UpdateApplicationEventAsync(ApplicationEvent applicationEvent)
        {
            _context.ApplicationEvent.Update(applicationEvent);

            await _context.SaveChangesAsync();

            return applicationEvent;
        }
    }
}
