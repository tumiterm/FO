// <copyright file="Company.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2023 11:00 AM
// Purpose:         Defines the Company class

#region Usings
using ForekOnline.Domain.Shared;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a company entity with relevant details.
    /// </summary>
    [SkipAuditInterceptor]
    public class Company : Base
    {
        /// <summary>
        /// Gets or sets the unique identifier for the company.
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the address of the company.
        /// </summary>
        public Address Address { get; set; }

        public Guid AddressId { get; set; } 

        /// <summary>
        /// Gets or sets the contact person details for the company.
        /// </summary>
        public ContactPerson Contact { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the contact.
        /// </summary>
        public Guid ContactId { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the company.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the company's area of specialization.
        /// </summary>
        public eSpeciality Speciality { get; set; }
    }

}
