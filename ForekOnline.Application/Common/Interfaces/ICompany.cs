// <copyright file="ICompany.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 13:06 PM
// Purpose:         Defines the ICompany interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for managing and updating company entities.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to company entities.</remarks>
    public interface ICompany : IRepository<Company>
    {
        /// <summary>
        /// Updates the details of an existing company asynchronously.
        /// </summary>
        /// <remarks>Ensure that the <paramref name="company"/> object includes all required fields for
        /// the update operation. The method performs the update operation asynchronously and returns the updated
        /// company details upon success.</remarks>
        /// <param name="company">The <see cref="Company"/> object containing the updated details. The object must include a valid identifier
        /// for the company to be updated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="Company"/> object.</returns>
        Task<Company> UpdateCompanyAsync(Company company);

    }
}
