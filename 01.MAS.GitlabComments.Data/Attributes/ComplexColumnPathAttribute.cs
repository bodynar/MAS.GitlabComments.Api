namespace MAS.GitlabComments.DataAccess.Attributes
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
    }
}
