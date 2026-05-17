//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    09/Jan/2024 21:00 PM
// Purpose:         Defines the New IRepository interface.

#region Usings
using ForekOnline.Application.Common.Validations;
using System.Linq.Expressions;
#endregion

namespace ForekOnline.Application.Common.Interfaces
{
    /// <summary>
    /// Defines the contract for a generic repository.
    /// Provides methods for performing CRUD operations on entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities that match the specified filter.
        /// </summary>
        /// <param name="filter">An optional filter expression to apply.</param>
        /// <param name="includeProperties">Optional comma-separated list of related entities to include.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of entities.</returns>
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string[]? includeProperties = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? skip = null, int? take = null, bool asNoTracking = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a single entity that matches the specified filter.
        /// </summary>
        /// <param name="filter">A filter expression to apply.</param>
        /// <param name="includeProperties">Optional comma-separated list of related entities to include.</param>
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string[]? includeProperties = null, bool asNoTracking = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an existing entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<bool> RemoveAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an entity of type <typeparamref name="T"/> exists based on a specified predicate.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="predicate">The expression to test each element for a condition.</param>
        /// <returns><c>true</c> if the entity exists; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task<int> ExecuteSqlAsync(string sql, object[]? parameters = null, CancellationToken cancellationToken = default);

        Task Update(T entity);
        Task Attach(T entity);

        #region To-be phased in 2025

        // Summary:
        //     Get a single IEntity entity. If the query retrieves multiple entities, only the
        //     first one will be returned.
        //
        // Parameters:
        //   filter:
        //     Specifies the filter to be applied to the query
        //
        //   selector:
        //     Specifies the selector (projection) to be applied to the query
        //
        //   sort:
        //     Specifies the sorting to be applied to the query
        //
        //   includes:
        //     Specifies any additional includes in the query
        //
        //   asNoTracking:
        //     Flag to determine if tracking should be applied or not. If not set, the default
        //     for the main configuration is used
        //
        //   isDirtyRead:
        //     Flag to determine if the read should be a dirty ready or not
        //
        //   cancellationToken:
        //     A cancellation token
        //
        // Type parameters:
        //   TEntity:
        //     The IEntity type to be queried and return
        //
        // Returns:
        //     The entity
        Task<TEntity> GetEntityAsync<TEntity>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> selector = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sort = null, string includes = null, bool? asNoTracking = null, bool isDirtyRead = false, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IEntity;

        //
        // Summary:
        //     Get a single entity, but project into another entity. If the query retrieves
        //     multiple entities, only the first one will be returned.
        //
        // Parameters:
        //   filter:
        //     Specifies the filter to be applied to the query
        //
        //   selector:
        //     Specifies the selector (projection) to be applied to the query
        //
        //   sort:
        //     Specifies the sorting to be applied to the query
        //
        //   includes:
        //     Specifies any additional includes in the query
        //
        //   asNoTracking:
        //     Flag to determine if tracking should be applied or not. If not set, the default
        //     for the main configuration is used
        //
        //   isDirtyRead:
        //     Flag to determine if the read should be a dirty ready or not
        //
        //   cancellationToken:
        //     A cancellation token
        //
        // Type parameters:
        //   TEntity:
        //     The entity type to be queried
        //
        //   TResult:
        //     The entity type to be returned
        //
        // Returns:
        //     The projected entity
        Task<TResult> GetEntityAsync<TEntity, TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> selector, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sort = null, string includes = null, bool? asNoTracking = null, bool isDirtyRead = false, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IEntity;

        //
        // Summary:
        //     Get a list of entities without any filter applied. Should not be executed on
        //     tables with a large number of records.
        //
        // Parameters:
        //   selector:
        //     Specifies the selector (projection) to be applied to the query
        //
        //   sort:
        //     Specifies the sorting to be applied to the query
        //
        //   includes:
        //     Specifies any additional includes in the query
        //
        //   asNoTracking:
        //     Flag to determine if tracking should be applied or not. If not set, the default
        //     for the main configuration is used
        //
        //   isDirtyRead:
        //     Flag to determine if the read should be a dirty ready or not
        //
        //   take:
        //     The maximum number of records to be returned
        //
        //   skip:
        //     The number of record to skip
        //
        //   cancellationToken:
        //     A cancellation token
        //
        // Type parameters:
        //   TEntity:
        //     The entity type to be queried and returned
        //
        // Returns:
        //     A collection of entities
        Task<ICollection<TEntity>> GetEntitiesAsync<TEntity>(Expression<Func<TEntity, TEntity>> selector = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sort = null, string includes = null, bool? asNoTracking = null, bool isDirtyRead = false, int take = 0, int skip = 0, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IEntity;


        /// <summary>
        /// Executes a command asynchronously against the database.
        /// </summary>
        /// <param name="command">The SQL command to execute.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous command execution, returning the number of affected rows.</returns>
        Task<int> ExecuteCommandAsync(string command, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// Executes a command synchronously against the database.
        /// </summary>
        /// <param name="command">The SQL command to execute.</param>
        /// <returns>The number of affected rows.</returns>
        int ExecuteCommand(string command);


        /// <summary>
        /// Executes a query asynchronously and returns the results as a collection of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="asNoTracking">Specifies whether to disable change tracking for the entities.</param>
        /// <param name="isDirtyRead">Indicates whether to perform a dirty read (read uncommitted data).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous query operation, returning a collection of entities.</returns>
        Task<ICollection<TEntity>> ExecuteQueryAsync<TEntity>(string query, bool? asNoTracking = null, bool isDirtyRead = false, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, new();


        // Summary:
        //     Gets the underlaying provider for the implementation of IEntity
        //
        // Returns:
        //     The provider
        object GetProvider();

        //
        // Summary:
        //     Creates the entity and peforms validation before persisting it to the database
        //
        //
        // Parameters:
        //   entity:
        //     The entity
        //
        //   cancellationToken:
        //     A cancellation token
        //
        // Type parameters:
        //   TEntity:
        //     The IEntity type
        //
        // Returns:
        //     The validation response
        Task<ValidationResponse> SaveEntityAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IEntity;

        #endregion

    }
}
