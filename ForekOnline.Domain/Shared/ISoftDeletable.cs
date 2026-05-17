// <copyright file="ISoftDeletable.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/02/2025 15:46 PM
// Purpose:         Defines the ISoftDeletable interface

namespace ForekOnline.Domain.Shared
{
    /// <summary>
    /// Represents a soft-deletable entity.
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Indicates whether the entity is deleted (retired).
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Date and time when the entity was deleted (retired), if applicable.
        /// </summary>
        DateTimeOffset? DateDeleted { get; set; }
    }
}
