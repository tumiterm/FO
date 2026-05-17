// <copyright file="EmployeeContactsViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/01/2025 20:01:27 PM
// Purpose:         Defines the EmployeeContactsViewModel class


using ForekOnline.Domain.Entities;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents a view model for employee contacts, including a user and their associated contact list.
    /// </summary>
    public class EmployeeContactsViewModel
    {
        /// <summary>
        /// Gets or sets the user associated with the employee contacts.
        /// </summary>
        public User? User {  get; set; }

        /// <summary>
        /// Gets or sets the list of employee contacts.
        /// </summary>
        public IReadOnlyList<EmployeeContact> Contacts { get; set; }
    }
}
