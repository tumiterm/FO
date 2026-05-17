// <copyright file="UserLoginHistoryRepository.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    20-01-2026 20:24 APM
// Purpose:         Defines the UserLoginHistoryRepository Repository.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a repository specifically for performing operations on the user login hitory Repository.
    /// </summary>
    public class UserLoginHistoryRepository : Repository<UserLoginHistory>, IUserLoginHistories
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginHistoryRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public UserLoginHistoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates an existing user login hitory model in the repository.
        /// </summary>
        /// <param name="userLoginHistory">The UserLoginHistoryRepository model to be updated.</param>
        public async Task<UserLoginHistory> UpdateLoginHistoryAsync(UserLoginHistory userLoginHistory)
        {
            _context.UserLoginHistory.Update(userLoginHistory);

            await _context.SaveChangesAsync();

            return userLoginHistory;
        }
    }
}
