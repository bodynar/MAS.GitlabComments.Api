namespace MAS.GitlabComments.DataAccess
{
    /// <summary>
    /// Type of database server
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// Data base is an MS SQL Server instance
        /// </summary>
        MSSQL = 1,

        /// <summary>
        /// Data base is an Postgre SQL Server instance
        /// </summary>
        PGSQL = 2,
    }
}
