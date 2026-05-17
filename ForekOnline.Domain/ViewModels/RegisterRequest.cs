// <copyright file="RegisterRequest.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    08/03/2026 09:54 PM
// Purpose:         Defines the RegisterRequest 

namespace ForekOnline.Domain.ViewModels
{
    public class RegisterRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the person.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the person.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identification number associated with the entity.
        /// </summary>
        public string? IdNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the passport number associated with the individual.
        /// </summary>
        public string? PassportNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identification number or passport number associated with the individual.
        /// </summary>
        public string? IdOrPassport { get; set; }

        /// <summary>
        /// Gets or sets the username associated with the user account.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier assigned to the student within the institution.
        /// </summary>
        public string? StudentNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cellphone number associated with the entity.
        /// </summary>
        public string Cellphone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password associated with the current user or entity.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
