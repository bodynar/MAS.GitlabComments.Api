namespace MAS.GitlabComments.DataAccess.Filter
{
    using MAS.GitlabComments.DataAccess.Attributes;

    /// <summary>
    /// Types of value comparison
    /// </summary>
    public enum ComparisonType
    {
        /// <summary>
        /// Default value, not valid
        /// </summary>
        None = 0,

        /// <summary>
        /// Equality
        /// <para>Left value must be equal to right value</para>
        /// </summary>
        [SqlOperator("=")]
        Equal = 1,

        /// <summary>
        /// Iversive equality
        /// <para>Left value must be not equal to right value</para>
        /// </summary>
        [SqlOperator("!=")]
        NotEqual = 2,

        /// <summary>
        /// Left value must be less than right value
        /// </summary>
        [SqlOperator("<")]
        Less = 3,

        /// <summary>
        /// Left value must be less or equal than right value
        /// </summary>
        [SqlOperator("<=")]
        LessOrEqual = 4,

        /// <summary>
        /// Left value must be greater than right value
        /// </summary>
        [SqlOperator(">")]
        Greater = 5,

        /// <summary>
        /// Left value must be greater or equal than right value
        /// </summary>
        [SqlOperator(">=")]
        GreaterOrEqual = 6,
    }
}
