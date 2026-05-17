// <copyright file="OnlineApplicationRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    12-02-2026 22:41 PM
// Purpose:         Defines the OnlineApplication Repository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the onlineApplication Repository.
    /// </summary>
    public class OnlineApplicationRepository : Repository<OnlineApplication>, IOnlineApplication
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineApplicationRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public OnlineApplicationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing onlineApplication model in the repository.
        /// </summary>
        /// <param name="onlineApplication">The onlineApplication model to be updated.</param>
        public async Task<OnlineApplication> UpdateApplicationAsync(OnlineApplication onlineApplication)
        {
            _context.OnlineApplications.Update(onlineApplication);

            await _context.SaveChangesAsync();

            return onlineApplication;
        }
    }
}
