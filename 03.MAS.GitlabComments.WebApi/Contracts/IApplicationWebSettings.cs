namespace MAS.GitlabComments.WebApi
{
    using MAS.GitlabComments.Logic.Services;

    /// <summary>
    /// Web application settings
    /// </summary>
    public interface IApplicationWebSettings : IApplicationSettings
    {
        /// <summary>
        /// Is application in read only mode
        /// </summary>
        bool ReadOnlyMode { get; }
    }
}
