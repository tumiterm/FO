// <copyright file="OnlineApplicantUserRepository.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/Feb/2025 22:05 PM
// Purpose:         Defines the OnlineApplicantUserRepository

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the OnlineApplicantUser Repository.
    /// </summary>
    public class OnlineApplicantUserRepository : Repository<OnlineApplicationUser>, IOnlineApplicantUser
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineApplicantUserRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public OnlineApplicantUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing onlineApplicationUser model in the repository.
        /// </summary>
        /// <param name="onlineApplicationUser">The onlineApplicationUser model to be updated.</param>
        public async Task<OnlineApplicationUser> UpdateOnlineApplicantUserAsync(OnlineApplicationUser onlineApplicationUser)
        {
            _context.OnlineApplicationUsers.Update(onlineApplicationUser);

            await _context.SaveChangesAsync();

            return onlineApplicationUser;
        }
    }
}
