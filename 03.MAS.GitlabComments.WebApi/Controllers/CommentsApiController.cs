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
    [Route("api/comments")]
    public class CommentsApiController : ControllerBase
    {
        /// <summary>
        /// Logger to store error information
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Service for comments managing
        /// </summary>
        private ICommentService CommentService { get; }

        /// <summary>
        /// Initialize <see cref="CommentsApiController"/>
        /// </summary>
        /// <param name="logger">Logger to store error information</param>
        /// <param name="commentService">Service for comments managing</param>
        /// <exception cref="ArgumentNullException">Some parameters is null</exception>
        public CommentsApiController(
            ILogger logger,
            ICommentService commentService
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            CommentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }

        /// <summary>
        /// Add comment by specified values
        /// </summary>
        /// <param name="addCommentModel">Comment values</param>
        [HttpPost("add")]
        public BaseServiceResult<NewComment> Add([FromBody] AddCommentApiModel addCommentModel)
        {
            try
            {
                var newComment = CommentService.Add(new AddCommentModel
                {
                    Message = addCommentModel.Message,
                    CommentWithLinkToRule = addCommentModel.CommentWithLinkToRule,
                    Description = addCommentModel.Description,
                });

                return BaseServiceResult<NewComment>.Success(newComment);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Trying to: add comment.");
                return BaseServiceResult<NewComment>.Error(e);
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
                Logger.Error(e, "Trying to: Get all comments");
                return BaseServiceResult<IEnumerable<CommentModel>>.Error(e);
            }
        }

        /// <summary>
        /// Increment <see cref="Comment.AppearanceCount"/> property of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        [HttpPost("increment")]
        public BaseServiceResult<Guid> Increment([FromBody] Guid commentId)
        {
            try
            {
                var tokenId = CommentService.Increment(commentId);

                return BaseServiceResult<Guid>.Success(tokenId);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Trying to: Incrementing \"{commentId}\".");
                return BaseServiceResult<Guid>.Error(e);
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
                Logger.Error(e, $"Trying to: Update comment \"{updateCommentModel?.Id}\".");
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
                Logger.Error(e, $"Trying to: Delete comment \"{commentId}\".");
                return BaseServiceResult.Error(e);
            }
        }

        /// <summary>
        /// Get incomplete comments data
        /// </summary>
        [HttpGet("getIncomplete")]
        public BaseServiceResult<IEnumerable<IncompleteCommentData>> GetIncomplete()
        {
            try
            {
                var result = CommentService.GetIncomplete();

                return BaseServiceResult<IEnumerable<IncompleteCommentData>>.Success(result);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Get incomplete comments");
                return BaseServiceResult<IEnumerable<IncompleteCommentData>>.Error(e);
            }
        }

        /// <summary>
        /// Update incomplete comments
        /// </summary>
        [HttpPost("updateIncomplete")]
        public BaseServiceResult UpdateIncomplete()
        {
            try
            {
                CommentService.UpdateIncomplete();

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Updating incomplete comments failed");
                return BaseServiceResult.Error(e);
            }
        }

        /// <inheritdoc cref="ICommentService.Merge(Guid, Guid, IReadOnlyDictionary{string, object})"/>
        /// <param name="mergeCommentModel">Information about merge operation</param>
        /// <returns>Result of operation, represented by <see cref="BaseServiceResult"/></returns>
        [HttpPost("merge")]
        public BaseServiceResult Merge([FromBody]MergeCommentModel mergeCommentModel)
        {
            try
            {
                CommentService.Merge(
                    mergeCommentModel.SourceCommentId,
                    mergeCommentModel.TargetCommentId,
                    mergeCommentModel.NewTargetValues?.ToMergeModel()
                );

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Merging comment \"{mergeCommentModel.SourceCommentId}\" into \"{mergeCommentModel.TargetCommentId}\"");
                return BaseServiceResult.Error(e);
            }
        }
    }
}
