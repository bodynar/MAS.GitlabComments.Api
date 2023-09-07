namespace MAS.GitlabComments.Data
{
    /// <summary>
    /// Gitlab comment
    /// </summary>
    public class Comment : BaseEntity
    {
        /// <summary>
        /// Comment text
        /// </summary>
        public virtual string Message { get; set; }

        /// <summary>
        /// Comment with link to rules
        /// </summary>
        public virtual string CommentWithLinkToRule { get; set; }

        /// <summary>
        /// Explanation
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Amount of times when comment was posted
        /// </summary>
        public virtual long AppearanceCount { get; set; }
    }
}
