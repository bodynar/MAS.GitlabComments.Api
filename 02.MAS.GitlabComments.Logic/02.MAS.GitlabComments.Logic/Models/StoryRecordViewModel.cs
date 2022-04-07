namespace MAS.GitlabComments.Logic.Models
{
    using System;

    using MAS.GitlabComments.Data.Attributes;

    /// <summary>
    /// Comment story record model
    /// </summary>
    public class StoryRecordViewModel
    {
        /// <summary>
        /// Unique comment identifier
        /// </summary>
        [ComplexColumnPath("[Comments:Id:CommentId].Id")]
        public Guid CommentId { get; set; }

        /// <summary>
        /// Comment message
        /// </summary>
        [ComplexColumnPath("[Comments:Id:CommentId].Message")]
        public string CommentText { get; set; }

        /// <summary>
        /// Amount of increment during selected period
        /// </summary>
        public int IncrementCount { get; set; }
    }
}
