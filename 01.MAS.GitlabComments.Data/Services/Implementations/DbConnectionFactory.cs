namespace MAS.GitlabComments.DataAccess.Services.Implementations
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    /// <inheritdoc cref="IDbConnectionFactory"/>
    public class DbConnectionFactory : IDbConnectionFactory
    {
        /// <summary>
        /// Connection string to database
        /// </summary>
        private string ConnectionString { get; }

        /// <inheritdoc cref="IDbConnectionFactory.QueryOptions"/>
        public DbConnectionQueryOptions QueryOptions { get; }

        /// <summary>
        /// Initializing <see cref="DbConnectionFactory"/>
        /// </summary>
        /// <param name="connectionString">String with connection info to current database</param>
        /// <param name="queryOptions"></param>
        /// <exception cref="ArgumentNullException">Param connectionString is null</exception>
        /// <exception cref="ArgumentNullException">Param queryOptions is null</exception>
        public DbConnectionFactory(string connectionString, DbConnectionQueryOptions queryOptions)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            QueryOptions = queryOptions ?? throw new ArgumentNullException(nameof(queryOptions));
        }

        /// <inheritdoc cref="IDbConnectionFactory.CreateDbConnection"/>
        public IDbConnection CreateDbConnection()
            => new SqlConnection(ConnectionString);
    }
}
