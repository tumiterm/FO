// <copyright file="OnlineApplicationUser.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/02/2025 21:46 PM
// Purpose:         Defines the OnlineApplicationUser model

#region Usings
using ForekOnline.Domain.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace ForekOnline.Domain.Entities
{
    /// <summary>
    /// Represents a user who is applying online, including personal identification and contact information, as well as
    /// entity metadata.
    /// </summary>
    /// <remarks>This class encapsulates details relevant to an online application user, such as names,
    /// identification numbers, and associated contact person. It also includes standard entity properties for tracking
    /// creation, modification, and deletion metadata. The type is typically used to manage and persist user data within
    /// online application workflows.</remarks>
    [Table("OnlineApplicationUser", Schema = "FO")]
    public class OnlineApplicationUser : EntityBase<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

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
        /// Gets or sets the username associated with the user account.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        public string Cellphone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier assigned to the student within the institution.
        /// </summary>
        public string? StudentNumber { get; set; } = string.Empty;


        #region IEntity

        /// <summary>
        /// Gets or sets the unique code that identifies the entity.
        /// </summary>
        public string? Code { get; set ; }

        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string? Name { get; set ; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTimeOffset DateCreated { get; set ; }

        /// <summary>
        /// Gets or sets the date and time when the item was last modified.
        /// </summary>
        public DateTimeOffset DateModified { get; set ; }

        /// <summary>
        /// Gets or sets the username of the user who created the entity.
        /// </summary>
        public string? UserCreated { get; set ; }

        /// <summary>
        /// Gets or sets the username of the user who last modified the entity.
        /// </summary>
        public string? UserModified { get; set ; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been marked as deleted.
        /// </summary>
        public bool IsDeleted { get; set ; }

        /// <summary>
        /// Gets or sets the date and time when the entity was deleted, if applicable.
        /// </summary>
        /// <remarks>If the entity has not been deleted, this property will be <see langword="null"/>. The
        /// value is typically used to track soft-deletion or archival status.</remarks>
        public DateTimeOffset? DateDeleted { get; set; }
        #endregion
    }
}
