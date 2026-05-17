// <copyright file="ITraining.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-02-2025 15:48 PM
// Purpose:         Defines the ITraining interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for managing and updating training entities in the repository.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to training entities.</remarks>
    public interface ITraining : IRepository<Training>
    {
        /// <summary>
        /// Updates the specified training entity in the system asynchronously.
        /// </summary>
        /// <remarks>This method updates the training entity with the provided details. Ensure that the
        /// <see cref="Training"/> object  contains valid data and corresponds to an existing entity in the
        /// system.</remarks>
        /// <param name="training">The <see cref="Training"/> object containing the updated details. The object must not be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="Training"/> object.</returns>
        Task<Training> UpdateTrainingAsync(Training training);

    }
}
