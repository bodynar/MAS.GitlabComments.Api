namespace MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.MS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Models;
    using MAS.GitlabComments.DataAccess.Select;
    using MAS.GitlabComments.DataAccess.Services.Implementations.DataProvider;

    /// <summary>
    /// Provider of data for specified entity type
    /// <para>For MSSQL Database</para>
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    public class MsSqlDataProvider<TEntity> : SqlDataProviderBase<TEntity>
        where TEntity : BaseEntity
    {
        /// <inheritdoc />
        public override DatabaseType DbType
            => DatabaseType.MSSQL;

        /// <inheritdoc />
        protected override string QueryParameterNameTemplate
            => "P{0}";

        /// <inheritdoc />
        protected override string QueryParameterPrefix
            => "@";

        /// <summary>
        /// Initializing <see cref="MsSqlDataProvider{TEntity}"/>
        /// </summary>
        /// <param name="dbConnectionFactory">Factory providing database connection</param>
        /// <param name="dbAdapter">Database operation adapter</param>
        /// <param name="filterBuilder">Sql filter builder</param>
        /// <param name="complexColumnBuilder">Complex column query builder</param>
        /// <exception cref="ArgumentNullException">Parameter dbConnectionFactory is null</exception>
        /// <exception cref="ArgumentNullException">Parameter dbAdapter is null</exception>
        /// <exception cref="ArgumentNullException">Parameter filterBuilder is null</exception>
        /// <exception cref="ArgumentNullException">Parameter complexColumnBuilder is null</exception>
        public MsSqlDataProvider(
            IDbConnectionFactory dbConnectionFactory, IDbAdapter dbAdapter,
            IFilterBuilder filterBuilder, IComplexColumnQueryBuilder complexColumnBuilder
        ) : base(dbConnectionFactory, dbAdapter, filterBuilder, complexColumnBuilder)
        {
        }

        /// <inheritdoc />
        protected override string BuildInsertQuery(string tableName, IReadOnlyDictionary<string, QueryParameter> arguments)
        {
            var query = $"INSERT INTO [{tableName}]" +
                $" ({string.Join(", ", arguments.Keys.Select(x => $"[{x}]"))})" +
                $" VALUES ({string.Join(", ", arguments.Values.Select(x => x.ParameterName))})"
            ;

            return query;
        }

        /// <inheritdoc />
        protected override string BuildSelectQuery(string tableName)
        {
            return $"SELECT * FROM [{tableName}]";
        }

        /// <inheritdoc />
        protected override string BuildSelectSingleItemQuery(string tableName, string parameterName)
        {
            return $"SELECT * FROM [{tableName}] WHERE [Id] = {parameterName}";
        }

        /// <inheritdoc />
        protected override string BuildUpdateQuery(string tableName, IReadOnlyDictionary<string, QueryParameter> arguments)
        {
            var setStatements = arguments
                .ToList()
                .Where(x => x.Key != nameof(BaseEntity.Id))
                .Select(x => $"[{x.Key}] = {x.Value.ParameterName}")
                .ToList();

            var query = $"UPDATE [{tableName}]" +
                $" SET {string.Join(", ", setStatements)}" +
                $" WHERE [Id] = {arguments[nameof(BaseEntity.Id)].ParameterName}"
            ;

            return query;
        }

        /// <inheritdoc />
        protected override string BuildDeleteQueryById(string tableName, string parameterName)
        {
            return $"DELETE FROM [{tableName}] WHERE [Id] IN {parameterName}";
        }

        /// <inheritdoc />
        protected override string BuildSelectQueryWithFilter(string tableName, string filter)
        {
            return $"SELECT * FROM [{tableName}] WHERE {filter}";
        }

        /// <inheritdoc />
        protected override string BuildSelectQueryWithJoins(string tableName, ComplexColumnData complexColumnData, string filterBuiltResult)
        {
            var columns = "*";
            var joinPart = "";

            if (complexColumnData != null)
            {
                columns = string.Join($", ", complexColumnData.Columns.Select(column => column.ToQuery(DbType)));

                joinPart = string.Join(" ", complexColumnData.Joins.Select(joinData => joinData.ToQuery(DbType)));
            }

            var filter = string.IsNullOrWhiteSpace(filterBuiltResult)
                ? string.Empty
                : $"WHERE {filterBuiltResult}";

            return $"SELECT {columns} FROM [{tableName}] {joinPart} {filter}".TrimEnd(' ');
        }
    }
}
