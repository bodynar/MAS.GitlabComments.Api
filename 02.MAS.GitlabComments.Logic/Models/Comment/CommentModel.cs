namespace MAS.GitlabComments.Logic.Models
{
    using System;

    /// <summary>
    /// Comment model for displaying
    /// </summary>
    public class CommentModel
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Amount of appearance time
        /// </summary>
        public long AppearanceCount { get; set; }

        /// <summary>
        /// Comment with link to rules
        /// </summary>
        public string CommentWithLinkToRule { get; set; }
    }
}
