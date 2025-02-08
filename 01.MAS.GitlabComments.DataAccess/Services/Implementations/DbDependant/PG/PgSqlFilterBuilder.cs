namespace MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.PG
{
    /// <summary>
    /// PostgreSql filter builder
    /// </summary>
    public class PgSqlFilterBuilder : SqlFilterBuilderBase
    {
        /// <inheritdoc />
        protected override string ParameterNamePrefix
            => "@";

        /// <inheritdoc />
        protected override string GetFilterFieldName(string fieldName, string tableAlias)
        {
            return string.IsNullOrWhiteSpace(tableAlias)
                ? $"\"{fieldName}\""
                : $"\"{tableAlias}\".\"{fieldName}\"";
        }
    }
}
