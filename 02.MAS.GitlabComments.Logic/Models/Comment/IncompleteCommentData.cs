namespace MAS.GitlabComments.Logic.Models
{
    using System;

    /// <summary>
    /// Model of comment with incomplete data
    /// </summary>
    public class IncompleteCommentData
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Amount of appearance time
        /// </summary>
        public long AppearanceCount { get; set; }
    }
}
