// <copyright file="IIdentifiable.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    04/02/2025 15:47 PM
// Purpose:         Defines the IIdentifiable interface

namespace ForekOnline.Domain.Shared
{
    // <summary>
    /// Represents an identifiable entity with a unique key.
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's unique identifier.</typeparam>
    public interface IIdentifiable<TKey>
    {
        /// <summary>
        /// Unique identifier for the entity.
        /// </summary>
        TKey Id { get; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        bool Equals(object? other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        int GetHashCode();
    }
}
