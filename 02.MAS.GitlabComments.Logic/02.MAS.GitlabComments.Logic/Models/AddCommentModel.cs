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
        /// Explonation message
        /// </summary>
        public string Description { get; set; }
    }
}
