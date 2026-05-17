// <copyright file="IApplications.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    26-06-2025 22:32 PM
// Purpose:         Defines the IApplications interface.

#region Usings
using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines a contract for managing and persisting <see cref="Domain.Entities.Application"/> entities,  including
    /// retrieval, storage, and updates.
    /// </summary>
    /// <remarks>This interface extends the <see cref="IRepository{T}"/> interface, providing additional
    /// functionality  specific to <see cref="Domain.Entities.Application"/> entities. It includes methods for updating 
    /// application data asynchronously.</remarks>
    public interface IApplications : IRepository<Domain.Entities.Application>
    {
        /// <summary>
        /// Updates the specified application entity in the system.
        /// </summary>
        /// <param name="application">The application entity to update. The entity must contain valid data, including a non-null identifier.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated application entity.</returns>
        Task<Domain.Entities.Application> UpdateApplicationAsync(Domain.Entities.Application application);

    }
}
