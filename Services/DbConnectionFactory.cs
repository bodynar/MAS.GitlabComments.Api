namespace MAS.GitlabComments.Services.Implementations
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private string ConnectionString { get; }

        public DbConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection CreateDbConnection()
            => new SqlConnection(ConnectionString);
    }
}
