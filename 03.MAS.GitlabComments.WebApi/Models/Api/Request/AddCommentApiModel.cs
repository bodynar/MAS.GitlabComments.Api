namespace MAS.GitlabComments.WebApi.Models
{
    /// <summary>
    /// Comment model for add operation
    /// </summary>
    public class AddCommentApiModel
    {
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Explanation message
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Comment with link to rules
        /// </summary>
        public string CommentWithLinkToRule { get; set; }
    }
}
