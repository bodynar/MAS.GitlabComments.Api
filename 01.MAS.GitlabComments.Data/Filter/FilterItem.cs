namespace MAS.GitlabComments.Data.Filter
{
    /// <summary>
    /// Filter.
    /// Represents single sql column value logical comparison
    /// </summary>
    public class FilterItem
    {
        /// <summary>
        /// Unique name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Comparison value
        /// </summary>
        public object Value { get; set; }

        /// <inheritdoc cref="ComparisonType"/>
        public ComparisonType LogicalComparisonType { get; set; }
    }
}
