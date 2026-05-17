// <copyright file="ICategory.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    15-03-2025 08:50 AM
// Purpose:         Defines the ICategory interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Represents a repository interface for managing <see cref="ResourceCategory"/> entities.
    /// </summary>
    /// <remarks>This interface extends the <see cref="IRepository{T}"/> interface, providing additional
    /// functionality specific to <see cref="ResourceCategory"/> entities.</remarks>
    public interface ICategory : IRepository<ResourceCategory>
    {
        /// <summary>
        /// Updates the specified resource category asynchronously.
        /// </summary>
        /// <param name="resourceCategory">The resource category to update. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="ResourceCategory"/>.</returns>
        Task<ResourceCategory> UpdateCategoryAsync(ResourceCategory resourceCategory);
    }
}
