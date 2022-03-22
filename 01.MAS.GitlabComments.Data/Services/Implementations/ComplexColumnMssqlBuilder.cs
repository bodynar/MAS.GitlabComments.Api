namespace MAS.GitlabComments.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using MAS.GitlabComments.Data.Attributes;
    using MAS.GitlabComments.Data.Select;

    /// <summary>
    /// Implementation of <see cref="IComplexColumnQueryBuilder"/> for MSSql
    /// </summary>
    public class ComplexColumnMssqlBuilder : IComplexColumnQueryBuilder
    {
        /// <summary> Regular expression for table join configuration expression template </summary>
        private const string TableColumnPathPattern = @"\[([^\:]*)\:([^\:]*)\:([^\]]*)\]";

        /// <summary> Regular expression for pascal case text </summary>
        private const string PascalCasePathPattern = @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])";

        /// <summary> Instance of <see cref="Regex"/> for table join configuration </summary>
        private Regex TableColumnPathRegex { get; }

        /// <summary> Instance of <see cref="Regex"/> for pascal case text </summary>
        private Regex PascalCaseRegex { get; }

        /// <summary>
        /// Initializing <see cref="ComplexColumnMssqlBuilder"/>
        /// </summary>
        public ComplexColumnMssqlBuilder()
        {
            TableColumnPathRegex = new Regex(TableColumnPathPattern);
            PascalCaseRegex = new Regex(PascalCasePathPattern);
        }

        /// <inheritdoc cref="IComplexColumnQueryBuilder.BuildComplexColumns{TProjection}"/>
        public ComplexColumnData BuildComplexColumns<TProjection>(string sourceTableName)
        {
            var columns = typeof(TProjection).GetProperties();

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
            var selectColumns =
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
                var (tableJoinData, column) = GetColumnPathParts(tableJoinDataItems, complexColumn.Item1, sourceTableName);
                
                if (tableJoinData != null)
                {
                    tableJoinDataItems.AddRange(tableJoinData);
                }

                if (column != null)
                {
                    column.Alias = complexColumn.Item2.Name;

                    selectColumns.Add(column);
                }
            }

            /*
                3. Validate result of ComplexColumnPathQueryBuilder before returning?
             */

            return new ComplexColumnData
            {
                Columns = selectColumns,
                Joins = tableJoinDataItems
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="existedJoins"></param>
        /// <param name="complexColumnPathAttribute"></param>
        /// <param name="sourceTableName"></param>
        /// <returns></returns>
        private (IEnumerable<TableJoinData>, ComplexColumn) GetColumnPathParts(IEnumerable<TableJoinData> existedJoins, ComplexColumnPathAttribute complexColumnPathAttribute, string sourceTableName)
        {
            /* 
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

            if (parts.Length == 0)
            {
                return (null, null);
            }

            if (parts.Length == 1)
            {
                var part = parts.First();

                return Regex.IsMatch(part, TableColumnPathPattern)
                    ? (null, null)
                    : (null, new ComplexColumn
                    {
                        TableAlias = sourceTableName,
                        Name = part,
                    });
            }

            var lastPart = parts.Last();
            var firstPart = parts.First();

            if (TableColumnPathRegex.IsMatch(lastPart) || !TableColumnPathRegex.IsMatch(firstPart))
            {
                // invalid path: [Table1:Table1Column:Table2Column].[Table1:Table1Column:Table2Column]
                // invalid path: SomeColumn.[Table1:Table1Column:Table2Column]

                return (null, null);
            }

            var tableJoinDataItems = new List<TableJoinData>();
            var previousLeftTableName = sourceTableName;

            for (int i = 0; i < parts.Length - 1; i++) // while not last
            {
                var part = parts[i];

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
                    // TODO: log error
                    continue;
                }

                var sameTablesCount =
                    existedJoins.Count(x => x.RightTableName == tableJoinData.RightTableName)
                    + tableJoinDataItems.Count(x => x.RightTableName == tableJoinData.RightTableName);

                var alias = $"{tableNameParts[0]}{sameTablesCount + 1}";
                tableJoinData.Alias = alias;
                previousLeftTableName = tableJoinData.Alias;

                tableJoinDataItems.Add(tableJoinData);
            }

            return (tableJoinDataItems, new ComplexColumn
            {
                TableAlias = previousLeftTableName,
                Name = lastPart,
            });
        }

        /// <summary>
        /// Get table join data values from join path spec-string
        /// </summary>
        /// <param name="columnPath">String configuration for table joins</param>
        /// <returns>Instance of <see cref="TableJoinData"/> is string is contains table join data; otherwise <see langword="null"/></returns>
        private TableJoinData GetTableJoinData(string columnPath)
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
