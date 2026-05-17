// <copyright file="IApplicationRejection.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 13:03 PM
// Purpose:         Defines the IApplicationRejection interface.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for managing and updating application rejection records.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to  <see cref="ApplicationRejection"/> entities, including asynchronous updates.</remarks>
    public interface IApplicationRejection : IRepository<ApplicationRejection>
    {
        /// <summary>
        /// Updates the specified application rejection in the system.
        /// </summary>
        /// <param name="applicationRejection">The <see cref="ApplicationRejection"/> object containing the updated rejection details.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="ApplicationRejection"/> object.</returns>
        Task<ApplicationRejection> UpdateApplicationRejectionAsync(ApplicationRejection applicationRejection);
    }
}
