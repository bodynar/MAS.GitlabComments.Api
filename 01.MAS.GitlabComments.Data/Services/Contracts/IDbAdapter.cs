namespace MAS.GitlabComments.Data.Services
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Database adapter providing extra layer data-mapping between application and database
    /// </summary>
    public interface IDbAdapter
    {
        /// <summary>
        /// Execute sql select query
        /// </summary>
        /// <param name="connection">Active database connection</param>
        /// <param name="sqlQuery">Sql query</param>
        /// <param name="arguments">Sql query arguments</param>
        /// <returns>Enumeration of selected rows mapped to specified class</returns>
        IEnumerable<TEntity> Query<TEntity>(IDbConnection connection, string sqlQuery, object arguments = null);

        /// <summary>
        /// Execute sql command
        /// </summary>
        /// <param name="connection">Active database connection</param>
        /// <param name="sqlQuery">Sql command</param>
        /// <param name="arguments">Sql command arguments</param>
        /// <returns>Affected rows count</returns>
        int Execute(IDbConnection connection, string sqlQuery, object arguments);
    }
}
