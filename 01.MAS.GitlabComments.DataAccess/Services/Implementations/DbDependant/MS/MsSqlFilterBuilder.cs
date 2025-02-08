namespace MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.MS
{
    using MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant;

    /// <summary>
    /// MsSql filter builder
    /// </summary>
    public class MsSqlFilterBuilder : SqlFilterBuilderBase
    {
        /// <inheritdoc />
        protected override string ParameterNamePrefix
            => "@";

        /// <inheritdoc />
        protected override string GetFilterFieldName(string fieldName, string tableAlias)
        {
            return string.IsNullOrWhiteSpace(tableAlias)
                ? $"[{fieldName}]"
                : $"[{tableAlias}].[{fieldName}]";
        }
    }
}
