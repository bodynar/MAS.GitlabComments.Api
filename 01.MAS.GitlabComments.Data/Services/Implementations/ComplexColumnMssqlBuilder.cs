namespace MAS.GitlabComments.Data.Services
{
    using System;
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
        /// <summary> Regular expression for table join expression template </summary>
        private const string TableColumnPathPattern = @"\[([^\:]*)\:([^\:]*)\:([^\]]*)\]";

        /// <inheritdoc cref="IComplexColumnQueryBuilder.BuildComplexColumns{TProjection}"/>
        public ComplexColumnData BuildComplexColumns<TProjection>()
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

            var selectColumns = columnAttributeMap.Where(x => x.Item1 == null).Select(x => x.Item2.Name).ToList();

            var complexColumns = columnAttributeMap.Where(x => x.Item1 != null);

            foreach (var complexColumn in complexColumns)
            {
                var (tableJoinData, column) = GetColumnPathParts(complexColumn.Item1);

                if (tableJoinData == null)
                {
                    if (string.IsNullOrEmpty(column))
                    {
                        continue;
                    }

                    selectColumns.Add(column);
                }


            }

            throw new NotImplementedException();
        }

        // TODO: Change return type to (IEnumerable<TableJoinData>, string)
        private (TableJoinData, string) GetColumnPathParts(ComplexColumnPathAttribute complexColumnPathAttribute)
        {
            string leftTableName = "",
                   leftTableRelationColumn = "",
                   currentTableRelationColumn = "";

            /* 
                1. [Table1:Table1Column:Table2Column].Value 
                    => (LT: Table1, LTC: Table1Column, RT: {no?}, RTC: Table2Column), CP: Value

                2. [Table1:Table1Column:Table2Column].[Table3:Table3Column:Table1Column].Value
                    => ([{ LT: Table1, LTC: Table1Column, RT: {no?}, RTC: Table2Column }, { LT: Table3, LTC: Table3Column, RT: Table1, RTC: Table3Column }]), CP: Value
             

             */

            if (!string.IsNullOrEmpty(complexColumnPathAttribute.LeftTableName))
            {
                leftTableName = complexColumnPathAttribute.LeftTableName;
                leftTableRelationColumn = complexColumnPathAttribute.LeftTableRelationColumn;
                currentTableRelationColumn = complexColumnPathAttribute.CurrentTableRelationColumn;
            }

            var parts = complexColumnPathAttribute.ColumnPath.Split('.');

            if (parts.Length == 1)
            {
                var part = parts.First();

                if (string.IsNullOrEmpty(leftTableName))
                {
                    // When Attribute contains only simple column

                    return Regex.IsMatch(part, TableColumnPathPattern)
                        ? (null, null)
                        : (null, part);
                }

                return Regex.IsMatch(part, TableColumnPathPattern)
                        ? (null, null)
                        : (new TableJoinData
                        {
                            JoinType = TableJoinType.Inner,
                            LeftTableName = leftTableName,
                            LeftTableRelationColumn = leftTableRelationColumn,
                            RightTableRelationColumn = currentTableRelationColumn,
                        }, part);
            }

            return (null, null);
        }
    }
}
