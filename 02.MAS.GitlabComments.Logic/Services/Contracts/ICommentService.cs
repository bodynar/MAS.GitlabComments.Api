namespace MAS.GitlabComments.Logic.Services
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.Logic.Models;

    /// <summary>
    /// Service for managing <see cref="Comment"/>
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// Add comment by specified values
        /// </summary>
        /// <param name="addCommentModel">Comment values</param>
        /// <returns>Created comment data</returns>
        public NewComment Add(AddCommentModel addCommentModel);

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <returns>All comments</returns>
        public IEnumerable<CommentModel> Get();

        /// <summary>
        /// Increment <see cref="Comment.AppearanceCount"/> property of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        /// <returns>Identifier of retraction token to perform an retraction operation</returns>
        public Guid Increment(Guid commentId);

        /// <summary>
        /// Update specified comment by values
        /// </summary>
        /// <param name="updateCommentModel">Comment new values</param>
        public void Update(UpdateCommentModel updateCommentModel);

        /// <summary>
        /// Delete comments by specified identifiers
        /// </summary>
        /// <param name="commentIds">Array of comment identifiers</param>
        public void Delete(params Guid[] commentIds);

        /// <summary>
        /// Get all comments that incomplete
        /// <para>
        ///     - Does not have a number
        /// </para>
        /// </summary>
        /// <returns>Pack of comments</returns>
        public IEnumerable<IncompleteCommentData> GetIncomplete();

        /// <summary>
        /// Update incomplete comments to satisfy all rules
        /// <para>
        ///     - Set number for those which doesn't have one
        /// </para>
        /// </summary>
        public void UpdateIncomplete();

        /// <summary>
        /// Merge two comments
        /// <para>
        ///     Source comment will be removed, target comment will be updated
        /// </para>
        /// </summary>
        /// <param name="sourceCommentId">Source comment identifier</param>
        /// <param name="targetCommentId">Target comment identifier</param>
        /// <param name="newTargetValues">Target comment updated values</param>
        public void Merge(Guid sourceCommentId, Guid targetCommentId, MergeCommentUpdateModel newTargetValues);

        /// <summary>
        /// Re-calculate last comment number system variable
        /// </summary>
        public void RecalculateLastNumber();
    }
}
