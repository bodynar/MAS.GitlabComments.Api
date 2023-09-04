namespace MAS.GitlabComments.DataAccess.Filter
{
    using MAS.GitlabComments.DataAccess.Attributes;

    /// <summary>
    /// Sql filter join types
    /// </summary>
    public enum FilterJoinType
    {
        /// <summary>
        /// Default value, not valid
        /// </summary>
        None = 0,

        /// <summary>
        /// Record must satisfy all filters in group to be selected
        /// <para>Logical multiplication</para>
        /// </summary>
        [SqlOperator("AND")]
        And = 1,

        /// <summary>
        /// Record must satisfy atleast one filters in group to be selected
        /// <para>Logical sum</para>
        /// </summary>
        [SqlOperator("OR")]
        Or = 2
    }
}
