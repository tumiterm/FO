// <copyright file="Guardian.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2023 11:00 AM
// Purpose:         Defines the Guardian class

#region Usings
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ForekOnline.Domain.Enums.EnumRegistry;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a guardian associated with an application.
    /// </summary>
    public class Guardian
    {
        /// <summary>
        /// Gets or sets the unique identifier for the guardian.
        /// </summary>
        [Key]
        public Guid GuardianId { get; set; }

        /// <summary>
        /// Gets or sets the application ID that this guardian is associated with.
        /// </summary>
        [ForeignKey("Application")]
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the guardian.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the guardian.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the relationship of the guardian to the applicant.
        /// </summary>
        public eRelationship Relationship { get; set; }

        /// <summary>
        /// Gets or sets the cellphone number of the guardian.
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// Gets or sets the file path or name of the guardian's ID document.
        /// </summary>
        public string? IDDoc { get; set; }

        /// <summary>
        /// Gets or sets the uploaded ID document file. This property is not mapped to the database.
        /// </summary>
        [NotMapped]
        public IFormFile? IDFile { get; set; }
    }

}
