namespace MAS.GitlabComments.Data.Services
{
    using System.Data;

    /// <summary>
    /// Factory providing database connection
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Database query options
        /// </summary>
        DbConnectionQueryOptions QueryOptions { get; }

        /// <summary>
        /// Get DbConnection instance for current database
        /// </summary>
        /// <returns><inheritdoc cref="IDbConnection"/> instance</returns>
        IDbConnection CreateDbConnection();
    }

    /// <summary>
    /// Options for database connection
    /// </summary>
    public class DbConnectionQueryOptions
    {
        /// <summary>
        /// Maximum rows to select in single query
        /// </summary>
        public int MaxRowCount { get; set; }
    }
}
