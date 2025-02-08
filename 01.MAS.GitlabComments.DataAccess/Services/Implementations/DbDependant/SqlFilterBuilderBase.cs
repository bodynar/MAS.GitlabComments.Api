namespace MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.DataAccess.Utilities;

    /// <summary>
    /// Sql filter builder base
    /// </summary>
    public abstract class SqlFilterBuilderBase: IFilterBuilder
    {
        /// <summary> Filter sql parameter name template </summary>
        private const string FilterParamNameTemplate = "FilterValue{0}";

        /// <summary>
        /// Template for filter parameter name
        /// </summary>
        protected virtual string FilterParameterNameTemplate
            => FilterParamNameTemplate;

        /// <summary>
        /// Prefix for parameter name
        /// </summary>
        protected abstract string ParameterNamePrefix { get; }

        /// <inheritdoc />
        public FilterResult Build(FilterGroup queryFilterGroup)
        {
            if (queryFilterGroup == null || queryFilterGroup.IsEmpty)
            {
                throw new ArgumentNullException(nameof(queryFilterGroup));
            }

            var arguments = new Dictionary<string, object>();
            var resultSql = BuildWhereFilter(queryFilterGroup, arguments);

            return new FilterResult()
            {
                Sql = resultSql.Trim(),
                Values = arguments,
            };
        }

        #region Not public API

        /// <summary>
        /// Internal filter building
        /// </summary>
        /// <param name="filterGroup">Filter group</param>
        /// <param name="arguments">Sql argument dictionary</param>
        /// <returns>Sql text, if filter built properly; otherwise <see cref="string.Empty"/></returns>
        private string BuildWhereFilter(FilterGroup filterGroup, IDictionary<string, object> arguments)
        {
            return filterGroup.NestedGroups.Any()
                ? BuildNestedGroups(filterGroup, arguments)
                : BuildWhereFilterGroupFromFields(filterGroup, arguments);
        }

        /// <summary>
        /// Build nested filter groups
        /// </summary>
        /// <param name="filterGroup">Filter group</param>
        /// <param name="arguments">Sql argument dictionary</param>
        /// <returns>Sql text, if filter built properly; otherwise <see cref="string.Empty"/></returns>
        private string BuildNestedGroups(FilterGroup filterGroup, IDictionary<string, object> arguments)
        {
            var nestedSql = new StringBuilder();
            var innerFilters =
                filterGroup.NestedGroups
                    .Where(x => x.LogicalJoinType != FilterJoinType.None)
                    .OrderByDescending(x => x.LogicalJoinType)
                    .ToList();

            if (!innerFilters.Any())
            {
                return string.Empty;
            }

            var filterJointypeOperator = filterGroup.LogicalJoinType.GetSqlOperator();

            if (string.IsNullOrEmpty(filterJointypeOperator))
            {
                return string.Empty;
            }

            // TODO: for each group add new line?

            foreach (var filterGroupItem in innerFilters)
            {
                var sqlFilter = BuildWhereFilter(filterGroupItem, arguments);

                if (string.IsNullOrEmpty(sqlFilter))
                {
                    continue;
                }

                if (innerFilters.Count > 1)
                {
                    sqlFilter = $"({sqlFilter})";
                }

                var addition = nestedSql.Length > 0
                    ? $"{filterJointypeOperator} {sqlFilter} "
                    : $"{sqlFilter} ";

                nestedSql.Append(addition);
            }

            return nestedSql.ToString().TrimEnd();
        }

        /// <summary>
        /// Building sql filter with value comparisons
        /// </summary>
        /// <param name="filterGroup">Filter group</param>
        /// <param name="arguments">Sql argument dictionary</param>
        /// <returns>Sql text, if filter built properly; otherwise <see cref="string.Empty"/></returns>
        private string BuildWhereFilterGroupFromFields(FilterGroup filterGroup, IDictionary<string, object> arguments)
        {
            if (filterGroup.IsEmpty || filterGroup.NestedGroups.Any())
            {
                return string.Empty;
            }

            var joinOperator = filterGroup.LogicalJoinType.GetSqlOperator();

            if (string.IsNullOrEmpty(joinOperator))
            {
                return string.Empty;
            }

            var filterItems = filterGroup.Items.Where(x => x.LogicalComparisonType != ComparisonType.None);
            if (!filterItems.Any())
            {
                return string.Empty;
            }

            var conditions = new StringBuilder();

            var hasAlias = string.IsNullOrEmpty(filterGroup.TableAlias);
            var comparisonTypeDictionary = new Dictionary<ComparisonType, string>();

            foreach (var filterItem in filterItems)
            {
                var sqlOperator = comparisonTypeDictionary.ContainsKey(filterItem.LogicalComparisonType)
                    ? comparisonTypeDictionary[filterItem.LogicalComparisonType]
                    : filterItem.LogicalComparisonType.GetSqlOperator();

                if (string.IsNullOrWhiteSpace(sqlOperator))
                {
                    continue;
                }

                if (!comparisonTypeDictionary.ContainsKey(filterItem.LogicalComparisonType))
                {
                    comparisonTypeDictionary.Add(filterItem.LogicalComparisonType, sqlOperator);
                }

                var paramName = string.Format(
                    string.IsNullOrWhiteSpace(FilterParameterNameTemplate)
                        ? FilterParamNameTemplate
                        : FilterParameterNameTemplate,
                    arguments.Count
                );

                arguments.Add(paramName, filterItem.Value);

                var fieldName = GetFilterFieldName(filterItem.FieldName, filterGroup.TableAlias);

                var line = $"{fieldName} {comparisonTypeDictionary[filterItem.LogicalComparisonType]} {ParameterNamePrefix}{paramName} ";

                if (conditions.Length > 0)
                {
                    line = $"{joinOperator} {line}";
                }

                conditions.Append(line);
            }

            return conditions.ToString().TrimEnd();
        }

        /// <summary>
        /// Get filter field name
        /// </summary>
        /// <param name="fieldName">Column name</param>
        /// <param name="tableAlias">Alias for source table (can be null)</param>
        /// <returns>Field name for where filter</returns>
        protected abstract string GetFilterFieldName(string fieldName, string tableAlias);

        #endregion
    }
}
