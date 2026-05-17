// <copyright file="ContactPerson.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2023 11:00 AM
// Purpose:         Defines the ContactPerson class

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a contact person associated with a company or entity.
    /// </summary>
    [SkipAuditInterceptor]
    public class ContactPerson
    {
        /// <summary>
        /// Gets or sets the unique identifier for the contact person.
        /// </summary>
        [Key]
        public Guid ContactId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated entity (e.g., company).
        /// </summary>
        public Guid AssociativeId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the contact person.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the last name of the contact person.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the cellphone number of the contact person.
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// Gets or sets the email address of the contact person.
        /// </summary>
        public string Email { get; set; }
    }

}
