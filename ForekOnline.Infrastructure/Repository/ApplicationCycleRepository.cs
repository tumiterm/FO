// <copyright file="ApplicationCycleRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    12-02-2026 22:41 PM
// Purpose:         Defines the ApplicationCycle Repository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the application cycle Repository.
    /// </summary>
    public class ApplicationCycleRepository : Repository<ApplicationCycle>, IApplicationCycle
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCycleRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ApplicationCycleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing applicationCycle model in the repository.
        /// </summary>
        /// <param name="applicationCycle">The applicationCycle model to be updated.</param>
        public async Task<ApplicationCycle> UpdateApplicationCycleAsync(ApplicationCycle applicationCycle)
        {
            _context.ApplicationCycles.Update(applicationCycle);

            await _context.SaveChangesAsync();

            return applicationCycle;
        }
    }
}
