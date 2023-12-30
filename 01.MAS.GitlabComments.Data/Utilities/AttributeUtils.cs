namespace MAS.GitlabComments.DataAccess.Utilities
{
    using System;

    using MAS.GitlabComments.DataAccess.Attributes;

    /// <summary>
    /// Attribute utilities
    /// </summary>
    public static class AttributeUtils
    {
        /// <summary>
        /// Get enum SqlOperator attribute value
        /// </summary>
        /// <param name="value">Enumeration value</param>
        /// <returns>Sql operator from attribute if enumeration value has specified attribute; otherwise <see cref="string.Empty"/></returns>
        public static string GetSqlOperator(this Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var field = value.GetType().GetField(value.ToString());

            if (field == null)
            {
                return string.Empty;
            }

            var comparisonOperatorAttributes =
                field.GetCustomAttributes(typeof(SqlOperatorAttribute), false) as SqlOperatorAttribute[];

            if (comparisonOperatorAttributes != null && comparisonOperatorAttributes.Length > 0)
            {
                return comparisonOperatorAttributes[0].Operator;
            }

            return string.Empty;
        }
    }
}
