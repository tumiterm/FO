// <copyright file="ApplicationSubmissionQueueRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    07-03-2026 20:31 PM
// Purpose:         Defines the ApplicationSubmissionQueueRepository Repository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the ApplicationSubmissionQueue Repository.
    /// </summary>
    public class ApplicationSubmissionQueueRepository : Repository<ApplicationSubmissionQueue>, IApplicationSubmissionQueue
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSubmissionQueueRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ApplicationSubmissionQueueRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing ApplicationSubmissionQueue model in the repository.
        /// </summary>
        /// <param name="applicationSubmissionQueue">The ApplicationSubmissionQueue model to be updated.</param>
        public async Task<ApplicationSubmissionQueue> UpdateApplicationSubmissionQueueAsync(ApplicationSubmissionQueue applicationSubmissionQueue)
        {
            _context.ApplicationSubmissionQueue.Update(applicationSubmissionQueue);

           // await _context.SaveChangesAsync();

            return applicationSubmissionQueue;
        }
    }
}
