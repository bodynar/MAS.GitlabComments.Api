﻿namespace MAS.GitlabComments.DataAccess.Services.Implementations.DataProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Exceptions;
    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.DataAccess.Select;

    /// <summary>
    /// Provider of data for specified entity type
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    public class SqlDataProvider<TEntity> : IDataProvider<TEntity>
        where TEntity : BaseEntity
    {
        /// <inheritdoc cref="IDbConnectionFactory"/>
        private IDbConnectionFactory DbConnectionFactory { get; }

        /// <inheritdoc cref="IDbAdapter"/>
        private IDbAdapter DbAdapter { get; }

        /// <inheritdoc cref="IFilterBuilder"/>
        private IFilterBuilder FilterBuilder { get; }

        /// <inheritdoc cref="ComplexColumnQueryBuilder"/>
        private IComplexColumnQueryBuilder ComplexColumnQueryBuilder { get; }

        /// <summary>
        /// Database table name for entity
        /// </summary>
        private string TableName { get; }

        /// <summary>
        /// List of entity fields
        /// </summary>
        private IEnumerable<string> EntityFields { get; }

        /// <summary>
        /// List of default entity fields which cannot be set manually
        /// </summary>
        private static IEnumerable<string> DefaultEntityFields { get; }
            = typeof(BaseEntity).GetProperties().Select(x => x.Name);

        /// <summary>
        /// Initializing <see cref="SqlDataProvider{TEntity}"/>
        /// </summary>
        /// <param name="dbConnectionFactory">Factory providing database connection</param>
        /// <param name="dbAdapter">Database operation adapter</param>
        /// <param name="filterBuilder">Sql filter builder</param>
        /// <param name="complexColumnBuilder">Complex column query builder</param>
        /// <exception cref="ArgumentNullException">Parameter dbConnectionFactory is null</exception>
        /// <exception cref="ArgumentNullException">Parameter dbAdapter is null</exception>
        /// <exception cref="ArgumentNullException">Parameter filterBuilder is null</exception>
        /// <exception cref="ArgumentNullException">Parameter complexColumnBuilder is null</exception>
        public SqlDataProvider(IDbConnectionFactory dbConnectionFactory, IDbAdapter dbAdapter, IFilterBuilder filterBuilder, IComplexColumnQueryBuilder complexColumnBuilder)
        {
            DbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(DbConnectionFactory));
            DbAdapter = dbAdapter ?? throw new ArgumentNullException(nameof(dbAdapter));
            FilterBuilder = filterBuilder ?? throw new ArgumentNullException(nameof(filterBuilder));
            ComplexColumnQueryBuilder = complexColumnBuilder ?? throw new ArgumentNullException(nameof(complexColumnBuilder));
            TableName = typeof(TEntity).Name;

            if (!TableName.EndsWith("s"))
            {
                TableName += "s";
            }

            EntityFields = GetEntityFields();
        }

        /// <summary>
        /// Add entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Identifier column value of new entity</returns>
        /// <exception cref="ArgumentNullException">Parameter entity is null</exception>
        /// <exception cref="QueryExecutionException"><typeparamref name="TEntity"/> is empty</exception>
        /// <exception cref="QueryExecutionException">No record were inserted</exception>
        public Guid Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var parameters =
                entity
                    .GetType()
                    .GetProperties()
                    .Where(x => EntityFields.Contains(x.Name))
                    .Select((x, i) => {
                        var boxedValue = x.GetValue(entity);

                        return new QueryParameter
                        {
                            ColumnName = x.Name,
                            Value = boxedValue,
                            ParameterName = $"@P{i + 1}",
                        };
                    });

            if (!parameters.Any())
            {
                throw new QueryExecutionException(
                    QueryExecutionExceptionState.Before,
                    $"Operation cannot be performed due to empty model \"{entity.GetType().FullName}\""
                );
            }

            var map = parameters.ToDictionary(x => x.ColumnName);
            Guid id = Guid.NewGuid();

            if (map.ContainsKey(nameof(BaseEntity.Id)))
            {
                id = (map[nameof(BaseEntity.Id)].Value as Guid?).Value;
            }
            else
            {
                map.Add(nameof(BaseEntity.Id), new QueryParameter
                {
                    Value = id,
                    ColumnName = nameof(BaseEntity.Id),
                    ParameterName = $"@P{map.Count + 1}",
                });
            }

            map.Set(nameof(BaseEntity.CreatedOn), DateTime.UtcNow);
            map.Set(nameof(BaseEntity.ModifiedOn), DateTime.UtcNow);

            var query = $"INSERT INTO [{TableName}]" +
                $" ({string.Join(", ", map.Keys.Select(x => $"[{x}]"))})" +
                $" VALUES ({string.Join(", ", map.Values.Select(x => x.ParameterName))})"
            ;

            int affectedRows = 0;

            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                affectedRows = DbAdapter.Execute(
                    connection,
                    query,
                    map.ToDictionary(x => x.Value.ParameterName, x => x.Value.Value)
                );
            }

            if (affectedRows == 0)
            {
                throw new QueryExecutionException(
                    QueryExecutionExceptionState.After,
                    "Insert command performed with empty result, no record was added"
                );
            }

            return id;
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>All entities</returns>
        public IEnumerable<TEntity> Get()
        {
            var entities = Enumerable.Empty<TEntity>();

            var sqlQuery = $"SELECT * FROM [{TableName}]";
            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                entities = DbAdapter.Query<TEntity>(connection, sqlQuery).ToList();
            }

            return entities;
        }

        /// <summary>
        /// Get entity by it identifier
        /// </summary>
        /// <param name="entityId">Identifier value</param>
        /// <returns>Entity by identifier if it found; otherwise null</returns>
        /// <exception cref="ArgumentNullException">Parameter entityId is not set</exception>
        public TEntity Get(Guid entityId)
        {
            if (entityId == default)
            {
                throw new ArgumentNullException(nameof(entityId));
            }

            TEntity entity = null;
            var sqlQuery = $"SELECT * FROM [{TableName}] WHERE Id = @P1";

            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                entity = DbAdapter.Query<TEntity>(connection, sqlQuery, new Dictionary<string, object>() { { "P1", entityId } }).FirstOrDefault();
            }

            return entity;
        }

        /// <summary>
        /// Update specified by identifier entity with new column values
        /// </summary>
        /// <param name="entityId">Identifier value</param>
        /// <param name="newValues">Entity new column values</param>
        /// <exception cref="ArgumentNullException">Parameter entityId is default</exception>
        /// <exception cref="ArgumentNullException">Parameter newValues is null or empty</exception>
        /// <exception cref="Exception">Update command doesn't applied to any record</exception>
        public void Update(Guid entityId, IDictionary<string, object> newValues)
        {
            if (entityId == default)
            {
                throw new ArgumentNullException(nameof(entityId));
            }
            if (newValues == null || !newValues.Any())
            {
                throw new ArgumentNullException(nameof(newValues));
            }

            var setStatements = new List<string>();
            var arguments = new Dictionary<string, object>();
            var argumentsCount = 0;

            var parameterName = $"@P{argumentsCount + 1}";

            foreach (var pair in newValues.Where(x => !string.IsNullOrEmpty(x.Key) && x.Value != null))
            {
                var isDefaultValue = IsDefaultValue(pair.Value, pair.Value.GetType());

                if (!isDefaultValue && EntityFields.Contains(pair.Key))
                {
                    if (arguments.TryAdd(parameterName, pair.Value))
                    {
                        setStatements.Add($"[{pair.Key}] = {parameterName}");
                        argumentsCount++;
                    }
                }
            }

            if (!setStatements.Any())
            {
                return;
            }

            parameterName = $"@P{argumentsCount + 1}";
            if (arguments.TryAdd(parameterName, DateTime.UtcNow))
            {
                setStatements.Add($"[ModifiedOn] = {parameterName}");
                argumentsCount++;
            }

            var setStatement = string.Join(", ", setStatements);

            if (!arguments.TryAdd($"@P{++argumentsCount}", entityId))
            {
                return;
            }

            var sqlQuery = $"UPDATE [{TableName}] SET {setStatement} WHERE [Id] = @P{argumentsCount}";
            int affectedRows = 0;

            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                affectedRows = DbAdapter.Execute(connection, sqlQuery, arguments);
            }

            if (affectedRows == 0)
            {
                throw new Exception("Update command performed with empty result, no record was updated.");
            }
        }

        /// <summary>
        /// Delete entities by their identifiers
        /// </summary>
        /// <param name="entityIds">Array of identifier values</param>
        /// <exception cref="ArgumentNullException">Parameter entityIds is null</exception>
        public void Delete(params Guid[] entityIds)
        {
            if (entityIds == null)
            {
                throw new ArgumentNullException(nameof(entityIds));
            }

            entityIds = entityIds.Where(x => x != default).ToArray();
            if (entityIds.Any())
            {
                var sqlQuery = $"DELETE FROM [{TableName}] WHERE [Id] IN @P1";

                using (var connection = DbConnectionFactory.CreateDbConnection())
                {
                    DbAdapter.Execute(connection, sqlQuery, new Dictionary<string, object>() { { "P1", entityIds } });
                }
            }
        }

        /// <summary>
        /// Get entities with filter
        /// </summary>
        /// <param name="filter">Filter config</param>
        /// <exception cref="ArgumentException">Filter contains column not presented in Entity</exception>
        /// <returns>Filtered entities</returns>
        public IEnumerable<TEntity> Where(FilterGroup filter)
        {
            if (filter == null || filter.IsEmpty)
            {
                return Get();
            }

            var (filterSql, filterArgs) = GetFilterValue(filter);

            if (string.IsNullOrEmpty(filterSql))
            {
                return Get();
            }

            IEnumerable<TEntity> entities = Enumerable.Empty<TEntity>();
            var sqlQuery = $"SELECT * FROM [{TableName}] WHERE {filterSql}";

            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                entities = DbAdapter.Query<TEntity>(connection, sqlQuery, filterArgs).ToList();
            }

            return entities;
        }

        /// <summary>
        /// Select entities into custom model
        /// </summary>
        /// <typeparam name="TProjection">Projection model type</typeparam>
        /// <param name="configuration">Select configuration</param>
        /// <exception cref="ArgumentNullException">Parameter configuration is null</exception>
        /// <exception cref="ArgumentException">Filter contains column not presented in Entity</exception>
        /// <returns>Entities mapped to specified model</returns>
        public IEnumerable<TProjection> Select<TProjection>(SelectConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var filterSql = string.Empty;
            IReadOnlyDictionary<string, object> filterArgs = new Dictionary<string, object>();

            if (configuration.Filter != null && !configuration.Filter.IsEmpty)
            {
                var builtFilter = GetFilterValue(configuration.Filter);

                filterSql = $"WHERE {builtFilter.Item1}";
                filterArgs = builtFilter.Item2;
            }

            var complexColumnData = ComplexColumnQueryBuilder.BuildComplexColumns<TProjection>(TableName);

            var columns = "*";
            var joinPart = "";

            if (complexColumnData != null)
            {
                columns = string.Join($",", complexColumnData.Columns.Select(column => column.ToString()));

                joinPart = string.Join(" ", complexColumnData.Joins.Select(joinData => joinData.ToQueryPart()));
            }

            var entities = Enumerable.Empty<TProjection>();
            var sqlQuery = $"SELECT {columns} FROM [{TableName}] {joinPart} {filterSql}".TrimEnd(' ');

            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                entities = DbAdapter.Query<TProjection>(connection, sqlQuery, filterArgs).ToList();
            }

            return entities;
        }

        #region Not public API

        /// <summary>
        /// Get entity field names
        /// </summary>
        /// <returns>List of entity field names</returns>
        private static IEnumerable<string> GetEntityFields()
        {
            return typeof(TEntity)
                .GetProperties()
                .Select(x => x.Name)
                .Except(DefaultEntityFields);
        }

        /// <summary>
        /// Check if boxed value is default value
        /// </summary>
        /// <param name="boxedValue">Boxed value</param>
        /// <param name="type">Type of value in the box</param>
        /// <returns>True if boxed value is default; otherwise false</returns>
        private static bool IsDefaultValue(object boxedValue, Type type)
        {
            if (!type.IsValueType)
            {
                return boxedValue == default;
            }

            var def = Activator.CreateInstance(type);
            return boxedValue.Equals(def);
        }

        /// <summary>
        /// Building sql filter from filter configuration
        /// </summary>
        /// <param name="filter">Entities filter</param>
        /// <exception cref="ArgumentException">Filter contains column not presented in Entity</exception>
        /// <returns>Pair of sql text and built sql arguments</returns>
        private (string, IReadOnlyDictionary<string, object>) GetFilterValue(FilterGroup filter)
        {
            var filterColumns = filter.GetFilterColumns();

            var notValidColumns = filterColumns.Except(EntityFields);

            if (notValidColumns.Any())
            {
                throw new ArgumentException($"Filter contains columns not presented in entity: {string.Join(", ", notValidColumns)}");
            }

            return FilterBuilder.Build(filter);
        }

        #endregion
    }
}
