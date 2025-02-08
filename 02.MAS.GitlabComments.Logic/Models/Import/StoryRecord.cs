namespace MAS.GitlabComments.Logic.Models.Import
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Data;

    /// <summary>
    /// Exported model of instance <see cref="StoryRecord"/>
    /// </summary>
    public record StoryRecordExportModel
    {
        /// <summary>
        /// Date of creation
        /// </summary>
        public DateTime CreatedOn { get; init; }

        /// <summary>
        /// Flag representing state: is increment action retracted
        /// </summary>
        public bool IsRetracted { get; init; }

        /// <summary>
        /// Related tokens
        /// </summary>
        public IEnumerable<RetractionTokenExportModel> RetractionTokens { get; init; }
    }
}
