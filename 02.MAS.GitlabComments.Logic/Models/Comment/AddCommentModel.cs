namespace MAS.GitlabComments.Logic.Models
{
    /// <summary>
    /// Comment model for add operation
    /// </summary>
    public class AddCommentModel
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

        /// <summary>
        /// Is model used due import action
        /// </summary>
        public bool IsImportAction { get; set; }

        /// <summary>
        /// Manual appearance count value
        /// </summary>
        public long AppearanceCount { get; set; } = 1;
    }
}
