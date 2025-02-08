namespace MAS.GitlabComments.DataAccess
{
    /// <summary>
    /// Something that could be converted to SQL
    /// </summary>
    public interface IQueryPart
    {
        /// <summary>
        /// Convert to sql query part
        /// </summary>
        /// <param name="databaseType">Type of database server</param>
        /// <returns>Instance of <see cref="string"/> with sql query part</returns>
        string ToQuery(DatabaseType databaseType);
    }
}
