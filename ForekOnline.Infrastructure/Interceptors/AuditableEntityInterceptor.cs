// <copyright file="AuditableEntityInterceptor.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/03/2026 22:30 PM
// Purpose:         Intercepts SaveChanges to auto-populate audit and soft-delete fields

#region Usings
using ForekOnline.Domain.Entities;
using ForekOnline.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Concurrent;
#endregion

namespace ForekOnline.Infrastructure.Interceptors
{
    /// <summary>
    /// EF Core interceptor that automatically populates audit fields
    /// (<see cref="IAuditable"/>) and soft-delete fields (<see cref="ISoftDeletable"/>)
    /// on every <c>SaveChanges</c> / <c>SaveChangesAsync</c> call.
    /// </summary>
    public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        #region Fields
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Thread-safe cache so we only reflect on each CLR type once.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, bool> _skipCache = new();
        #endregion

        /// <summary>
        /// Initializes a new instance of <see cref="AuditableEntityInterceptor"/>.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context for resolving the authenticated user.</param>
        public AuditableEntityInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <inheritdoc />
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyAuditInfo(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        /// <inheritdoc />
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        #region Private Helper Methods

        /// <summary>
        /// Iterates over tracked entities and sets audit/soft-delete fields
        /// based on the current <see cref="EntityState"/>.
        /// Entities decorated with <see cref="SkipAuditInterceptorAttribute"/> are ignored.
        /// </summary>
        private void ApplyAuditInfo(DbContext? context)
        {
            if (context is null)
                return;

            var now = DateTimeHelper.GetCurrentSastDateTimeOffset();
            var currentUser = ResolveCurrentUserName();

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (ShouldSkip(entry.Entity.GetType()))
                    continue;

                if (entry.Entity is IAuditable auditable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditable.DateCreated = now;
                            auditable.DateModified = now;
                            auditable.UserCreated = currentUser;
                            auditable.UserModified = currentUser;
                            break;

                        case EntityState.Modified:
                            auditable.DateModified = now;
                            auditable.UserModified = currentUser;

                            entry.Property(nameof(IAuditable.DateCreated)).IsModified = false;
                            entry.Property(nameof(IAuditable.UserCreated)).IsModified = false;
                            break;
                    }
                }

                if (entry.Entity is ISoftDeletable softDeletable && entry.State == EntityState.Modified)
                {
                    if (softDeletable.IsDeleted && softDeletable.DateDeleted is null)
                    {
                        softDeletable.DateDeleted = now;
                    }
                    else if (!softDeletable.IsDeleted)
                    {
                        softDeletable.DateDeleted = null;
                    }
                }

                if (entry.State == EntityState.Added && entry.Entity is IIdentifiable<Guid>)
                {
                    var idProperty = entry.Property(nameof(IIdentifiable<Guid>.Id));
                    if (idProperty.CurrentValue is Guid guid && guid == Guid.Empty)
                    {
                        idProperty.CurrentValue = Guid.NewGuid();
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the entity type is decorated with <see cref="SkipAuditInterceptorAttribute"/>.
        /// Results are cached per type for performance.
        /// </summary>
        private static bool ShouldSkip(Type entityType)
        {
            return _skipCache.GetOrAdd(entityType, static type =>
                Attribute.IsDefined(type, typeof(SkipAuditInterceptorAttribute)));
        }

        /// <summary>
        /// Resolves the current user's display name from the session via <see cref="IHttpContextAccessor"/>.
        /// Falls back to <c>"System"</c> when no session or user is available
        /// (e.g. background jobs, migrations).
        /// </summary>
        private string ResolveCurrentUserName()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session is null)
                    return "System";

                var sessionUserJson = session.GetString("SessionUser");
                if (string.IsNullOrWhiteSpace(sessionUserJson))
                    return "System";

                var user = JsonConvert.DeserializeObject<User>(sessionUserJson);
                if (user is null)
                    return "System";

                return $"{user.Name} {user.LastName}".Trim();
            }
            catch
            {
                return "System";
            }
        }

        #endregion
    }
}