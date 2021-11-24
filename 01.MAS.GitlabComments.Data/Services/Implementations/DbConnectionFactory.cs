namespace MAS.GitlabComments.Data.Services.Implementations
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

        /// <summary>
        /// Initializing <see cref="DbConnectionFactory"/>
        /// </summary>
        /// <param name="connectionString">String with connection info to current database</param>
        /// <exception cref="ArgumentNullException">Param connectionString is null</exception>
        public DbConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <inheritdoc cref="IDbConnectionFactory.CreateDbConnection"/>
        public IDbConnection CreateDbConnection()
            => new SqlConnection(ConnectionString);
    }
}
