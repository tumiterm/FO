// <copyright file="IAddress.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    28-02-2025 11:42 AM
// Purpose:         Defines the IAddress interface.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for managing and updating address entities.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to address management.</remarks>
    public interface IAddress : IRepository<Address>
    {
        /// <summary>
        /// Updates the specified address in the system and returns the updated address.
        /// </summary>
        /// <param name="address">The <see cref="Address"/> object containing the updated address details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="Address"/> object.</returns>
        Task<Address> UpdateAddressAsync(Address address);
    }
}
