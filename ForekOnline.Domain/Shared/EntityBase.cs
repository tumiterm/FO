// <copyright file="EntityBase.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/03/2026 22:30 PM
// Purpose:         Defines the abstract EntityBase class

#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.Shared
{
    /// <summary>
    /// Abstract base class that implements <see cref="IEntity{TKey}"/>.
    /// Provides default implementations for identity, auditing, soft-delete,
    /// and concurrency control — eliminating repetitive property declarations.
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's unique identifier.</typeparam>
    public abstract class EntityBase<TKey> : IEntity<TKey>
    {
        #region IIdentifiable

        /// <inheritdoc />
        public TKey Id { get; set; } = default!;

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            if (other is not EntityBase<TKey> entity)
                return false;

            if (ReferenceEquals(this, entity))
                return true;

            if (EqualityComparer<TKey>.Default.Equals(Id, default) ||
                EqualityComparer<TKey>.Default.Equals(entity.Id, default))
                return false;

            return EqualityComparer<TKey>.Default.Equals(Id, entity.Id);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return EqualityComparer<TKey>.Default.Equals(Id, default)
                ? base.GetHashCode()
                : Id!.GetHashCode();
        }

        #endregion

        #region IEntity

        /// <inheritdoc />
        [StringLength(50)]
        public string? Code { get; set; }

        /// <inheritdoc />
        [StringLength(250)]
        public string? Name { get; set; }

        /// <inheritdoc />
        [Timestamp]
        public byte[] RowVersion { get; set; } = [];

        #endregion

        #region IAuditable

        /// <inheritdoc />
        public DateTimeOffset DateCreated { get; set; }

        /// <inheritdoc />
        public DateTimeOffset DateModified { get; set; }

        /// <inheritdoc />
        [StringLength(256)]
        public string? UserCreated { get; set; }

        /// <inheritdoc />
        [StringLength(256)]
        public string? UserModified { get; set; }

        #endregion

        #region ISoftDeletable

        /// <inheritdoc />
        public bool IsDeleted { get; set; }

        /// <inheritdoc />
        public DateTimeOffset? DateDeleted { get; set; }

        #endregion
    }
}