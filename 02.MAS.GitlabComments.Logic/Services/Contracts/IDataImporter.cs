namespace MAS.GitlabComments.Logic.Services
{
    using System.Collections.Generic;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.Logic.Models.Import;

    /// <summary>
    /// Service that can perform import\export to migrate app data (e.g. between app instances)
    /// </summary>
    public interface IDataImporter
    {
        /// <summary>
        /// Import exported data to app
        /// <para>
        ///     Saves all comments, story records and tokens, ignoring base properties (<see cref="BaseEntity.Id"/>, <see cref="BaseEntity.CreatedOn"/>, <see cref="BaseEntity.ModifiedOn"/>), except: StoryRecord.ModifiedOn
        /// </para>
        /// </summary>
        /// <param name="comments">Exported data</param>
        void ImportAppData(IEnumerable<CommentExportModel> comments);

        /// <summary>
        /// Export all app data in deep model
        /// </summary>
        /// <returns>All comments with nested data</returns>
        IEnumerable<CommentExportModel> ExportAppData();
    }
}
