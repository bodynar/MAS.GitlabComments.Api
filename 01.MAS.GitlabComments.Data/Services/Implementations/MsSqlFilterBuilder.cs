namespace MAS.GitlabComments.DataAccess.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.DataAccess.Utilities;

    /// <summary>
    /// MsSql filter builder
    /// </summary>
    public class MsSqlFilterBuilder : IFilterBuilder
    {
        /// <summary> Filter sql parameter name template </summary>
        private const string FilterParamNameTemplate = "FilterValue{0}";

        /// <inheritdoc cref="IFilterBuilder.Build(FilterGroup)"/>
        public (string sqlCondition, IReadOnlyDictionary<string, object> sqlArguments) Build(FilterGroup queryFilterGroup)
        {
            if (queryFilterGroup == null || queryFilterGroup.IsEmpty)
            {
                throw new ArgumentNullException(nameof(queryFilterGroup));
            }

            var arguments = new Dictionary<string, object>();
            var resultSql = BuildWhereFilter(queryFilterGroup, arguments);

            return (resultSql.Trim(), arguments);
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
            // TODO: If has items & groups => build groups & attach build from items

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
                    ? $"{filterJointypeOperator} {sqlFilter}"
                    : $"{sqlFilter}";

                nestedSql.AppendLine(addition);
            }

            return nestedSql.ToString().TrimEnd();
        }

        /// <summary>
        /// Building sql filter with value comparisons
        /// </summary>
        /// <param name="filterGroup">Filter group</param>
        /// <param name="arguments">Sql argument dictionary</param>
        /// <exception cref="ArgumentException">Method called for filter group with nested groups</exception>
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

            var conditions = new List<string>(); // TODO: To StringBuilder?

            foreach (var filter in filterItems)
            {
                var comparisonOperator = filter.LogicalComparisonType.GetSqlOperator();

                if (string.IsNullOrEmpty(comparisonOperator))
                {
                    continue;
                }

                var paramName = string.Format(FilterParamNameTemplate, arguments.Count);

                if (arguments.TryAdd(paramName, filter.Value))
                {
                    var fieldName = string.IsNullOrEmpty(filterGroup.TableAlias)
                        ? $"[{filter.FieldName}]"
                        : $"[{filterGroup.TableAlias}].[{filter.FieldName}]";

                    conditions.Add($"{fieldName} {comparisonOperator} @{paramName}");
                }
                else
                {
                    // TODO: log somehow
                }
            }

            return string.Join($"{Environment.NewLine}{joinOperator} ", conditions);
        }

        #endregion
    }
}
