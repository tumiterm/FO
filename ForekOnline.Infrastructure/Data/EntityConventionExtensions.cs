// <copyright file="EntityConventionExtensions.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/03/2026 22:30 PM
// Purpose:         Convention-based model configuration for IEntity types

#region Usings
using ForekOnline.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
#endregion

namespace ForekOnline.Infrastructure.Data
{
    /// <summary>
    /// Provides extension methods for applying entity conventions in EF Core.
    /// </summary>
    public static class EntityConventionExtensions
    {
        /// <summary>
        /// Scans the model for all entity types that implement any closed generic form of
        /// <see cref="IEntity{TKey}"/> and applies:
        /// <list type="bullet">
        ///   <item><description><c>RowVersion</c> configured as an EF Core row-version column (only if not already configured via <c>[Timestamp]</c>).</description></item>
        ///   <item><description>A global query filter: <c>WHERE IsDeleted = 0</c>.</description></item>
        ///   <item><description><c>DateCreated</c> / <c>DateModified</c> SQL default values.</description></item>
        /// </list>
        /// Call this once at the end of <see cref="DbContext.OnModelCreating"/>.
        /// </summary>
        /// <param name="modelBuilder">The EF Core model builder.</param>
        public static void ApplyEntityConventions(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;

                if (!ImplementsIEntity(clrType))
                    continue;

                if (Attribute.IsDefined(clrType, typeof(SkipAuditInterceptorAttribute)))
                    continue;

                var builder = modelBuilder.Entity(clrType);

                var rowVersionProp = clrType.GetProperty(nameof(IEntity<object>.RowVersion));
                if (rowVersionProp is not null
                    && !Attribute.IsDefined(rowVersionProp, typeof(TimestampAttribute)))
                {
                    builder.Property(nameof(IEntity<object>.RowVersion)).IsRowVersion();
                }

                var parameter = Expression.Parameter(clrType, "e");
                var isDeletedProperty = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var filterBody = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                var filterLambda = Expression.Lambda(filterBody, parameter);

                builder.HasQueryFilter(filterLambda);

                builder.Property(nameof(IAuditable.DateCreated))
                       .HasDefaultValueSql("SYSDATETIMEOFFSET()");

                builder.Property(nameof(IAuditable.DateModified))
                       .HasDefaultValueSql("SYSDATETIMEOFFSET()");
            }
        }

        /// <summary>
        /// Determines whether the given type implements <see cref="IEntity{TKey}"/> for any <c>TKey</c>.
        /// </summary>
        private static bool ImplementsIEntity(Type type)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>));
        }
    }
}