namespace MAS.GitlabComments.DataAccess.Models
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
        ///     If map already has value for specified key - it will be overridden (depending on <paramref name="override"/> flag)
        /// </para>
        /// </summary>
        /// <param name="map">Dictionary</param>
        /// <param name="key">Column name</param>
        /// <param name="value">Query parameter value</param>
        /// <param name="update">Override value if found (default is <see langword="true"/>)</param>
        public static void Set(this IDictionary<string, QueryParameter> map, string key, object value, bool update = true)
        {
            if (map.ContainsKey(key))
            {
                if (update)
                {
                    map[key] = new QueryParameter
                    {
                        ColumnName = key,
                        Value = value,
                        ParameterName = map[key].ParameterName,
                    };
                }

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
