namespace MAS.GitlabComments.Logic.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.Logic.Models.Import;

    /// <summary>
    /// Implementation of <see cref="IDataImporter"/>
    /// </summary>
    public class DataImporter : IDataImporter
    {
        /// <summary>
        /// Data provider for <see cref="Comment"/>
        /// </summary>
        private IDataProvider<Comment> CommentDataProvider { get; }

        /// <summary>
        /// Data provider for <see cref="StoryRecord"/>
        /// </summary>
        private IDataProvider<StoryRecord> StoryRecordDataProvider { get; }

        /// <summary>
        /// Data provider for <see cref="RetractionToken"/>
        /// </summary>
        private IDataProvider<RetractionToken> TokenDataProvider { get; }

        /// <inheritdoc cref="ICommentService"/>
        private ICommentService CommentService { get; }

        /// <summary>
        /// Initializing <see cref="DataImporter"/>
        /// </summary>
        /// <param name="commentDataProvider">Instance of <see cref="IDataProvider{T}"/> for managing comments</param>
        /// <param name="storyRecordDataProvider">Instance of <see cref="IDataProvider{T}"/> for managing story records</param>
        /// <param name="tokenDataProvider">Instance of <see cref="IDataProvider{T}"/> for managing retraction tokens</param>
        /// <param name="commentService">Instance of <see cref="ICommentService"/></param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="commentDataProvider"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="storyRecordDataProvider"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="tokenDataProvider"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="commentService"/> is <see langword="null"/></exception>
        public DataImporter(
            IDataProvider<Comment> commentDataProvider,
            IDataProvider<StoryRecord> storyRecordDataProvider,
            IDataProvider<RetractionToken> tokenDataProvider,
            ICommentService commentService
        )
        {
            CommentDataProvider = commentDataProvider ?? throw new ArgumentNullException(nameof(commentDataProvider));
            StoryRecordDataProvider = storyRecordDataProvider ?? throw new ArgumentNullException(nameof(storyRecordDataProvider));
            TokenDataProvider = tokenDataProvider ?? throw new ArgumentNullException(nameof(tokenDataProvider));
            CommentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }

        /// <inheritdoc />
        public IEnumerable<CommentExportModel> ExportAppData()
        {
            var result = new List<CommentExportModel>();

            var comments = CommentDataProvider.Get();
            var storyRecords = StoryRecordDataProvider.Get();
            var tokens = TokenDataProvider.Get();

            foreach (var comment in comments)
            {
                var relatedTokens = tokens.Where(x => x.CommentId == comment.Id);
                var relatedStoryRecords = storyRecords.Where(x => x.CommentId == comment.Id);

                var commentExportModel = new CommentExportModel
                {
                    Message = comment.Message,
                    AppearanceCount = comment.AppearanceCount,
                    CommentWithLinkToRule = comment.CommentWithLinkToRule,
                    Description = comment.Description,

                    StoryRecords =
                        relatedStoryRecords.Select(x =>
                            new StoryRecordExportModel
                            {
                                CreatedOn = x.CreatedOn,
                                IsRetracted = x.IsRetracted,

                                RetractionTokens = relatedTokens.Select(t => new RetractionTokenExportModel
                                {
                                    ValidUntil = t.ValidUntil
                                })
                            }
                        ),
                };

                result.Add(commentExportModel);
            }

            return result;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="comments"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="comments"/> contains <see langword="null"/> value</exception>
        /// <exception cref="ArgumentException"><paramref name="comments"/> is empty</exception>
        public void ImportAppData(IEnumerable<CommentExportModel> comments)
        {
            if (comments == null)
            {
                throw new ArgumentNullException(nameof(comments));
            }

            if (comments.Contains(null))
            {
                throw new ArgumentException("Exported data contains 'null' value", nameof(comments));
            }

            if (!comments.Any())
            {
                throw new ArgumentException("No data to import");
            }

            foreach (var comment in comments)
            {
                var data = CommentService.Add(new AddCommentModel
                {
                    IsImportAction = true,
                    Message = comment.Message,
                    AppearanceCount = comment.AppearanceCount,
                    CommentWithLinkToRule = comment.CommentWithLinkToRule,
                    Description = comment.Description,
                });

                foreach (var storyRecord in comment.StoryRecords)
                {
                    var storyRecordId = StoryRecordDataProvider.Add(new StoryRecord
                    {
                        CreatedOn = new DateTime(storyRecord.CreatedOn.Ticks, DateTimeKind.Utc),
                        IsRetracted = storyRecord.IsRetracted,

                        CommentId = data.Id,
                    });

                    foreach (var token in storyRecord.RetractionTokens)
                    {
                        TokenDataProvider.Add(new RetractionToken
                        {
                            CommentId = data.Id,
                            ValidUntil = new DateTime(token.ValidUntil.Ticks, DateTimeKind.Utc),
                            StoryRecordId = storyRecordId,
                        });
                    }
                }
            }

            CommentService.RecalculateLastNumber();
        }
    }
}
