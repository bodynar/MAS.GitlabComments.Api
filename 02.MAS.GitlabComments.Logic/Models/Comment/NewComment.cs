namespace MAS.GitlabComments.Logic.Models
{
    using System;

    /// <summary>
    /// Created comment data
    /// </summary>
    public class NewComment
    {
        /// <summary>
        /// Generated identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Generated number
        /// </summary>
        public string Number { get; set; }
    }
}
