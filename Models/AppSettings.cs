namespace MAS.GitlabComments.Models
{
    /// <summary>
    /// Global application settings
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Is application in read only mode
        /// </summary>
        public virtual bool ReadOnlyMode { get; } // virtual required for unit-tests with Moq

        /// <summary>
        /// Initializing <see cref="AppSettings"/>
        /// </summary>
        /// <param name="readOnlyMode">Is application in read only mode</param>
        public AppSettings(bool readOnlyMode)
        {
            ReadOnlyMode = readOnlyMode;
        }
    }
}
