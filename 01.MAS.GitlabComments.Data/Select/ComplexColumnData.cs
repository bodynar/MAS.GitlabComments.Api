namespace MAS.GitlabComments.Data.Select
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Information about complex columns in select query
    /// <para>
    ///     Contains data about required joins and final path for selecting columns
    /// </para>
    /// </summary>
    public class ComplexColumnData
    {
        /// <summary>
        /// Columns paths to include in select
        /// </summary>
        public IEnumerable<ComplexColumn> Columns { get; set; } = Enumerable.Empty<ComplexColumn>();

        /// <summary>
        /// Tables join data
        /// </summary>
        public IEnumerable<TableJoinData> Joins { get; set; } = Enumerable.Empty<TableJoinData>();
    }
}
