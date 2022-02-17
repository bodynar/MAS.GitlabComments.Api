namespace MAS.GitlabComments.Data.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    using MAS.GitlabComments.Data.Filter;
    using MAS.GitlabComments.Data.Models;
    using MAS.GitlabComments.Data.Select;

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
        private static IEnumerable<string> DefaultEntityFields { get; } = new[] { "CreatedOn", "ModifiedOn" };

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
        /// <exception cref="ArgumentNullException">Parameter entity is null</exception>
        /// <exception cref="Exception">No records was added</exception>
        public void Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var sqlQuery = $"INSERT INTO [{TableName}]";

            var setStatements = new List<KeyValuePair<string, string>>();
            var arguments = new ExpandoObject();

            var fields = entity.GetType().GetProperties().Where(x => !DefaultEntityFields.Contains(x.Name));

            foreach (var field in fields)
            {
                var boxedValue = field.GetValue(entity);
                var isDefault = IsDefaultValue(boxedValue, field.PropertyType);

                if (!isDefault)
                {
                    var parameterName = $"@P{setStatements.Count + 1}";

                    if (arguments.TryAdd(parameterName, boxedValue))
                    {
                        setStatements.Add(new KeyValuePair<string, string>(field.Name, parameterName));
                    }
                }
            }

            if (setStatements.Any())
            {
                string parameterName;

                if (!setStatements.Any(x => x.Key == "Id"))
                {
                    parameterName = $"@P{setStatements.Count + 1}";
                    if (arguments.TryAdd(parameterName, Guid.NewGuid()))
                    {
                        setStatements.Add(new KeyValuePair<string, string>("Id", parameterName));
                    }
                    else
                    {
                        throw new Exception("Cannot build arguments for sql command: [Id].");
                    }
                }

                parameterName = $"@P{setStatements.Count + 1}";
                if (arguments.TryAdd(parameterName, DateTime.UtcNow))
                {
                    setStatements.Add(new KeyValuePair<string, string>("CreatedOn", parameterName));
                }
                else
                {
                    throw new Exception("Cannot build arguments for sql command: [CreatedOn].");
                }

                var fieldNames = setStatements.Select(x => $"[{x.Key}]");
                var parameterNames = setStatements.Select(x => x.Value);

                sqlQuery += $" ({string.Join(", ", fieldNames)}) VALUES ({string.Join(", ", parameterNames)})";
                int affectedRows = 0;

                using (var connection = DbConnectionFactory.CreateDbConnection())
                {
                    affectedRows = DbAdapter.Execute(connection, sqlQuery, arguments);
                }

                if (affectedRows == 0)
                {
                    throw new Exception("Insert command performed with empty result, no record was added.");
                }
            }
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>All entities</returns>
        public IEnumerable<TEntity> Get()
        {
            IEnumerable<TEntity> entities = Enumerable.Empty<TEntity>();

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
        public TEntity Get(Guid entityId)
        {
            // TODO: add param check
            TEntity entity = null;
            var sqlQuery = $"SELECT * FROM [{TableName}] WHERE Id = @P1";

            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                entity = DbAdapter.Query<TEntity>(connection, sqlQuery, new { P1 = entityId }).FirstOrDefault();
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
            var arguments = new ExpandoObject();
            var argumentsCount = 0;

            foreach (var pair in newValues.Where(x => !string.IsNullOrEmpty(x.Key) && x.Value != null))
            {
                var isDefaultValue = IsDefaultValue(pair.Value, pair.Value.GetType());

                if (!isDefaultValue
                    && EntityFields.Contains(pair.Key)
                    && !DefaultEntityFields.Contains(pair.Key))
                {
                    var parameterName = $"@P{argumentsCount + 1}";
                    if (arguments.TryAdd(parameterName, pair.Value))
                    {
                        setStatements.Add($"[{pair.Key}] = {parameterName}");
                        argumentsCount++;
                    }
                }
            }

            if (setStatements.Any())
            {
                var parameterName = $"@P{argumentsCount + 1}";
                if (arguments.TryAdd(parameterName, DateTime.UtcNow))
                {
                    setStatements.Add($"[ModifiedOn] = {parameterName}");
                    argumentsCount++;
                }

                var setStatement = string.Join(", ", setStatements);

                if (arguments.TryAdd($"@P{++argumentsCount}", entityId))
                {
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
                    DbAdapter.Execute(connection, sqlQuery, new { P1 = entityIds });
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

            var entityFields = GetEntityFields();
            var filterColumns = filter.GetFilterColumns();

            var notValidColumns = filterColumns.Except(entityFields);

            if (notValidColumns.Any())
            {
                throw new ArgumentException($"Filter contains columns not presented in entity: {string.Join(", ", notValidColumns)}");
            }

            var (filterSql, filterArgs) = FilterBuilder.Build(filter);

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

        #region Not public API

        /// <summary>
        /// Get entity field names
        /// </summary>
        /// <returns>List of entity field names</returns>
        private static IEnumerable<string> GetEntityFields()
        {
            return typeof(TEntity).GetProperties().Select(x => x.Name);
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

        #endregion
    }
}
