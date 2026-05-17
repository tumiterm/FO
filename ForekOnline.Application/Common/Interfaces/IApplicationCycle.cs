// <copyright file="IOnlineApplication.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    12-02-2026 22:24 PM
// Purpose:         Defines the IApplicationCycle  interface.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for managing and updating the new online applications entities.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to address management.</remarks>
    public interface IApplicationCycle : IRepository<ApplicationCycle>
    {
        /// <summary>
        /// Updates the specified application cycle in the system and returns the updated application cycle.
        /// </summary>
        /// <param name="applicationCycle">The <see cref="ApplicationCycle"/> object containing the updated application details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="ApplicationCycle"/> object.</returns>
        Task<ApplicationCycle> UpdateApplicationCycleAsync(ApplicationCycle applicationCycle);
    }
}
