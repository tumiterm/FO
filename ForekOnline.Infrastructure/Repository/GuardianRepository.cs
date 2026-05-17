// <copyright file="GuardianRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 16:49 PM
// Purpose:         Defines the GuardianRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion


namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Guardian Repository.
    /// </summary>
    public class GuardianRepository : Repository<Guardian>, IGuardian
    {

        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuardianRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public GuardianRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Guardian model in the repository.
        /// </summary>
        /// <param name="guardian">The Guardian model to be updated.</param>
        public async Task<Guardian> UpdateGuardianAsync(Guardian guardian)
        {
            _context.Guardians.Update(guardian);

            await _context.SaveChangesAsync();

            return guardian;
        }
    }
}
