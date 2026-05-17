// <copyright file="IGuardian.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 13:20 PM
// Purpose:         Defines the IGuardian interface.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Represents a repository for managing guardian entities, providing methods for retrieving, updating, and
    /// persisting guardian data.
    /// </summary>
    public interface IGuardian : IRepository<Guardian>
    {
        /// <summary>
        /// Asynchronously updates the specified guardian record.
        /// </summary>
        /// <param name="guardian">The guardian entity containing the updated information. Must not be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated guardian entity.</returns>
        Task<Guardian> UpdateGuardianAsync(Guardian guardian);

    }
}
