namespace MAS.GitlabComments.WebApi.Models
{
    using System;

    using MAS.GitlabComments.DataAccess;

    /// <summary>
    /// Global application settings
    /// </summary>
    public class AppSettings : IApplicationWebSettings
    {
        /// <inheritdoc />
        public bool ReadOnlyMode { get; } // virtual required for unit-tests with Moq

        /// <inheritdoc />
        public string CommentNumberTemplate { get; } // virtual required for unit-tests with Moq

        /// <inheritdoc />
        public int RetractionTokenLifeSpanHours { get; }

        /// <inheritdoc />
        public DatabaseType DatabaseType { get; init; }

        /// <summary>
        /// Initializing <see cref="AppSettings"/>
        /// </summary>
        /// <param name="readOnlyMode">Is application in read only mode</param>
        /// <param name="commentNumberTemplate">Comment number template</param>
        /// <param name="retractionTokenLifeSpanHours">Life span of retraction token in hours</param>
        /// <param name="dbType">Type of database server</param>
        /// <exception cref="ArgumentException">Parameter 'retractionTokenLifeSpanHours' is less or equal to zero</exception>
        /// <exception cref="ArgumentException">Parameter 'commentNumberTemplate' is null or whitespace</exception>
        public AppSettings(
            bool readOnlyMode,
            string commentNumberTemplate,
            int retractionTokenLifeSpanHours,
            DatabaseType dbType
        )
        {
            if (retractionTokenLifeSpanHours <= 0)
            {
                throw new ArgumentException("Parameter 'retractionTokenLiveTimeHours' must be greater than 0");
            }

            if (string.IsNullOrWhiteSpace(commentNumberTemplate))
            {
                throw new ArgumentException("Parameter 'commentNumberTemplate' must be set");
            }

            ReadOnlyMode = readOnlyMode;
            CommentNumberTemplate = commentNumberTemplate;
            RetractionTokenLifeSpanHours = retractionTokenLifeSpanHours;
            DatabaseType = dbType;
        }
    }
}
