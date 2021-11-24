namespace MAS.GitlabComments.Data.Services
{
    using System.Data;

    /// <summary>
    /// Factory providing database connection
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Get DbConnection instance for current database
        /// </summary>
        /// <returns><inheritdoc cref="IDbConnection"/> instance</returns>
        IDbConnection CreateDbConnection();
    }
}
