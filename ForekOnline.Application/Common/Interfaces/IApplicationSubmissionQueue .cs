// <copyright file="IApplicationSubmissionQueue.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    07-03-2026 20:24 PM
// Purpose:         Defines the IApplicationSubmissionQueue interface.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for managing and updating submissionQueue entities.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to submissionQueue management.</remarks>
    public interface IApplicationSubmissionQueue : IRepository<ApplicationSubmissionQueue>
    {
        /// <summary>
        /// Updates the specified submissionQueue in the system and returns the updated submissionQueue.
        /// </summary>
        /// <param name="submissionQueue">The <see cref="ApplicationSubmissionQueue"/> object containing the updated submissionQueue details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="ApplicationSubmissionQueue"/> object.</returns>
        Task<ApplicationSubmissionQueue> UpdateApplicationSubmissionQueueAsync(ApplicationSubmissionQueue submissionQueue);
    }
}
