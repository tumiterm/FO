// <copyright file="IAddress.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    10-01-2026 17:57 PM
// Purpose:         Defines the IStoredDocument interface.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for managing and updating storedDocument entities.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to storedDocument management.</remarks>
    public interface IStoredDocument : IRepository<StoredDocument>
    {
        /// <summary>
        /// Updates the specified storedDocument in the system and returns the updated storedDocument.
        /// </summary>
        /// <param name="storedDocument">The <see cref="StoredDocument"/> object containing the updated storedDocument details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="StoredDocument"/> object.</returns>
        Task<StoredDocument> UpdateStoredDocumentAsync(StoredDocument storedDocument);
    }
}
