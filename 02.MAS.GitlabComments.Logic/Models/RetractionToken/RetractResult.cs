namespace MAS.GitlabComments.Logic.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Result of massive retraction operation
    /// </summary>
    public class RetractResult
    {
        /// <summary>
        /// Successfully retracted operations
        /// </summary>
        public IEnumerable<RetractOperationResult> Success { get; set; }

        /// <summary>
        /// Information about outdated tokens
        /// </summary>
        public IEnumerable<RetractOperationResult> Outdated { get; set; }

        /// <summary>
        /// Failed operation due error
        /// </summary>
        public IEnumerable<RetractOperationResult> Errors { get; set; }
    }
}
