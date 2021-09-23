namespace MAS.GitlabComments.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    using MAS.GitlabComments.Exceptions;
    using MAS.GitlabComments.Models;

    /// <summary>
    /// Service for managing <see cref="Comment"/>
    /// </summary>
    public class CommentService : ICommentService
    {
        private IDataProvider<Comment> CommentsDataProvider { get; }

        /// <summary>
        /// Initializing <see cref="CommentService"/>
        /// </summary>
        /// <param name="commentsDataProvider">Comments data provider</param>
        /// <exception cref="ArgumentNullException">Parameter commentsDataProvider is null</exception>
        public CommentService(IDataProvider<Comment> commentsDataProvider)
        {
            CommentsDataProvider = commentsDataProvider ?? throw new ArgumentNullException(nameof(commentsDataProvider));
        }

        /// <summary>
        /// Add comment by specified values
        /// </summary>
        /// <param name="addCommentModel">Comment values</param>
        /// <exception cref="ArgumentNullException">Parameter addCommentModel is null</exception>
        /// <exception cref="ArgumentNullException">Message isn't specified</exception>
        /// <returns>Identifier value of new created comment</returns>
        public Guid Add(AddCommentModel addCommentModel)
        {
            if (addCommentModel == null)
            {
                throw new ArgumentNullException(nameof(addCommentModel));
            }

            if (string.IsNullOrEmpty(addCommentModel.Message))
            {
                throw new ArgumentNullException(nameof(addCommentModel.Message));
            }

            var newId = Guid.NewGuid();

            CommentsDataProvider.Add(new Comment
            {
                Id = newId,
                AppearanceCount = 1,
                CreatedOn = DateTime.UtcNow,
                Message = addCommentModel.Message,
                Description = addCommentModel.Description
            });

            return newId;
        }

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <returns>All comments</returns>
        public IEnumerable<CommentModel> Get()
        {
            return
                CommentsDataProvider
                    .Get()
                    .OrderByDescending(x => x.AppearanceCount)
                    .Select(x => new CommentModel
                    {
                        Id = x.Id,
                        Message = x.Message,
                        AppearanceCount = x.AppearanceCount
                    });
        }

        /// <summary>
        /// Get comment item by specifying it's identifier
        /// </summary>
        /// <param name="commentId">Comment identifier value</param>
        /// <exception cref="ArgumentNullException">Parameter commentId is default</exception>
        /// <exception cref="EntityNotFoundException">Comment not found</exception>
        /// <returns>Comment model</returns>
        public ExtendedCommentModel Get(Guid commentId)
        {
            var entity = GetCommentWithWithChecking(commentId);

            return new ExtendedCommentModel
            {
                Id = entity.Id,
                Message = entity.Message,
                Description = entity.Description,
            };
        }

        /// <summary>
        /// Get description of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        /// <exception cref="ArgumentNullException">Parameter commentId is default</exception>
        /// <exception cref="EntityNotFoundException">Comment not found</exception>
        /// <returns>Description if specified; otherwise <see cref="string.Empty"/></returns>
        public string GetDescription(Guid commentId)
        {
            var comment = GetCommentWithWithChecking(commentId);

            return comment.Description ?? string.Empty;
        }

        /// <summary>
        /// Update specified comment by values
        /// </summary>
        /// <param name="updateCommentModel">Comment new values</param>
        /// <exception cref="ArgumentNullException">Parameter updateCommentModel is null</exception>
        /// <exception cref="ArgumentNullException">Message isn't specified</exception>
        /// <exception cref="ArgumentNullException">Parameter commentId is default</exception>
        /// <exception cref="EntityNotFoundException">Comment not found</exception>
        public void Update(UpdateCommentModel updateCommentModel)
        {
            if (updateCommentModel == null)
            {
                throw new ArgumentNullException(nameof(updateCommentModel));
            }
            if (string.IsNullOrEmpty(updateCommentModel.Message))
            {
                throw new ArgumentNullException(nameof(updateCommentModel.Message));
            }

            GetCommentWithWithChecking(updateCommentModel.Id);

            var newValues = new ExpandoObject();

            newValues.TryAdd(nameof(Comment.Message), updateCommentModel.Message);

            if (!string.IsNullOrEmpty(updateCommentModel.Description))
            {
                newValues.TryAdd(nameof(Comment.Description), updateCommentModel.Description);
            }

            CommentsDataProvider.Update(updateCommentModel.Id, newValues);
        }

        /// <summary>
        /// Increment <see cref="Comment.AppearanceCount"/> property of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        /// <exception cref="ArgumentNullException">Parameter commentId is default</exception>
        /// <exception cref="EntityNotFoundException">Comment not found</exception>
        public void Increment(Guid commentId)
        {
            var entity = GetCommentWithWithChecking(commentId);

            var newValues = new ExpandoObject();

            newValues.TryAdd(nameof(Comment.AppearanceCount), entity.AppearanceCount + 1);

            CommentsDataProvider.Update(commentId, newValues);
        }

        /// <summary>
        /// Delete comments by specifying their identifiers
        /// </summary>
        /// <param name="commentIds">Array of comment identifiers</param>
        /// <exception cref="ArgumentNullException">Parameter commentIds is null</exception>
        public void Delete(params Guid[] commentIds)
        {
            if (commentIds == null)
            {
                throw new ArgumentNullException(nameof(commentIds));
            }

            commentIds = commentIds.Where(x => x != default).ToArray();

            if (commentIds.Any())
            {
                CommentsDataProvider.Delete(commentIds);
            }
        }

        #region Not public API

        /// <summary>
        /// Get comment with validation by specified identifier
        /// </summary>
        /// <param name="commentId">Comment identifier value</param>
        /// <exception cref="ArgumentNullException">Parameter commentId is default</exception>
        /// <exception cref="EntityNotFoundException">Comment not found</exception>
        /// <returns>Found comment</returns>
        private Comment GetCommentWithWithChecking(Guid commentId)
        {
            if (commentId == default)
            {
                throw new ArgumentNullException(nameof(commentId));
            }

            var entity = CommentsDataProvider.Get(commentId);

            if (entity == null)
            {
                throw new EntityNotFoundException(nameof(Comment), commentId);
            }

            return entity;
        }

        #endregion
    }
}
