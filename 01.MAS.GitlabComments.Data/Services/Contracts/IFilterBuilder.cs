namespace MAS.GitlabComments.DataAccess.Services
{
    using System.Collections.Generic;

    using MAS.GitlabComments.DataAccess.Filter;

    /// <summary>
    /// Sql filter builder
    /// </summary>
    public interface IFilterBuilder
    {
        /// <summary>
        /// Build filter from filter model
        /// </summary>
        /// <param name="queryFilterGroup">Filter model</param>
        /// <returns>Pair of sql text and built sql arguments</returns>
        (string sqlCondition, IReadOnlyDictionary<string, object> sqlArguments) Build(FilterGroup queryFilterGroup);
    }
}
