namespace MAS.GitlabComments.Logic.Models
{
    /// <summary>
    /// Data for updating target comment
    /// </summary>
    public class MergeCommentUpdateModel
    {
        /// <summary>
        /// Message text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Comment with link to rules
        /// </summary>
        public string CommentWithLinkToRule { get; set; }

        /// <summary>
        /// Explanation description
        /// </summary>
        public string Description { get; set; }
    }
}
