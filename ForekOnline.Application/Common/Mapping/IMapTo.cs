// <copyright file="IMapTo.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/03/2026 23:30 PM
// Purpose:         Defines a contract for mapping FROM this ViewModel/DTO TO an entity

namespace ForekOnline.Domain.Mapping
{
    /// <summary>
    /// Defines a contract for a ViewModel or DTO that can be mapped TO an entity of type <typeparamref name="TEntity"/>.
    /// Implement this interface to provide a method that applies ViewModel values onto an entity.
    /// </summary>
    /// <typeparam name="TEntity">The target entity type.</typeparam>
    public interface IMapTo<TEntity> where TEntity : class
    {
        /// <summary>
        /// Applies values from this ViewModel/DTO onto the given entity.
        /// Use for updates — the entity is already tracked by EF Core.
        /// </summary>
        /// <param name="entity">The target entity to update.</param>
        void MapTo(TEntity entity);

        /// <summary>
        /// Creates a new entity instance populated from this ViewModel/DTO.
        /// Use for inserts — returns a detached entity ready for <c>DbSet.Add</c>.
        /// </summary>
        /// <returns>A new entity populated with values from this ViewModel/DTO.</returns>
        TEntity ToEntity();
    }
}