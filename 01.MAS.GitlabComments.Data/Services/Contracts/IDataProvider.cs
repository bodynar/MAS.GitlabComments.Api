namespace MAS.GitlabComments.Data.Services
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Data.Filter;
    using MAS.GitlabComments.Data.Models;

    /// <summary>
    /// Provider of data for specified entity type
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    public interface IDataProvider<TEntity>
        where TEntity : BaseEntity
    {
        /// <summary>
        /// Add entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Add(TEntity entity);

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>All entities</returns>
        IEnumerable<TEntity> Get();

        /// <summary>
        /// Get entity by it identifier
        /// </summary>
        /// <param name="entityId">Identifier value</param>
        /// <returns>Entity by identifier if it found; otherwise null</returns>
        TEntity Get(Guid entityId);

        /// <summary>
        /// Update specified by identifier entity with new column values
        /// </summary>
        /// <param name="entityId">Identifier value</param>
        /// <param name="newValues">Entity new column values</param>
        void Update(Guid entityId, IDictionary<string, object> newValues);

        /// <summary>
        /// Delete entities by their identifiers
        /// </summary>
        /// <param name="entityIds">Array of identifier values</param>
        void Delete(params Guid[] entityIds);

        /// <summary>
        /// Get entities with filter
        /// </summary>
        /// <param name="filter">Filter config</param>
        /// <returns>Filtered entities</returns>
        IEnumerable<TEntity> Where(FilterGroup filter);
    }
}
