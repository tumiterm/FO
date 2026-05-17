// <copyright file="IApplicationEvent.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    27-12-2025 18:29 PM
// Purpose:         Defines the IApplicationEvent interface.

using ForekOnline.Domain.Entities;

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for managing and updating address entities.
    /// </summary>
    /// <remarks>This interface extends <see cref="IRepository{T}"/> to provide additional functionality
    /// specific to application event management.</remarks>
    public interface IApplicationEvent : IRepository<ApplicationEvent>
    {
        /// <summary>
        /// Updates the specified application event in the system and returns the updated application event.
        /// </summary>
        /// <param name="applicationEvent">The <see cref="ApplicationEvent"/> object containing the updated application event details. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="ApplicationEvent"/> object.</returns>
        Task<ApplicationEvent> UpdateApplicationEventAsync(ApplicationEvent applicationEvent);
    }
}
