namespace MAS.GitlabComments.Data
{
    /// <summary>
    /// Container of built-in system variables
    /// </summary>
    public static class BuiltInSysVariables
    {
        /// <summary>
        /// Number of last created comment
        /// <para>Type is <see langword="int"/></para>
        /// </summary>
        public const string LastCommentNumber = "LastCommentNumber";

        /// <summary>
        /// Is comments table was updated with Number column unique constraint
        /// <para>Type is <see langword="bool"/></para>
        /// </summary>
        public const string IsCommentNumberColumnUnique = "IsChangeNumberUnique";
    }
}
