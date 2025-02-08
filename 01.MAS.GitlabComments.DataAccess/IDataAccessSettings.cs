namespace MAS.GitlabComments.DataAccess
{
    using MAS.GitlabComments.Base;

    /// <summary>
    /// Application settings for data access layer
    /// </summary>
    public interface IDataAccessSettings: IAppSettings
    {
        /// <inheritdoc cref="DataAccess.DatabaseType"/>
        DatabaseType DatabaseType { get; init; }
    }
}
