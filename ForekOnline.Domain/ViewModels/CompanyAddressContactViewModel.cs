// <copyright file="CompanyAddressContactViewModel.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/08/2023 13:18 PM
// Purpose:         Defines the CompanyAddressContactViewModel class

using static ForekOnline.Domain.Enums.EnumRegistry;

namespace ForekOnline.Domain.ViewModels
{
    /// <summary>
    /// Represents the contact and address details of a company.
    /// </summary>
    public class CompanyAddressContactViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the company.
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the company.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the company's area of specialization.
        /// </summary>
        public eSpeciality Speciality { get; set; }

        /// <summary>
        /// Gets or sets the street name of the company's address.
        /// </summary>
        public string? StreetName { get; set; }

        /// <summary>
        /// Gets or sets additional address details (e.g., building or unit number).
        /// </summary>
        public string? Line1 { get; set; }

        /// <summary>
        /// Gets or sets the city where the company is located.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the province where the company is located.
        /// </summary>
        public eProvince? Province { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the company's address.
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the first name of the company's contact person.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the last name of the company's contact person.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the cellphone number of the company's contact person.
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// Gets or sets the email address of the company's contact person.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the company is active.
        /// </summary>
        public bool IsActive { get; set; }
    }

}
