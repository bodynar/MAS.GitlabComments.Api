namespace MAS.GitlabComments.Logic.Models
{
    using System;

    /// <summary>
    /// Comment story record model
    /// </summary>
    public class StoryRecordViewModel
    {
        /// <summary>
        /// Unique comment identifier
        /// </summary>
        public Guid CommentId { get; set; }

        /// <summary>
        /// Comment message
        /// </summary>
        public string CommentText { get; set; }

        /// <summary>
        /// Amount of increment during selected period
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Unique number
        /// </summary>
        public string Number { get; set; }
    }
}
