namespace MAS.GitlabComments.DataAccess.Services
{
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
        /// <returns>Built filter data</returns>
        FilterResult Build(FilterGroup queryFilterGroup);
    }
}
