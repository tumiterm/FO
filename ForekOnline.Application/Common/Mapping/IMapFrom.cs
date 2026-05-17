// <copyright file="IMapFrom.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/03/2026 23:30 PM
// Purpose:         Defines a contract for mapping FROM an entity TO this ViewModel/DTO

namespace ForekOnline.Domain.Mapping
{
    /// <summary>
    /// Defines a contract for a ViewModel or DTO that can be mapped FROM an entity of type <typeparamref name="TEntity"/>.
    /// Implement this interface to provide a factory method that creates a ViewModel from an entity.
    /// </summary>
    /// <typeparam name="TEntity">The source entity type.</typeparam>
    public interface IMapFrom<in TEntity> where TEntity : class
    {
        /// <summary>
        /// Creates and populates this instance from the given entity.
        /// </summary>
        /// <param name="entity">The source entity.</param>
        void MapFrom(TEntity entity);
    }
}