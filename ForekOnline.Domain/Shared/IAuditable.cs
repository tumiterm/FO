// <copyright file="IAuditable.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/02/2025 15:47 PM
// Purpose:         Defines the IAuditable interface

namespace ForekOnline.Domain.Shared
{
    /// <summary>
    /// Represents an auditable entity with creation and modification tracking.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// Date and time when the entity was created.
        /// </summary>
        DateTimeOffset DateCreated { get; set; }

        /// <summary>
        /// Date and time when the entity was last modified.
        /// </summary>
        DateTimeOffset DateModified { get; set; }

        /// <summary>
        /// User who created the entity.
        /// </summary>
        string? UserCreated { get; set; }

        /// <summary>
        /// User who last modified the entity.
        /// </summary>
        string? UserModified { get; set; }
    }
}