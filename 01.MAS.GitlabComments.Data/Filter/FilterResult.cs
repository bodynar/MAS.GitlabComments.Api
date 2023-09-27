namespace MAS.GitlabComments.DataAccess.Filter
{
    using System.Collections.Generic;

    /// <summary>
    /// Built filter data which could be used in query
    /// </summary>
    public class FilterResult
    {
        /// <summary>
        /// Condition part
        /// </summary>
        public string Sql { get; set; } = string.Empty;

        /// <summary>
        /// Parameter name to value map
        /// </summary>
        public IReadOnlyDictionary<string, object> Values { get; set; } = new Dictionary<string, object>();
    }
}
