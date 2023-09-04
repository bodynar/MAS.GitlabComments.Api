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
        /// Flag representing sign of action
        /// <para><see langword="true"/> if it is increment action, otherwise <see langword="false"/></para>
        /// </summary>
        public virtual bool IsIncrementAction { get; set; }
    }
}
