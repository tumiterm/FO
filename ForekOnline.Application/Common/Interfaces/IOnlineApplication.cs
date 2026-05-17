// <copyright file="IOnlineApplication.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    12-02-2026 22:24 PM
// Purpose:         Defines the IOnlineApplication interface.

#region Usings
using ForekOnline.Application.Common.Interfaces;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Defines the contract for managing and updating the new online applications entities.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to address management.</remarks>
    public interface IOnlineApplication : IRepository<OnlineApplication>
    {
        /// <summary>
        /// Updates the specified online application in the system and returns the updated online application.
        /// </summary>
        /// <param name="application">The <see cref="OnlineApplication"/> object containing the updated application details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="OnlineApplication"/> object.</returns>
        Task<OnlineApplication> UpdateApplicationAsync(OnlineApplication application);
    }
}
