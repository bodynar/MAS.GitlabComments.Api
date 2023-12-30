namespace MAS.GitlabComments.Logic.Models
{
    /// <summary>
    /// Possible system variable types
    /// </summary>
    public enum SysVariableType
    {
        /// <summary>
        /// Type is not specified
        /// </summary>
        None,

        /// <summary>
        /// String data
        /// </summary>
        String,

        /// <summary>
        /// Anything digits only value without decimal point
        /// </summary>
        Int,

        /// <summary>
        /// Number with decimal point
        /// </summary>
        Decimal,

        /// <summary>
        /// Flag value
        /// </summary>
        Bool,

        /// <summary>
        /// Complex object in json
        /// </summary>
        Json,
    }
}
