namespace MAS.GitlabComments.DataAccess.Attributes
{
    using System;

    /// <summary>
    /// Sql operator representative
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SqlOperatorAttribute : Attribute
    {
        /// <summary>
        /// Sql operator key
        /// </summary>
        public string Operator { get; }

        /// <summary>
        /// Initializing <see cref="SqlOperatorAttribute"/>
        /// </summary>
        /// <param name="operator">Sql operator key</param>
        /// <exception cref="ArgumentNullException">Param @operator is null</exception>
        public SqlOperatorAttribute(string @operator)
        {
            Operator = @operator?.ToUpper() ?? throw new ArgumentNullException(nameof(@operator));
        }
    }
}
