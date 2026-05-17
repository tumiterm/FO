
// <copyright file="EmployeeContact.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    05/01/2025 11:26:27 PM
// Purpose:         Defines the EmployeeContact class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a view model for an employee's contact details.
    /// </summary>
    [SkipAuditInterceptor]
    public class EmployeeContact
    {
        // <summary>
        /// Gets or sets Unique Id of the employee.
        /// </summary>
        [Key]
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the employee.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the employee.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address of the employee.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number of the employee.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the department to which the employee belongs.
        /// </summary>
        public eDepartment Department { get; set; }

        /// <summary>
        /// Gets or sets the position or job title of the employee within the organization.
        /// </summary>
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the employee's profile image. Can be null if not set.
        /// </summary>
        public string? ProfileImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Cellphone Number Visibility of the employee's.
        /// </summary>
        public bool IsCellNumberVisible { get; set; }

    }
}
