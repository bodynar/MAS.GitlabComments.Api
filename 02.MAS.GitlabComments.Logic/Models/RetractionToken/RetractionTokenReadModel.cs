namespace MAS.GitlabComments.Logic.Models
{
    using System;

    using MAS.GitlabComments.Data;

    /// <summary>
    /// Read model of <see cref="RetractionToken"/>
    /// </summary>
    public class RetractionTokenReadModel
    {
        /// <summary>
        /// Identifier of token
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identifier of related comment
        /// </summary>
        public Guid CommentId { get; set; }
    }
}
