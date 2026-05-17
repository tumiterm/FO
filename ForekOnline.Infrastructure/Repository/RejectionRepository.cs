// <copyright file="RejectionRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 17:18 PM
// Purpose:         Defines the RejectionRepository interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the Rejections Repository.
    /// </summary>
    public class RejectionRepository : Repository<ApplicationRejection>, IApplicationRejection
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RejectionRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public RejectionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing Rejection model in the repository.
        /// </summary>
        /// <param name="applicationRejection">The Rejection model to be updated.</param>
        public async Task<ApplicationRejection> UpdateApplicationRejectionAsync(ApplicationRejection applicationRejection)
        {
            _context.RejectionTBL.Update(applicationRejection);

            await _context.SaveChangesAsync();

            return applicationRejection;
        }

    }
}
