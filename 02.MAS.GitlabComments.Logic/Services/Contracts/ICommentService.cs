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
        /// <returns>Identifier value of new created comment</returns>
        public Guid Add(AddCommentModel addCommentModel);

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <returns>All comments</returns>
        public IEnumerable<CommentModel> Get();

        /// <summary>
        /// Increment <see cref="Comment.AppearanceCount"/> property of specified comment
        /// </summary>
        /// <param name="commentId">Comment identifier</param>
        public void Increment(Guid commentId);

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
        /// Add unique constraint for Number column for comment in database
        /// </summary>
        [Obsolete("v1.3 | Will be removed in v1.4")]
        public void MakeNumberColumnUnique();
    }
}
