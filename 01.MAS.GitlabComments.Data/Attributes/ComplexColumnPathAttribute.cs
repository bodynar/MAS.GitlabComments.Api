namespace MAS.GitlabComments.Data.Attributes
{
    using System;

    /// <summary>
    /// Complex column path attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ComplexColumnPathAttribute : Attribute
    {
        /// <summary>
        /// Complex column path
        /// </summary>
        /// <example>
        ///     [Table1:Table1Column:Table2Column].Value
        ///         Where Table2Column is FK to Table1
        ///         and SomeColumn is member of Table2
        /// 
        ///     OR
        ///     [Table1:Table1Column:Table2Column].[Table3:Table3Column:Table1Column].Value
        ///     
        ///     Simple [Table1:Table1Column:Table2Column] at the end of path is not valid
        /// </example>
        public string ColumnPath { get; }

        /// <summary>
        /// Name of left table to join
        /// <para>Sets manually to prevent value calculating from column path</para>
        /// </summary>
        public string LeftTableName { get; }

        /// <summary>
        /// Name of relation column in left table to join
        /// <para>Sets manually to prevent value calculating from column path</para>
        /// </summary>
        public string LeftTableRelationColumn { get; }

        /// <summary>
        /// Name of relation column in current table to join
        /// <para>Sets manually to prevent value calculating from column path</para>
        /// </summary>
        public string CurrentTableRelationColumn { get; }

        /// <summary>
        /// Initializing <see cref="ComplexColumnPathAttribute"/>
        /// </summary>
        /// <param name="columnPath">Complex column path</param>
        /// <exception cref="ArgumentNullException">Parameter columnPath is null</exception>
        public ComplexColumnPathAttribute(string columnPath)
        {
            if (string.IsNullOrWhiteSpace(columnPath))
            {
                throw new ArgumentNullException(nameof(columnPath));
            }

            ColumnPath = columnPath;
        }

        /// <summary>
        /// Initializing <see cref="ComplexColumnPathAttribute"/>
        /// </summary>
        /// <param name="columnPath">Final column path</param>
        /// <param name="leftTableName">Name of left table to join</param>
        /// <param name="leftTableRelationColumn">Name of relation column in left table to join</param>
        /// <param name="currentTableRelationColumn">Name of relation column in current table to join</param>
        /// <exception cref="ArgumentNullException">Parameter columnPath is null</exception>
        /// <exception cref="ArgumentNullException">Parameter leftTableName is null</exception>
        /// <exception cref="ArgumentNullException">Parameter leftTableRelationColumn is null</exception>
        /// <exception cref="ArgumentNullException">Parameter currentTableRelationColumn is null</exception>
        public ComplexColumnPathAttribute(string columnPath, string leftTableName, string leftTableRelationColumn, string currentTableRelationColumn)
            : this(columnPath)
        {
            if (string.IsNullOrWhiteSpace(leftTableName))
            {
                throw new ArgumentNullException(nameof(leftTableName));
            }

            if (string.IsNullOrWhiteSpace(leftTableRelationColumn))
            {
                throw new ArgumentNullException(nameof(leftTableRelationColumn));
            }

            if (string.IsNullOrWhiteSpace(currentTableRelationColumn))
            {
                throw new ArgumentNullException(nameof(currentTableRelationColumn));
            }

            LeftTableName = leftTableName;
            LeftTableRelationColumn = leftTableRelationColumn;
            CurrentTableRelationColumn = currentTableRelationColumn;
        }
    }
}
