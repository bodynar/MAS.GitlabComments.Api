namespace MAS.GitlabComments.DataAccess.Select
{
    using System;

    /// <summary>
    /// Column with complex path data
    /// </summary>
    public record ComplexColumn: IQueryPart
    {
        /// <summary>
        /// Source table alias
        /// </summary>
        public string TableAlias { get; init; }

        /// <summary>
        /// Column name in table
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Column alias (name of property to project)
        /// </summary>
        public string Alias { get; init; }

        /// <inheritdoc />
        public string ToQuery(DatabaseType databaseType)
        {
            if (string.IsNullOrEmpty(TableAlias) || string.IsNullOrEmpty(Name))
            {
                throw new ArgumentException($"Required string data is empty: TableAlias = \"{TableAlias}\", Name = \"{Name}\"");
            }

            switch (databaseType)
            {
                case DatabaseType.MSSQL:
                    return string.IsNullOrEmpty(Alias) || Alias == Name
                        ? $"[{TableAlias}].[{Name}]"
                        : $"[{TableAlias}].[{Name}] AS [{Alias}]";

                case DatabaseType.PGSQL:
                    return string.IsNullOrEmpty(Alias) || Alias == Name
                        ? $"\"{TableAlias}\".\"{Name}\""
                        : $"\"{TableAlias}\".\"{Name}\" AS \"{Alias}\"";

                default:
                    throw new NotImplementedException($"Handler for DB type \"{databaseType}\" not implemented yet.");
            }
        }
    }
}
