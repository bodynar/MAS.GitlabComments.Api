namespace MAS.GitlabComments.Logic.Models
{
    using System;

    using MAS.GitlabComments.Data.Attributes;
    using MAS.GitlabComments.Data.Select;
    using MAS.GitlabComments.Data.Services;

    /// <summary>
    /// Comment story record model to read via <see cref="IDataProvider{TEntity}.Select{TProjection}(SelectConfiguration)"/>
    /// </summary>
    public class StoryRecordReadModel
    {
        /// <summary>
        /// Unique comment identifier
        /// </summary>
        [ComplexColumnPath("[Comments:Id:CommentId].Id")]
        public Guid CommentId { get; set; }

        /// <summary>
        /// Comment message
        /// </summary>
        [ComplexColumnPath("[Comments:Id:CommentId].Message")]
        public string CommentText { get; set; }
    }
}
