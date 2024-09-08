namespace MAS.GitlabComments.DataAccess.Services
{
    using MAS.GitlabComments.DataAccess.Select;

    /// <summary>
    /// Builder of complex query columns
    /// </summary>
    public interface IComplexColumnQueryBuilder
    {
        /// <summary>
        /// Build complex column data to properly select columns from related tables
        /// </summary>
        /// <typeparam name="TProjection">Type of model where to select data</typeparam>
        /// <param name="sourceTableName">Name of source table</param>
        /// <returns>Instance of <see cref="ComplexColumnData"/> if data built properly, otherwise <see langword="null"/></returns>
        ComplexColumnData BuildComplexColumns<TProjection>(string sourceTableName);
    }
}
