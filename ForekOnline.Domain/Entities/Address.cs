// <copyright file="Address.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2023 11:00 AM
// Purpose:         Defines the Address class

#region Usings
using ForekOnline.Domain.Shared;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents an address with details such as street name, city, province, and postal code.
    /// </summary>
    [SkipAuditInterceptor]
    public class Address
    {
        /// <summary>
        /// Gets or sets the unique identifier for the address.
        /// </summary>
        public Guid AddressId { get; set; }

        /// <summary>
        /// Gets or sets the name of the street.
        /// </summary>
        public string? StreetName { get; set; }

        /// <summary>
        /// Gets or sets additional address information (e.g., apartment, suite, or unit number).
        /// </summary>
        public string? Line1 { get; set; }

        /// <summary>
        /// Gets or sets the name of the city.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the province or state of the address.
        /// </summary>
        public eProvince? Province { get; set; }

        /// <summary>
        /// Gets or sets the postal or ZIP code.
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the associative identifier, which links this address to another entity.
        /// </summary>
        public Guid AssociativeId { get; set; }
    }
}
