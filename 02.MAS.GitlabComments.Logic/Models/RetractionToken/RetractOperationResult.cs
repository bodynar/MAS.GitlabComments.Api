namespace MAS.GitlabComments.Logic.Models
{
    using System;

    /// <summary>
    /// Information about operation retraction data
    /// </summary>
    public class RetractOperationResult
    {
        /// <summary>
        /// Identifier of token
        /// </summary>
        public Guid TokenId { get; set; }

        /// <summary>
        /// Extra error message
        /// </summary>
        public string Error { get; set; }
    }
}
