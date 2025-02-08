namespace MAS.GitlabComments.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Base;
    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.WebApi.Attributes;
    using MAS.GitlabComments.WebApi.Models;

    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [UseReadOnlyMode]
    [Route("api/ret-tokens")]
    public class RetractionTokenController : ControllerBase
    {
        /// <summary>
        /// Logger to store error information
        /// </summary>
        private ILogger Logger { get; }

        /// <inheritdoc cref="IRetractionTokenManager" />
        public IRetractionTokenManager TokenManager { get; }

        /// <summary>
        /// Initialize <see cref="RetractionTokenController"/>
        /// </summary>
        /// <param name="logger">Logger to store error information</param>
        /// <param name="tokenManager">Retraction tokens manager</param>
        /// <exception cref="ArgumentNullException">Some parameters is null</exception>
        public RetractionTokenController(
            ILogger logger,
            IRetractionTokenManager tokenManager
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            TokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
        }

        /// <summary>
        /// Retract single token
        /// </summary>
        /// <param name="tokenId">Token identifier</param>
        /// <returns>Result of operation, instance of <see cref="BaseServiceResult"/></returns>
        [HttpPost("retract")]
        public BaseServiceResult Retract([FromBody] Guid tokenId)
        {
            try
            {
                TokenManager.Retract(tokenId);

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Retracting \"{tokenId}\"");
                return BaseServiceResult.Error(e);
            }
        }

        /// <summary>
        /// Retract batch of tokens
        /// </summary>
        /// <param name="tokenIds">Token identifiers</param>
        /// <returns>Operation result, wrapped in <see cref="BaseServiceResult{TResult}"/></returns>
        [HttpPost("batchRetract")]
        public BaseServiceResult<RetractResult> Retract([FromBody]IEnumerable<Guid> tokenIds)
        {
            try
            {
                var result = TokenManager.Retract(tokenIds);

                return BaseServiceResult<RetractResult>.Success(result);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Batch retracting tokens");
                return BaseServiceResult<RetractResult>.Error(e);
            }
        }

        /// <summary>
        /// Get non-expired tokens
        /// </summary>
        /// <returns>All valid tokens as boxed result, wrapped in <see cref="BaseServiceResult{TResult}"/></returns>
        [HttpGet("getAll")]
        public BaseServiceResult<IEnumerable<RetractionTokenReadModel>> GetAvailable()
        {
            try
            {
                var available = TokenManager.GetAvailable();

                return BaseServiceResult<IEnumerable<RetractionTokenReadModel>>.Success(available);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Reading active tokens");
                return BaseServiceResult<IEnumerable<RetractionTokenReadModel>>.Error(e);
            }
        }

        /// <summary>
        /// Remove all expired tokens
        /// </summary>
        /// <returns>Result of operation, instance of <see cref="BaseServiceResult"/></returns>
        [HttpPost("removeExpired")]
        public BaseServiceResult RemoveExpired()
        {
            try
            {
                TokenManager.RemoveExpired();

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Removing expired");
                return BaseServiceResult.Error(e);
            }
        }
    }
}