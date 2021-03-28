namespace MAS.GitlabComments.Services
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    using Dapper;

    /// <summary>
    /// Provider of data for specified entity type
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    public class DapperDataProvider<TEntity> : IDataProvider<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Factory providing database connection
        /// </summary>
        private IDbConnectionFactory DbConnectionFactory { get; }

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
        private static IEnumerable<string> DefaultEntityFields { get; } = new[] { "Id", "CreatedOn", "ModifiedOn" };

        /// <summary>
        /// Initializing <see cref="DataProvider"/>
        /// </summary>
        /// <param name="dbConnectionFactory">Factory providing database connection</param>
        /// <exception cref="ArgumentNullException">Parameter dbConnectionFactory is null</exception>
        public DapperDataProvider(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(DbConnectionFactory));
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
                var value = field.GetValue(entity);

                if (value != default)
                {
                    var parameterName = $"@P{setStatements.Count + 1}";

                    if (arguments.TryAdd(parameterName, value))
                    {
                        setStatements.Add(new KeyValuePair<string, string>(field.Name, parameterName));
                    }
                }
            }

            if (setStatements.Any())
            {
                var parameterName = $"@P{setStatements.Count + 1}";
                if (arguments.TryAdd(parameterName, Guid.NewGuid()))
                {
                    setStatements.Add(new KeyValuePair<string, string>("Id", parameterName));
                }
                else
                {
                    throw new Exception("Cannot build arguments for sql command: [Id].");
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

                sqlQuery += $"({string.Join(", ", fieldNames)}) VALUES ({string.Join(", ", fieldNames)})";
                int affectedRows = 0;

                using (var connection = DbConnectionFactory.CreateDbConnection())
                {
                    affectedRows = connection.Execute(sqlQuery, arguments);
                }

                if (affectedRows == 0)
                {
                    throw new Exception("Insert command performed with empty result.");
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
                entities = connection.Query<TEntity>(sqlQuery).ToList();
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
            TEntity entity = null;
            var sqlQuery = $"SELECT * FROM [{TableName}] WHERE Id = @P1";

            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                entity = connection.Query<TEntity>(sqlQuery, new { P1 = entityId }).FirstOrDefault();
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

            foreach (var value in newValues)
            {
                if (!string.IsNullOrEmpty(value.Key)
                    && value.Value != default
                    && EntityFields.Contains(value.Key)
                    && !DefaultEntityFields.Contains(value.Key))
                {
                    var parameterName = $"P{argumentsCount + 1}";
                    if (arguments.TryAdd(parameterName, value.Value))
                    {
                        setStatements.Add($"[{value.Key}] = @{parameterName}");
                        argumentsCount++;
                    }
                }
            }

            if (setStatements.Any())
            {
                var parameterName = $"P{argumentsCount + 1}";
                if (arguments.TryAdd(parameterName, DateTime.UtcNow))
                {
                    setStatements.Add($"[ModifiedOn] = @{parameterName}");
                    argumentsCount++;
                }

                var setStatement = string.Join(", ", setStatements);

                if (arguments.TryAdd($"P{++argumentsCount}", entityId))
                {
                    var sqlQuery = $"UPDATE [{TableName}] {setStatement} WHERE Id = @P{argumentsCount}";

                    using (var connection = DbConnectionFactory.CreateDbConnection())
                    {
                        connection.Execute(sqlQuery, arguments);
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
                    connection.Execute(sqlQuery, new { P1 = entityIds });
                }
            }
        }

        #region Not public API

        /// <summary>
        /// Get entity field names
        /// </summary>
        /// <returns>List of entity field names</returns>
        private static IEnumerable<string> GetEntityFields()
        {
            return typeof(TEntity).GetProperties(System.Reflection.BindingFlags.Public).Select(x => x.Name);
        }

        #endregion
    }
}
