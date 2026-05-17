//     Copyright © Forek ICT.
// </copyright>
// Created By:      IF Oliphant (on IFOliphantPC)
// Created Date:    09/Jan/2024 21:00 PM
// Purpose:         Defines the Generic Repo class.

#region Usings
using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Validations;
using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace ForekOnline.Infrastructure.Repository
{
    /// <summary>
    /// Represents a generic repository for performing CRUD operations on entities of type T.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly ApplicationDbContext _db;  

        /// <summary>
        /// The database set for the entity type T.
        /// </summary>
        internal DbSet<T> dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="db">The application database context.</param>
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        /// <summary>
        /// Asynchronously adds an entity to the database.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>The added entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                if (entity == null) throw new ArgumentNullException(nameof(entity));

                await dbSet.AddAsync(entity, cancellationToken);

                return entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Asynchronously retrieves an entity from the database based on a given filter.
        /// </summary>
        /// <param name="filter">A lambda expression to filter the entity.</param>
        /// <param name="includeProperties">An optional array of related entities to include in the query.</param>
        /// <param name="asNoTracking">Indicates whether the entity should be retrieved without tracking.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>The first entity that matches the filter or null if none is found.</returns>
        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter,string[]? includeProperties = null, bool asNoTracking = false, CancellationToken cancellationToken = default)
        {
            try
            {
                IQueryable<T> query = BuildQuery(dbSet, filter, includeProperties, asNoTracking);

                return await query.FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Asynchronously retrieves a list of entities from the database based on given parameters.
        /// </summary>
        /// <param name="filter">An optional lambda expression to filter the entities.</param>
        /// <param name="includeProperties">An optional array of related entities to include in the query.</param>
        /// <param name="orderBy">An optional function to order the entities.</param>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to retrieve.</param>
        /// <param name="asNoTracking">Indicates whether the entities should be retrieved without tracking.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>A read-only list of entities matching the criteria.</returns>
        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string[]? includeProperties = null,Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? skip = null,int? take = null,bool asNoTracking = false, CancellationToken cancellationToken = default)
        {
            try
            
            {
                IQueryable<T> query = BuildQuery(dbSet, filter, includeProperties, asNoTracking);

                if (orderBy != null)
                    query = orderBy(query);

                if (skip.HasValue)
                    query = query.Skip(skip.Value);

                if (take.HasValue)
                    query = query.Take(take.Value);

                return await query.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Asynchronously removes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to be removed.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>A boolean indicating whether the removal was successful.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
        public async Task<bool> RemoveAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                if (entity == null) throw new ArgumentNullException(nameof(entity));

                dbSet.Remove(entity);
                return await _db.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Asynchronously checks whether any entity exists in the database that matches a given predicate.
        /// </summary>
        /// <param name="predicate">An optional lambda expression to filter the entities.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>A boolean indicating whether an entity exists that matches the criteria.</returns>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                return predicate != null
                    ? await dbSet.AnyAsync(predicate, cancellationToken)
                    : await dbSet.AnyAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Asynchronously executes a raw SQL command.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="parameters">An optional array of parameters for the query.</param>
        /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
        /// <returns>The number of affected rows.</returns>
        /// <exception cref="ArgumentException">Thrown when the SQL command is empty or null.</exception>
        public async Task<int> ExecuteSqlAsync(string sql, object[]? parameters = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sql))
                    throw new ArgumentException("SQL command cannot be empty", nameof(sql));

                return parameters != null
                    ? await _db.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken)
                    : await _db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            dbSet.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public async Task Attach(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            dbSet.Attach(entity);
        }


        #region To-be phased-In

        /// <summary>
        /// Gets a list of entities projected into the result type
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter">Specifies the filter to be applied</param>
        /// <param name="selector">Specified the selector (projection) to be applied</param>
        /// <param name="sort">Specifies the sorting to be applied</param>
        /// <param name="includes">Specifies any additional includes in the query</param>
        /// <param name="asNoTracking">Flag to determing if tracking should be applied or not</param>
        /// <param name="isDirtyRead">Flag tp determing if the read should be a dirty read or not</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        public async Task<TEntity> GetEntityAsync<TEntity>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> selector, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sort, string includes, bool? asNoTracking, bool isDirtyRead, CancellationToken cancellationToken) where TEntity : class, IEntity
        {
            IQueryable<TEntity> query = _db.Set<TEntity>();

            if (asNoTracking.GetValueOrDefault())
            {
                query = query.AsNoTracking();
            }

            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))

                    query = query.Include(include);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (sort != null)
            {
                query = sort(query);
            }

            return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="filter">Specifies the filter to be applied</param>
        /// <param name="selector">Specified the selector (projection) to be applied</param>
        /// <param name="sort">Specifies the sorting to be applied</param>
        /// <param name="includes">Specifies any additional includes in the query</param>
        /// <param name="asNoTracking">Flag to determing if tracking should be applied or not</param>
        /// <param name="isDirtyRead">Flag tp determing if the read should be a dirty read or not</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        public async Task<TResult> GetEntityAsync<TEntity, TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> selector, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sort, string includes, bool? asNoTracking, bool isDirtyRead, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            IQueryable<TEntity> query = _db.Set<TEntity>();

            if (asNoTracking.GetValueOrDefault())
            {
                query = query.AsNoTracking();
            }

            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))

                    query = query.Include(include);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (sort != null)
            {
                query = sort(query);
            }

            return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Gets a list of entities projected into the result type
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="selector"></param>
        /// <param name="sort"></param>
        /// <param name="includes"></param>
        /// <param name="asNoTracking"></param>
        /// <param name="isDirtyRead"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        public async Task<ICollection<TEntity>> GetEntitiesAsync<TEntity>(Expression<Func<TEntity, TEntity>> selector, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sort, string includes, bool? asNoTracking, bool isDirtyRead, int take, int skip, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            IQueryable<TEntity> query = _db.Set<TEntity>();

            if (asNoTracking.GetValueOrDefault())
            {
                query = query.AsNoTracking();
            }

            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))

                    query = query.Include(include);
            }

            if (sort != null)
            {
                query = sort(query);
            }

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take > 0)
            {
                query = query.Take(take);
            }

            if (selector != null)
            {
                return await query.Select(selector).ToListAsync(cancellationToken);
            }

            return await query.ToListAsync(cancellationToken);
        }


        /// <summary>
        /// Execute a command
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>The number of affected rows</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> ExecuteCommandAsync(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException("Command cannot be null or empty.", nameof(command));
            }

            return await _db.Database.ExecuteSqlRawAsync(command, cancellationToken);
        }

        /// <summary>
        /// Execute a command
        /// </summary>
        /// <param name="command"> SQL Command to be executed</param>
        /// <returns>The number of affected rows</returns>
        /// <exception cref="ArgumentException"></exception>
        public int ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException("Command cannot be null or empty.", nameof(command));
            }

            return _db.Database.ExecuteSqlRaw(command);
        }
        /// <summary>
        /// Execute a query
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query">The query to be executed</param>
        /// <param name="asNoTracking">Flag to determing if tracking should be applied</param>
        /// <param name="isDirtyRead">Flag to determing it the read should be dirty read or not</param>
        /// <param name="cancellationToken">A Cancellation Token</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<ICollection<TEntity>> ExecuteQueryAsync<TEntity>(string query, bool? asNoTracking = null, bool isDirtyRead = false, CancellationToken cancellationToken = default) where TEntity : class, new()
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));

            }

            IQueryable<TEntity> queryable = _db.Set<TEntity>().FromSqlRaw(query);

            if (asNoTracking.GetValueOrDefault())
            {
                queryable = queryable.AsNoTracking();
            }

            return await queryable.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the underlying provider for the implemetation of IEntity
        /// </summary>
        /// <returns></returns>
        public object GetProvider()
        {
            return _db.Database.GetDbConnection().DataSource;
        }

        /// <summary>
        /// Saves the entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ValidationResponse> SaveEntityAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            var validationResponse = ValidateEntity(entity);

            if (!validationResponse.IsError)
            {
                return validationResponse;
            }

            _db.Add(entity);

            await _db.SaveChangesAsync(cancellationToken);

            return validationResponse;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Validates an entity based on predefined rules.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to validate.</typeparam>
        /// <param name="entity">The entity to be validated.</param>
        /// <returns>A validation response indicating whether the entity is valid.</returns>
        private ValidationResponse ValidateEntity<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            var validationResponse = new ValidationResponse();

            // Perform validation logic here
            // For example, check for required fields, data integrity, etc.
            // Add validation errors to validationResponse if necessary:
            // validationResponse.Errors.Add("Some validation error message");

            validationResponse.IsValid = !validationResponse.Errors.Any();
            return validationResponse;
        }

        /// <summary>
        /// Builds a query based on the provided filter, included properties, and tracking behavior.
        /// </summary>
        /// <param name="query">The base query to modify.</param>
        /// <param name="filter">An optional lambda expression to filter the entities.</param>
        /// <param name="includeProperties">An optional array of related entities to include.</param>
        /// <param name="asNoTracking">Indicates whether the query should be executed without tracking.</param>
        /// <returns>An IQueryable representing the modified query.</returns>
        private static IQueryable<T> BuildQuery( IQueryable<T> query, Expression<Func<T, bool>>? filter,string[]? includeProperties,bool asNoTracking)
        {
            if (filter != null)
                query = query.Where(filter);

            if (includeProperties?.Length > 0)
            {
                foreach (var include in includeProperties.Where(ip => !string.IsNullOrWhiteSpace(ip)))
                {
                    query = query.Include(include);
                }
            }

            return asNoTracking ? query.AsNoTracking() : query;
        }
        #endregion
    }
}
