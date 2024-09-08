namespace MAS.GitlabComments.DataAccess.Services.Implementations.DataProvider
{
    using System.Collections.Generic;

    /// <summary>
    /// SQL Parameter related with column
    /// </summary>
    public class QueryParameter
    {
        /// <summary>
        /// Name of table column
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Name of query parameter
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Parameter value
        /// </summary>
        public object Value { get; set; }
    }

    /// <summary>
    /// Extension methods of parameter map (<see cref="Dictionary{TKey, TValue}"/>)
    /// </summary>
    public static class ParameterMapExtensions
    {
        /// <summary>
        /// Set value for specified key
        /// <para>
        ///     If map already has value for specified key - it will be overridden
        /// </para>
        /// </summary>
        /// <param name="map">Dictionary</param>
        /// <param name="key">Column name</param>
        /// <param name="value">Query parameter value</param>
        public static void Set(this IDictionary<string, QueryParameter> map, string key, object value)
        {
            if (map.ContainsKey(key))
            {
                map[key] = new QueryParameter
                {
                    ColumnName = key,
                    Value = value,
                    ParameterName = map[key].ParameterName,
                };

                return;
            }

            map.Add(key, new QueryParameter
            {
                ColumnName = key,
                Value = value,
                ParameterName = $"@P{map.Count + 1}",
            });
        }
    }
}
