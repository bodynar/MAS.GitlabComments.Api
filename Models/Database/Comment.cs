namespace MAS.GitlabComments.Models.Database
{
    using System;

    /// <summary>
    /// Gitlab comment
    /// </summary>
    public class Comment : BaseEntity
    {
        /// <summary>
        /// Date of last update
        /// </summary>
        public virtual DateTime ModifiedOn { get; set; }

        /// <summary>
        /// Comment text
        /// </summary>
        public virtual string Message { get; set; }

        /// <summary>
        /// Explonation
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Amount of times when comment was posted
        /// </summary>
        public virtual long AppearanceCount { get; set; }
    }
}
