// <copyright file="IEmployeeContact.cs" company="Forek ICT">
//     Copyright © Forek ICT.
// </copyright>
// Created By:      Itumeleng Oliphant (on LT4CD7175A2BAC)
// Created Date:    28-02-2025 11:42 AM
// Purpose:         Defines the IEmployeeContact interface.

#region Usings
using ForekOnline.Domain.Entities;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines operations for managing employee contact information, including updating contact details asynchronously.
    /// </summary>
    public interface IEmployeeContact : IRepository<EmployeeContact>
    {
        /// <summary>
        /// Asynchronously updates the contact information for an employee.
        /// </summary>
        /// <param name="address">The employee contact details to update. Cannot be null. The identifier within the provided object determines
        /// which employee's contact information will be updated.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated employee contact
        /// information.</returns>
        Task<EmployeeContact> UpdateEmployeeContactAsync(EmployeeContact address);
    }
}
