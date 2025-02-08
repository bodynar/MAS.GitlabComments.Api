namespace MAS.GitlabComments.DataAccess.Services.Implementations
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using Npgsql;

    /// <inheritdoc cref="IDbConnectionFactory"/>
    public class DbConnectionFactory : IDbConnectionFactory
    {
        /// <summary>
        /// Connection string to database
        /// </summary>
        private string ConnectionString { get; }

        /// <inheritdoc />
        public DbConnectionQueryOptions QueryOptions { get; }

        /// <inheritdoc />
        public DatabaseType DatabaseType { get; }

        /// <summary>
        /// Initializing <see cref="DbConnectionFactory"/>
        /// </summary>
        /// <param name="connectionString">String with connection info to current database</param>
        /// <param name="queryOptions">Query options</param>
        /// <param name="databaseType">Type of db server</param>
        /// <exception cref="ArgumentNullException">Param connectionString is null</exception>
        /// <exception cref="ArgumentNullException">Param queryOptions is null</exception>
        public DbConnectionFactory(string connectionString, DbConnectionQueryOptions queryOptions, DatabaseType databaseType)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            QueryOptions = queryOptions ?? throw new ArgumentNullException(nameof(queryOptions));
            DatabaseType = databaseType;
        }

        /// <inheritdoc cref="IDbConnectionFactory.CreateDbConnection"/>
        public IDbConnection CreateDbConnection()
        {
            switch (DatabaseType)
            {
                case DatabaseType.MSSQL:
                    return new SqlConnection(ConnectionString);

                case DatabaseType.PGSQL:
                    return new NpgsqlConnection(ConnectionString);
                default:
                    throw new NotImplementedException($"Connection for \"{DatabaseType}\" not implemented yet");
            }
        }
    }
}
