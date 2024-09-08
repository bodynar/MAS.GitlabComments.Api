namespace MAS.GitlabComments.Logic.Services
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.Logic.Models;

    /// <summary>
    /// Manager of <see cref="RetractionToken"/>'s
    /// </summary>
    public interface IRetractionTokenManager
    {
        /// <summary>
        /// Create token for retraction operation of comment appearance increment
        /// </summary>
        /// <param name="commentId">Related comment identifier</param>
        /// <returns>Identifier of created token</returns>
        Guid Create(Guid commentId);

        /// <summary>
        /// Get valid retraction tokens
        /// </summary>
        /// <returns>Collection with information about non-expired retraction tokens</returns>
        IEnumerable<RetractionTokenReadModel> GetAvailable();

        /// <summary>
        /// Delete all expired tokens
        /// </summary>
        void RemoveExpired();

        /// <summary>
        /// Perform a comment appearance increment operation retraction by retraction token 
        /// </summary>
        /// <param name="tokenId">Retraction token identifier</param>
        void Retract(Guid tokenId);

        /// <summary>
        /// Comment appearance increment operation retraction by several retraction tokens
        /// </summary>
        /// <param name="tokenIds">Token identifiers</param>
        RetractResult Retract(IEnumerable<Guid> tokenIds);
    }
}
