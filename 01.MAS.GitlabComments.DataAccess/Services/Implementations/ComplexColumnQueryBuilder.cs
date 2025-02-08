namespace MAS.GitlabComments.DataAccess.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using MAS.GitlabComments.DataAccess.Attributes;
    using MAS.GitlabComments.DataAccess.Select;

    /// <summary>
    /// Implementation of <see cref="IComplexColumnQueryBuilder"/>
    /// </summary>
    public class ComplexColumnQueryBuilder : IComplexColumnQueryBuilder
    {
        /// <summary> Regular expression for table join configuration expression template </summary>
        private const string TableColumnPathPattern = @"\[([^\:]*)\:([^\:]*)\:([^\]]*)\]";

        /// <summary> Regular expression for pascal case text </summary>
        private const string PascalCasePathPattern = @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])";

        /// <summary> Instance of <see cref="Regex"/> for table join configuration </summary>
        private static Regex TableColumnPathRegex { get; }
            = new Regex(TableColumnPathPattern);

        /// <summary> Instance of <see cref="Regex"/> for pascal case text </summary>
        private static Regex PascalCaseRegex { get; }
            = new Regex(PascalCasePathPattern);

        /// <summary> Cached built complex column configurations </summary>
        private static Dictionary<KeyValuePair<string, Type>, ComplexColumnData> CachedComplexColumnConfigs { get; }
            = new Dictionary<KeyValuePair<string, Type>, ComplexColumnData>();

        /// <summary>
        /// Initializing <see cref="ComplexColumnQueryBuilder"/>
        /// </summary>
        public ComplexColumnQueryBuilder() { }

        /// <inheritdoc cref="IComplexColumnQueryBuilder.BuildComplexColumns{TProjection}"/>
        public ComplexColumnData BuildComplexColumns<TProjection>(string sourceTableName)
        {
            if (string.IsNullOrWhiteSpace(sourceTableName))
            {
                return null;
            }

            var projectionType = typeof(TProjection);
            var cacheKey = new KeyValuePair<string, Type>(sourceTableName, projectionType);

            if (CachedComplexColumnConfigs.ContainsKey(cacheKey))
            {
                return CachedComplexColumnConfigs[cacheKey];
            }

            var columns = projectionType.GetProperties();

            if (!columns.Any())
            {
                return null;
            }

            var columnAttributeMap = columns.Select(x =>
                new Tuple<ComplexColumnPathAttribute, PropertyInfo>(
                    x.GetCustomAttributes(typeof(ComplexColumnPathAttribute), false).FirstOrDefault() as ComplexColumnPathAttribute,
                    x
                )
            );

            var tableJoinDataItems = new List<TableJoinData>();
            var columnsConfiguration =
                columnAttributeMap
                    .Where(x => x.Item1 == null)
                    .Select(x =>
                        new ComplexColumn
                        {
                            TableAlias = sourceTableName,
                            Name = x.Item2.Name,
                        }
                    )
                    .ToList();

            var complexColumns = columnAttributeMap.Where(x => x.Item1 != null);

            foreach (var complexColumn in complexColumns)
            {
                var column = BuildComplexColumnPathParams(tableJoinDataItems, complexColumn.Item1, sourceTableName, complexColumn.Item2.Name);

                if (column == null)
                {
                    continue;
                }

                columnsConfiguration.Add(column);
            }

            /*
             * TODO: (next release)
             *  1. When path contains same 2nd+ join, but different previous - make another join data
             *      1.1. Update test ShouldBuildDataWithSeveralJoinDataInSeveralAttributeWithSameTableButDifferentJoinParams
             *  2. Validate result of ComplexColumnPathQueryBuilder before returning
             */

            var columnData = new ComplexColumnData
            {
                Columns = columnsConfiguration,
                Joins = tableJoinDataItems
            };

            CachedComplexColumnConfigs.Add(cacheKey, columnData);

            return columnData;
        }

        /// <summary>
        /// Building complex column path parameters from attribute
        /// </summary>
        /// <param name="existedJoins">Defined join data items</param>
        /// <param name="complexColumnPathAttribute">Column complex path attribute</param>
        /// <param name="sourceTableName">Name of source table</param>
        /// <param name="alias">Alias for column</param>
        /// <returns>Column configuration if data successfully mapped; otherwise - <see langword="null"/> values</returns>
        private static ComplexColumn BuildComplexColumnPathParams(
            List<TableJoinData> existedJoins,
            ComplexColumnPathAttribute complexColumnPathAttribute,
            string sourceTableName,
            string alias
        )
        {
            /** 
                1. [Table1:Table1Column:Table2Column].Value
                    => ([
                            { LT: Table1, LTC: Table1Column, RT: {CurrentTable}, RTC: Table2Column, Alias: Table11 },
                        ],
                        CP: Table11.Value
                    )

                2. [Table1:Table1Column:Table2Column].[Table3:Table3Column:Table1Column].Value
                    => ([
                            { LT: Table1, LTC: Table1Column, RT: {CurrentTable}, RTC: Table2Column, Alias: Table11 },
                            { LT: Table3, LTC: Table3Column, RT: Table11, RTC: Table3Column, Alias: Table31 }
                        ],
                        CP: Table31.Value
                    )
             */

            var parts = complexColumnPathAttribute.ColumnPath.Split('.');

            if (parts.Length <= 1)
            {
                return null;
            }

            var lastPart = parts.Last();
            var firstPart = parts[0];

            if (TableColumnPathRegex.IsMatch(lastPart) || !TableColumnPathRegex.IsMatch(firstPart))
            {
                // invalid path: [Table1:Table1Column:Table2Column].[Table1:Table1Column:Table2Column]
                // invalid path: SomeColumn.[Table1:Table1Column:Table2Column]

                return null;
            }

            var tableJoinDataItems = new List<TableJoinData>();
            var previousLeftTableName = sourceTableName;

            foreach (var part in parts.Take(parts.Length - 1))
            {
                var sameJoinData = existedJoins.FirstOrDefault(x => x.Configuration == part);

                if (sameJoinData != null)
                {
                    previousLeftTableName = sameJoinData.Alias;
                    continue;
                }

                var tableJoinData = GetTableJoinData(part);
                if (tableJoinData == default)
                {
                    continue;
                }

                tableJoinData.LeftTableName = previousLeftTableName;

                var tableNameParts = PascalCaseRegex.Split(tableJoinData.RightTableName);

                if (tableNameParts == default || !tableNameParts.Any())
                {
                    continue;
                }

                var abbriveation = string.Join(
                    "", tableNameParts.Select(x => 
                        string.Join("", x.Take(3))
                    )
                );

                var sameTablesCount =
                    existedJoins.Count(x => x.RightTableName == tableJoinData.RightTableName)
                    + tableJoinDataItems.Count(x => x.RightTableName == tableJoinData.RightTableName);

                tableJoinData.Alias = previousLeftTableName = $"{abbriveation}{sameTablesCount + 1}";

                tableJoinDataItems.Add(tableJoinData);
            }

            existedJoins.AddRange(tableJoinDataItems);

            return new ComplexColumn
            {
                TableAlias = previousLeftTableName,
                Name = lastPart,
                Alias = alias,
            };
        }

        /// <summary>
        /// Get table join data values from join path spec-string
        /// </summary>
        /// <param name="columnPath">String configuration for table joins</param>
        /// <returns>Instance of <see cref="TableJoinData"/> is string is contains table join data; otherwise <see langword="null"/></returns>
        private static TableJoinData GetTableJoinData(string columnPath)
        {
            if (!TableColumnPathRegex.IsMatch(columnPath))
            {
                return null;
            }

            var parts = columnPath.Replace("[", "").Replace("]", "").Split(":");

            return new TableJoinData(columnPath)
            {
                JoinType = TableJoinType.Inner,
                RightTableName = parts[0],
                RightTableRelationColumn = parts[1],
                LeftTableRelationColumn = parts[2],
            };
        }
    }
}
