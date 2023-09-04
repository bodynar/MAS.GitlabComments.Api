namespace MAS.GitlabComments.DataAccess.Select
{
    using MAS.GitlabComments.DataAccess.Attributes;

    /// <summary>
    /// Type of sql table join
    /// </summary>
    public enum TableJoinType
    {
        /// <summary>
        /// Invalid join type
        /// </summary>
        None = 0,

        /// <summary>
        /// Left outer
        /// </summary>
        [SqlOperator("Left outer")]
        LeftOuter = 1,

        /// <summary>
        /// Left
        /// </summary>
        [SqlOperator("Left")]
        Left = 2,

        /// <summary>
        /// Inner
        /// </summary>
        [SqlOperator("Inner")]
        Inner = 3,

        /// <summary>
        /// Right
        /// </summary>
        [SqlOperator("Right")]
        Right = 4,

        /// <summary>
        /// Right outer
        /// </summary>
        [SqlOperator("Right outer")]
        RightOuter = 5,

        /// <summary>
        /// Full outer
        /// </summary>
        [SqlOperator("Full outer")]
        FullOuter = 6,
    }
}
