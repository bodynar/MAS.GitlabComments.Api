namespace MAS.GitlabComments.WebApi.Models
{
    /// <summary>
    /// Global application settings
    /// </summary>
    public class AppSettings: IApplicationWebSettings
    {
        /// <summary>
        /// Default value for template for comment number
        /// </summary>
        public const string CommentDefaultNumberTemplate = "{0:00}";

        /// <summary>
        /// Is application in read only mode
        /// </summary>
        public bool ReadOnlyMode { get; } // virtual required for unit-tests with Moq

        /// <summary>
        /// Comment number template
        /// </summary>
        public string CommentNumberTemplate { get; } // virtual required for unit-tests with Moq

        /// <summary>
        /// Initializing <see cref="AppSettings"/>
        /// </summary>
        /// <param name="readOnlyMode">Is application in read only mode</param>
        /// <param name="commentNumberTemplate">Comment number template</param>
        public AppSettings(bool readOnlyMode, string commentNumberTemplate)
        {
            ReadOnlyMode = readOnlyMode;

            CommentNumberTemplate = string.IsNullOrWhiteSpace(commentNumberTemplate)
                    ? CommentDefaultNumberTemplate
                    : commentNumberTemplate; // TODO: add tests
        }
    }
}
