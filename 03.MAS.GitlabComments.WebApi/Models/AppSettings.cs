namespace MAS.GitlabComments.WebApi.Models
{
    using System;

    /// <summary>
    /// Global application settings
    /// </summary>
    public class AppSettings : IApplicationWebSettings
    {
        /// <summary>
        /// Default value for template for comment number
        /// </summary>
        public const string CommentDefaultNumberTemplate = "{0:00}";

        /// <inheritdoc />
        public bool ReadOnlyMode { get; } // virtual required for unit-tests with Moq

        /// <inheritdoc />
        public string CommentNumberTemplate { get; } // virtual required for unit-tests with Moq

        /// <inheritdoc />
        public int RetractionTokenLifeSpanHours { get; }

        /// <summary>
        /// Initializing <see cref="AppSettings"/>
        /// </summary>
        /// <param name="readOnlyMode">Is application in read only mode</param>
        /// <param name="commentNumberTemplate">Comment number template</param>
        /// <param name="retractionTokenLifeSpanHours">Life span of retraction token in hours</param>
        /// <exception cref="ArgumentException">Parameter 'retractionTokenLifeSpanHours' is less or equal to zero</exception>
        public AppSettings(
            bool readOnlyMode,
            string commentNumberTemplate,
            int retractionTokenLifeSpanHours
        )
        {
            ReadOnlyMode = readOnlyMode;
            RetractionTokenLifeSpanHours = retractionTokenLifeSpanHours;

            if (retractionTokenLifeSpanHours <= 0)
            {
                throw new ArgumentException("Parameter 'retractionTokenLiveTimeHours' must be greater than 0");
            }

            CommentNumberTemplate = string.IsNullOrWhiteSpace(commentNumberTemplate)
                    ? CommentDefaultNumberTemplate
                    : commentNumberTemplate;
        }
    }
}
