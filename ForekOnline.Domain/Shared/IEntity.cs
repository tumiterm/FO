// <copyright file="IEntity.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/03/2026 22:05 PM
// Purpose:         Defines the IEntity

namespace ForekOnline.Domain.Shared
{
    /// <summary>
    /// Represents a base entity with concurrency support.
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's unique identifier.</typeparam>
    public interface IEntity<TKey> : IIdentifiable<TKey>, IAuditable, ISoftDeletable
    {
        /// <summary>
        /// Unique code for the entity.
        /// </summary>
        string? Code { get; set; }

        /// <summary>
        /// Name of the entity.
        /// </summary>
        string? Name { get; set; }

        /// <summary>
        /// Row version for optimistic concurrency.
        /// </summary>
        byte[] RowVersion { get; set; }
    }

}
