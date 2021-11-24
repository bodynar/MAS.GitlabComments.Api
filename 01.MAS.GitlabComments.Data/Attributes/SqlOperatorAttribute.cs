namespace MAS.GitlabComments.Data.Attributes
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SqlOperatorAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Operator { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operator"></param>
        public SqlOperatorAttribute(string @operator)
        {
            Operator = @operator.ToUpper();
        }
    }
}
