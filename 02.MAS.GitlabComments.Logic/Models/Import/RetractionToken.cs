namespace MAS.GitlabComments.Logic.Models.Import
{
    using System;

    using MAS.GitlabComments.Data;

    /// <summary>
    /// Exported model of instance <see cref="RetractionToken"/>
    /// </summary>
    public record RetractionTokenExportModel
    {
        /// <summary>
        /// Point in time, after which token cannot be used
        /// </summary>
        public DateTime ValidUntil { get; init; }
    }
}
