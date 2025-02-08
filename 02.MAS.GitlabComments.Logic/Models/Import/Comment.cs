namespace MAS.GitlabComments.Logic.Models.Import
{
    using System.Collections.Generic;

    using MAS.GitlabComments.Data;

    /// <summary>
    /// Exported model of instance <see cref="Comment"/>
    /// </summary>
    public record CommentExportModel
    {
        /// <summary>
        /// Comment text
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// Comment with link to rules
        /// </summary>
        public string CommentWithLinkToRule { get; init; }

        /// <summary>
        /// Explanation
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Amount of times when comment was posted
        /// </summary>
        public long AppearanceCount { get; init; }

        /// <summary>
        /// Related story records
        /// </summary>
        public IEnumerable<StoryRecordExportModel> StoryRecords { get; init; }
    }
}
