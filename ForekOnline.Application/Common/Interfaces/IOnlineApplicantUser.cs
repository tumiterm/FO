// <copyright file="IOnlineApplicantUser.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/Feb/2025 21:56 PM
// Purpose:         Defines the IOnlineApplicantUser interface

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Represents a repository for managing online applicant users, providing operations to update online applicant users
    /// information asynchronously.
    /// </summary>
    public interface IOnlineApplicantUser : IRepository<OnlineApplicationUser>
    {
        /// <summary>
        /// Updates the specified onlineApplicationUser in the system and returns the updated onlineApplicationUser.
        /// </summary>
        /// <param name="onlineApplicationUser">The <see cref="OnlineApplicationUser"/> object containing the online applicant users details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="OnlineApplicationUser"/> object.</returns>
        Task<OnlineApplicationUser> UpdateOnlineApplicantUserAsync(OnlineApplicationUser onlineApplicationUser);
    }
}
