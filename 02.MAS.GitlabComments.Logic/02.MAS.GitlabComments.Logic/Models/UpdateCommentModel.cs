namespace MAS.GitlabComments.Logic.Models
{
    using System;

    /// <summary>
    /// Comment model for update operation
    /// </summary>
    public class UpdateCommentModel
    {
        /// <summary>
        /// Comment unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// New message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// New description
        /// </summary>
        public string Description { get; set; }
    }
}
