namespace MAS.GitlabComments.Logic.Models
{
    using System;

    /// <summary>
    /// Comment model for description display
    /// </summary>
    public class ExtendedCommentModel
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
        /// Description
        /// </summary>
        public string Description { get; set; }
    }
}
