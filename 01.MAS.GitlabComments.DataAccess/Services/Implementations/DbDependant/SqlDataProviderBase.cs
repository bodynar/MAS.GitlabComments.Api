namespace MAS.GitlabComments.DataAccess.Services.Implementations.DataProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Exceptions;
    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.DataAccess.Models;
    using MAS.GitlabComments.DataAccess.Select;

    /// <summary>
    /// Base class for implementations of <see cref="IDataProvider{TEntity}"/> in specific SQL implementation
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    public abstract class SqlDataProviderBase<TEntity> : IDataProvider<TEntity>
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
        /// List of default entity fields which cannot be set manually
        /// </summary>
        private static IEnumerable<string> DefaultEntityFields { get; }
            = typeof(BaseEntity).GetProperties().Select(x => x.Name);

        /// <summary>
        /// List of entity fields
        /// </summary>
        private static IEnumerable<string> EntityFields { get; }
            = GetEntityFields();

        #region Abstract members

        /// <inheritdoc />
        public abstract DatabaseType DbType { get; }

        /// <summary>
        /// Template for query parameter name, with 1 string parameter
        /// </summary>
        protected abstract string QueryParameterNameTemplate { get; }

        /// <summary>
        /// Prefix for query parameter
        /// </summary>
        protected abstract string QueryParameterPrefix { get; }

        #endregion

        /// <summary>
        /// Initializing <see cref="SqlDataProviderBase{TEntity}"/>
        /// </summary>
        /// <param name="dbConnectionFactory">Factory providing database connection</param>
        /// <param name="dbAdapter">Database operation adapter</param>
        /// <param name="filterBuilder">Sql filter builder</param>
        /// <param name="complexColumnBuilder">Complex column query builder</param>
        /// <exception cref="ArgumentNullException">Parameter dbConnectionFactory is null</exception>
        /// <exception cref="ArgumentNullException">Parameter dbAdapter is null</exception>
        /// <exception cref="ArgumentNullException">Parameter filterBuilder is null</exception>
        /// <exception cref="ArgumentNullException">Parameter complexColumnBuilder is null</exception>
        public SqlDataProviderBase(
            IDbConnectionFactory dbConnectionFactory, IDbAdapter dbAdapter,
            IFilterBuilder filterBuilder, IComplexColumnQueryBuilder complexColumnBuilder
        )
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
                            ParameterName = QueryParameterPrefix + string.Format(QueryParameterNameTemplate, i + 1),
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
                    ParameterName = QueryParameterPrefix + string.Format(QueryParameterNameTemplate, map.Count + 1),
                });
            }

            map.Set(nameof(BaseEntity.CreatedOn), DateTime.UtcNow, false);
            map.Set(nameof(BaseEntity.ModifiedOn), DateTime.UtcNow);

            var query = BuildInsertQuery(TableName, map);

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
        /// Build insert query in specific sql implementation
        /// </summary>
        /// <param name="tableName">Name of current table</param>
        /// <param name="arguments">Command arguments</param>
        /// <returns>Sql query to insert data for specific table</returns>
        protected abstract string BuildInsertQuery(string tableName, IReadOnlyDictionary<string, QueryParameter> arguments);

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>All entities</returns>
        public IEnumerable<TEntity> Get()
        {
            var entities = Enumerable.Empty<TEntity>();

            var sqlQuery = BuildSelectQuery(TableName);
            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                entities = DbAdapter.Query<TEntity>(connection, sqlQuery).ToList();
            }

            return entities;
        }

        /// <summary>
        /// Build select query in specific sql implementation
        /// </summary>
        /// <param name="tableName">Name of current table</param>
        /// <returns>Sql query to select all columns without any</returns>
        protected abstract string BuildSelectQuery(string tableName);

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

            string parameterName = string.Format(QueryParameterNameTemplate, 1);

            TEntity entity = null;
            var sqlQuery = BuildSelectSingleItemQuery(TableName, QueryParameterPrefix + parameterName);

            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                entity = DbAdapter.Query<TEntity>(
                    connection,
                    sqlQuery,
                    new Dictionary<string, object>() {
                        { parameterName, entityId }
                    })
                .FirstOrDefault();
            }

            return entity;
        }

        /// <summary>
        /// Build select query in specific sql implementation for selecting single item by its id column
        /// </summary>
        /// <param name="tableName">Name of current table</param>
        /// <param name="parameterName">Id parameter name</param>
        /// <returns>Sql query to select all columns without any</returns>
        protected abstract string BuildSelectSingleItemQuery(string tableName, string parameterName);

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

            var parameters = newValues
                .Where(x => EntityFields.Contains(x.Key) && x.Value != null) // TODO: How about setting null to value?
                .Select((x, i) =>
                    new QueryParameter
                    {
                        ColumnName = x.Key,
                        Value = x.Value,
                        ParameterName = QueryParameterPrefix + string.Format(QueryParameterNameTemplate, i + 1),
                    }
                );

            if (!parameters.Any())
            {
                throw new QueryExecutionException(
                    QueryExecutionExceptionState.Before,
                    $"Operation cannot be performed due to empty entity values dictionary"
                );
            }

            var map = parameters.ToDictionary(x => x.ColumnName);

            map.Set(nameof(BaseEntity.ModifiedOn), DateTime.UtcNow);
            map.Set(nameof(BaseEntity.Id), entityId);

            var query = BuildUpdateQuery(TableName, map);

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
                    "Update command performed with empty result, no record was updates"
                );
            }
        }

        /// <summary>
        /// Build update query in specific sql implementation
        /// </summary>
        /// <param name="tableName">Name of current table</param>
        /// <param name="arguments">Command arguments</param>
        /// <returns>Sql query to update data for specific table</returns>
        protected abstract string BuildUpdateQuery(string tableName, IReadOnlyDictionary<string, QueryParameter> arguments);

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

            if (!entityIds.Any())
            {
                return;
            }

            string parameterName = string.Format(QueryParameterNameTemplate, 1);
            var sqlQuery = BuildDeleteQueryById(TableName, QueryParameterPrefix + parameterName);

            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                DbAdapter.Execute(
                    connection,
                    sqlQuery,
                    new Dictionary<string, object>() {
                        { parameterName, entityIds }
                    }
                );
            }
        }

        /// <summary>
        /// Build delete query
        /// </summary>
        /// <param name="tableName">Name of current table</param>
        /// <param name="parameterName">Ids array parameter name</param>
        /// <returns>Sql query to remove records by ids</returns>
        protected abstract string BuildDeleteQueryById(string tableName, string parameterName);

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

            var filterResult = GetFilterValue(filter);

            if (filterResult == default || string.IsNullOrEmpty(filterResult.Sql))
            {
                return Get();
            }

            IEnumerable<TEntity> entities = Enumerable.Empty<TEntity>();
            var sqlQuery = BuildSelectQueryWithFilter(TableName, filterResult.Sql);

            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                entities = DbAdapter.Query<TEntity>(connection, sqlQuery, filterResult.Values).ToList();
            }

            return entities;
        }

        /// <summary>
        /// Build select query with filter in specific sql implementation
        /// </summary>
        /// <param name="tableName">Name of current table</param>
        /// <param name="filter">Filter sql part</param>
        /// <returns>Sql query to select all columns with specific filter</returns>
        protected abstract string BuildSelectQueryWithFilter(string tableName, string filter);

        /// <summary>
        /// Select entities into custom model
        /// </summary>
        /// <typeparam name="TProjection">Projection model type</typeparam>
        /// <param name="configuration">Select configuration</param>
        /// <exception cref="ArgumentNullException">Parameter configuration is null</exception>
        /// <exception cref="ArgumentException">Filter contains column not presented in Entity</exception>
        /// <returns>Entities mapped to specified model</returns>
        public IEnumerable<TProjection> Select<TProjection>(SelectConfiguration configuration = null)
        {
            var complexColumnData = ComplexColumnQueryBuilder.BuildComplexColumns<TProjection>(TableName);
            string builtFilter = null;
            IReadOnlyDictionary<string, object> queryArguments = new Dictionary<string, object>();

            if (configuration != null && configuration.Filter != null && !configuration.Filter.IsEmpty)
            {
                var builtFilterResult = GetFilterValue(configuration.Filter);

                if (builtFilterResult != null && !string.IsNullOrWhiteSpace(builtFilterResult.Sql))
                {
                    queryArguments = builtFilterResult.Values;
                    builtFilter = builtFilterResult.Sql;
                }
            }

            var entities = Enumerable.Empty<TProjection>();
            var sqlQuery = BuildSelectQueryWithJoins(TableName, complexColumnData, builtFilter);

            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                entities = DbAdapter.Query<TProjection>(connection, sqlQuery, queryArguments).ToList();
            }

            return entities;
        }

        /// <summary>
        /// Build select query with filter in specific sql implementation
        /// </summary>
        /// <param name="tableName">Name of current table</param>
        /// <param name="complexColumnData">Information about complex columns (joins)</param>
        /// <param name="filterBuiltResult">Built query filter part</param>
        /// <returns>Sql query to select all columns with specific filter</returns>
        protected abstract string BuildSelectQueryWithJoins(string tableName, ComplexColumnData complexColumnData, string filterBuiltResult);

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
        /// Building sql filter from filter configuration
        /// </summary>
        /// <param name="filter">Entities filter</param>
        /// <exception cref="ArgumentException">Filter contains column not presented in Entity</exception>
        /// <returns>Built filter data</returns>
        private FilterResult GetFilterValue(FilterGroup filter)
        {
            var filterColumns = filter.GetFilterColumns();

            var notValidColumns = filterColumns
                .Except(EntityFields)
                .Except(DefaultEntityFields);

            if (notValidColumns.Any())
            {
                throw new ArgumentException($"Filter contains columns not presented in entity: {string.Join(", ", notValidColumns)}");
            }

            return FilterBuilder.Build(filter);
        }

        #endregion
    }
}
