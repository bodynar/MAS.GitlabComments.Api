namespace MAS.GitlabComments.Controllers
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Attributes;
    using MAS.GitlabComments.Models;
    using MAS.GitlabComments.Services;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [UseReadOnlyMode]
    [Route("api/comments")]
    public class CommentsApiController : ControllerBase
    {
        /// <summary>
        /// Logger to store error information
        /// </summary>
        private ILogger<CommentsApiController> Logger { get; }

        /// <summary>
        /// Service for comments managing
        /// </summary>
        private ICommentService CommentService { get; }

        /// <summary>
        /// Initialize <see cref="CommentsApiController"/>
        /// </summary>
        /// <param name="logger">Logger to store error information</param>
        /// <param name="commentService">Service for comments managing</param>
        /// <exception cref="ArgumentNullException">Parameter commentService is null</exception>
        public CommentsApiController(
            ILogger<CommentsApiController> logger,
            ICommentService commentService
        )
        {
            Logger = logger;
            CommentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }

        /// <summary>
        /// Add comment by specified values
        /// </summary>
        /// <param name="addCommentModel">Comment values</param>
        [HttpPost("add")]
        public BaseServiceResult<Guid> Add([FromBody] AddCommentModel addCommentModel)
        {
            try
            {
                var newId = CommentService.Add(addCommentModel);

                return BaseServiceResult<Guid>.Success(newId);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, "Trying to: add comment.");
                return BaseServiceResult<Guid>.Error(e);
            }
        }

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <returns>Service perform operation result</returns>
        [AllowInReadOnly]
        [HttpGet("getAll")]
        public BaseServiceResult<IEnumerable<CommentModel>> Get()
        {
            try
            {
                var result = CommentService.Get();

                return BaseServiceResult<IEnumerable<CommentModel>>.Success(result);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, "Trying to: Get all comments");
                return BaseServiceResult<IEnumerable<CommentModel>>.Error(e);
            }
        }

        /// <summary>
        /// Get comment item by specifying it's identifier
        /// </summary>
        /// <param name="commentId">Comment identifier value</param>
        /// <returns>Comment model</returns>
        [HttpGet("get")]
        public BaseServiceResult<ExtendedCommentModel> Get([FromQuery] Guid commentId)
        {
            try
            {
                var result = CommentService.Get(commentId);

                return BaseServiceResult<ExtendedCommentModel>.Success(result);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Trying to: Get comment \"{commentId}\".");
                return BaseServiceResult<ExtendedCommentModel>.Error(e);
            }
        }

        /// <summary>
        /// Get description of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        /// <returns>Service perform operation result</returns>
        [AllowInReadOnly]
        [HttpGet("description")]
        public BaseServiceResult<string> GetDescription([FromQuery] Guid commentId)
        {
            try
            {
                var result = CommentService.GetDescription(commentId);

                return BaseServiceResult<string>.Success(result);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Trying to: Get description of comment \"{commentId}\".");
                return BaseServiceResult<string>.Error(e);
            }
        }

        /// <summary>
        /// Increment <see cref="Comment.AppearanceCount"/> property of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        [HttpPost("increment")]
        public BaseServiceResult Increment([FromBody] Guid commentId)
        {
            try
            {
                CommentService.Increment(commentId);

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Trying to: Incrementing \"{commentId}\".");
                return BaseServiceResult.Error(e);
            }
        }

        /// <summary>
        /// Update specified comment by values
        /// </summary>
        /// <param name="updateCommentModel">Comment new values</param>
        [HttpPost("update")]
        public BaseServiceResult Update([FromBody] UpdateCommentModel updateCommentModel)
        {
            try
            {
                CommentService.Update(updateCommentModel);

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Trying to: Update comment \"{updateCommentModel?.Id}\".");
                return BaseServiceResult.Error(e);
            }
        }

        /// <summary>
        /// Delete comments by specified identifiers
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        [HttpPost("delete")]
        public BaseServiceResult Delete([FromBody] Guid commentId)
        {
            try
            {
                CommentService.Delete(commentId);

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Trying to: Delete comment \"{commentId}\".");
                return BaseServiceResult.Error(e);
            }
        }
    }
}
