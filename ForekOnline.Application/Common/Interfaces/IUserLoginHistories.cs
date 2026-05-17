// <copyright file="IUserLoginHistories.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    28-02-2025 11:42 AM
// Purpose:         Defines the IUserLoginHistories interface.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for managing and updating userLoginHistory entities.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to userLoginHistory management.</remarks>
    public interface IUserLoginHistories : IRepository<UserLoginHistory>
    {
        /// <summary>
        /// Updates the specified userLoginHistory in the system and returns the updated userLoginHistory.
        /// </summary>
        /// <param name="userLoginHistory">The <see cref="UserLoginHistory"/> object containing the updated userLoginHistory details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="UserLoginHistory"/> object.</returns>
        Task<UserLoginHistory> UpdateLoginHistoryAsync(UserLoginHistory userLoginHistory);
    }
}
