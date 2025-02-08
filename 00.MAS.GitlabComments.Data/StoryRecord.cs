namespace MAS.GitlabComments.Data
{
    using System;

    /// <summary>
    /// Log entity of comment appearance change
    /// </summary>
    public class StoryRecord : BaseEntity
    {
        /// <summary>
        /// Comment identifier
        /// </summary>
        public virtual Guid CommentId { get; set; }

        /// <summary>
        /// Flag representing state: is increment action retracted
        /// </summary>
        public virtual bool IsRetracted { get; set; }
    }
}
