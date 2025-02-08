namespace MAS.GitlabComments.DataAccess.Select
{
    using System;

    using MAS.GitlabComments.DataAccess.Utilities;

    /// <summary>
    /// Information about table join
    /// </summary>
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class TableJoinData: IQueryPart
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        /// <summary>
        /// Source string configuration
        /// </summary>
        public string Configuration { get; }

        /// <summary>
        /// Left table name
        /// </summary>
        public string LeftTableName { get; set; }

        /// <summary>
        /// Name of relation column from left table
        /// </summary>
        public string LeftTableRelationColumn { get; init; }

        /// <summary>
        /// Right table name
        /// </summary>
        public string RightTableName { get; init; }

        /// <summary>
        /// Name of relation column from right table
        /// </summary>
        public string RightTableRelationColumn { get; init; }

        /// <summary>
        /// Tables join type
        /// </summary>
        public TableJoinType JoinType { get; init; }

        /// <summary>
        /// Left table alias
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Initializing <see cref="TableJoinData"/>
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <exception cref="ArgumentNullException">Parameter configuration is null</exception>
        public TableJoinData(string configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is TableJoinData compareTableJoinData)
            {
                return Configuration == compareTableJoinData.Configuration;
            }

            return base.Equals(obj);
        }

#if DEBUG
        /// <inheritdoc />
        public override string ToString()
        {
            return Configuration;
        }
#endif

        /// <inheritdoc />
        public string ToQuery(DatabaseType databaseType)
        {
            var joinType = JoinType.GetSqlOperator();

            if (JoinType == TableJoinType.None)
            {
                throw new ArgumentException("\"Join type\" must be set");
            }

            switch (databaseType)
            {
                case DatabaseType.MSSQL:
                    // EXAMPLE:
                    //
                    // LEFT OUTER JOIN [Table] AS [Alias] WITH(NOLOCK)
                    //      ON [Alias].[TableColumn] = [SourceTableAlias].[SourceTableColumn]

                    return $"{joinType} JOIN [{RightTableName}] AS [{Alias}] WITH(NOLOCK) ON ([{Alias}].[{RightTableRelationColumn}] = [{LeftTableName}].[{LeftTableRelationColumn}])";

                case DatabaseType.PGSQL:
                    // EXAMPLE:
                    //
                    // LEFT OUTER JOIN "Table" AS "Alias"
                    //      ON "Alias"."TableColumn" = "SourceTableAlias"."SourceTableColumn"

                    return $"{joinType} JOIN \"{RightTableName}\" AS \"{Alias}\" ON (\"{Alias}\".\"{RightTableRelationColumn}\" = \"{LeftTableName}\".\"{LeftTableRelationColumn}\")";

                default:
                    throw new NotImplementedException($"Handler for DB type \"{databaseType}\" not implemented yet.");
            }
        }
    }
}
