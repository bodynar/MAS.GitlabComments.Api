namespace MAS.GitlabComments.WebApi.Models
{
    using System;

    using MAS.GitlabComments.Logic.Models;

    /// <summary>
    /// Data for merging comment operation
    /// </summary>
    public class MergeCommentModel
    {
        /// <summary>
        /// Identifier of source comment
        /// </summary>
        public Guid SourceCommentId { get; set; }

        /// <summary>
        /// Identifier of target comment
        /// </summary>
        public Guid TargetCommentId { get; set; }

        /// <summary>
        /// Target comment new values
        /// </summary>
        public UpdateCommentData NewTargetValues { get; set; }

        /// <summary>
        /// Data for updating target comment
        /// </summary>
        public class UpdateCommentData
        {
            /// <summary>
            /// Message text
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// Comment with link to rules
            /// </summary>
            public string CommentWithLinkToRule { get; set; }

            /// <summary>
            /// Explanation description
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Map to logic DTO model
            /// </summary>
            /// <returns>Instance of <see cref="MergeCommentUpdateModel"/></returns>
            public MergeCommentUpdateModel ToMergeModel()
            {
                return new MergeCommentUpdateModel
                {
                    Message = Message,
                    Description = Description,
                    CommentWithLinkToRule = CommentWithLinkToRule,
                };
            }
        }
    }
}
