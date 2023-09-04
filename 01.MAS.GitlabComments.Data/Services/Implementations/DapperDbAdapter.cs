namespace MAS.GitlabComments.DataAccess.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Dapper;

    /// <summary>
    /// Dapper database adapter
    /// </summary>
    public class DapperDbAdapter : IDbAdapter
    {
        /// <summary>
        /// Execute sql select query
        /// </summary>
        /// <param name="connection">Active database connection</param>
        /// <param name="sqlQuery">Sql query</param>
        /// <param name="arguments">Sql query arguments</param>
        /// <exception cref="ArgumentNullException">Parameter connection is null</exception>
        /// <exception cref="ArgumentNullException">Parameter sqlQuery is null</exception>
        /// <returns>Enumeration of selected rows mapped to specified class</returns>
        public IEnumerable<TEntity> Query<TEntity>(IDbConnection connection, string sqlQuery, object arguments = null)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (sqlQuery == null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            if (sqlQuery.Length == 0)
            {
                return Enumerable.Empty<TEntity>();
            }

            return arguments == null
                ? connection.Query<TEntity>(sqlQuery)
                : connection.Query<TEntity>(sqlQuery, arguments);
        }

        /// <summary>
        /// Execute sql command
        /// </summary>
        /// <param name="connection">Active database connection</param>
        /// <param name="sqlQuery">Sql command</param>
        /// <param name="arguments">Sql command arguments</param>
        /// <exception cref="ArgumentNullException">Parameter connection is null</exception>
        /// <exception cref="ArgumentNullException">Parameter sqlQuery is null</exception>
        /// <exception cref="ArgumentNullException">Parameter arguments is null</exception>
        /// <returns>Affected rows count if sql query provided; otherwise 0</returns>
        public int Execute(IDbConnection connection, string sqlQuery, object arguments)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (sqlQuery == null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            if (sqlQuery.Length == 0)
            {
                return 0;
            }

            return connection.Execute(sqlQuery, arguments);
        }
    }
}
