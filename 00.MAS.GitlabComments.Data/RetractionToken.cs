namespace MAS.GitlabComments.Data
{
    using System;

    /// <summary>
    /// Token for comment increment action retraction
    /// </summary>
    public class RetractionToken : BaseEntity
    {
        /// <summary>
        /// Point in time, after which token cannot be used
        /// </summary>
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// Identifier of related comment
        /// </summary>
        public Guid CommentId { get; set; }

        /// <summary>
        /// Identifier of related story record
        /// </summary>
        public Guid StoryRecordId { get; set; }
    }
}
