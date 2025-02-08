namespace MAS.GitlabComments.Logic.Tests.DataImporterTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.Logic.Services.Implementations;

    using Moq;

    /// <summary>
    /// Base class for <see cref="DataImporter"/> tests
    /// </summary>
    public abstract class BaseDataImporterTests
    {
        /// <summary>
        /// Instance of <see cref="DataImporter"/> for tests
        /// </summary>
        protected DataImporter TestedService { get; }

        /// <summary>
        /// Pack of <see cref="Comment"/> which will be returned from <see cref="IDataProvider{TEntity}.Get"/>
        /// </summary>
        protected IEnumerable<Comment> ReturnedComments { get; set; }

        /// <summary>
        /// Pack of <see cref="StoryRecord"/> which will be returned from <see cref="IDataProvider{TEntity}.Get"/>
        /// </summary>
        protected IEnumerable<StoryRecord> ReturnedStoryRecords { get; set; }

        /// <summary>
        /// Pack of <see cref="RetractionToken"/> which will be returned from <see cref="IDataProvider{TEntity}.Get"/>
        /// </summary>
        protected IEnumerable<RetractionToken> ReturnedTokens { get; set; }

        /// <summary>
        /// Instance of <see cref="Comment"/> added via <see cref="ICommentService.Add(AddCommentModel)"/>
        /// </summary>
        protected AddCommentModel AddedComment { get; private set; }

        /// <summary>
        /// Instance of <see cref="StoryRecord"/> added via <see cref="IDataProvider{TEntity}.Add(TEntity)"/>
        /// </summary>
        protected StoryRecord AddedStoryRecord { get; private set; }

        /// <summary>
        /// Instance of <see cref="RetractionToken"/> added via <see cref="IDataProvider{TEntity}.Add(TEntity)"/>
        /// </summary>
        protected RetractionToken AddedToken { get; private set; }

        /// <summary>
        /// Is <see cref="ICommentService.RecalculateLastNumber"/> was called during tests
        /// </summary>
        protected bool IsRecalculateLastNumberCalled { get; private set; }

        /// <summary>
        /// Initialization of <see cref="BaseDataImporterTests"/>
        /// </summary>
        public BaseDataImporterTests()
        {
            var (cp, sp, rp, cs) = GetServiceDependencies();
            TestedService = new DataImporter(cp, sp, rp, cs);
        }

        /// <summary>
        /// Creating mocks of <see cref="DataImporter"/> dependencies
        /// </summary>
        private (IDataProvider<Comment>, IDataProvider<StoryRecord>, IDataProvider<RetractionToken>, ICommentService) GetServiceDependencies()
        {
            var mockComments = new Mock<IDataProvider<Comment>>();

            mockComments
                .Setup(x => x.Get())
                .Returns(() => {
                    return ReturnedComments;
                });

            var mockStoryRecords = new Mock<IDataProvider<StoryRecord>>();

            mockStoryRecords
                .Setup(x => x.Get())
                .Returns(() => ReturnedStoryRecords);

            mockStoryRecords
                .Setup(x => x.Add(It.IsAny<StoryRecord>()))
                .Callback<StoryRecord>(x => {
                    AddedStoryRecord = x;
                });

            var mockTokens = new Mock<IDataProvider<RetractionToken>>();

            mockTokens
                .Setup(x => x.Get())
                .Returns(() => ReturnedTokens);

            mockTokens
                .Setup(x => x.Add(It.IsAny<RetractionToken>()))
                .Callback<RetractionToken>(x => {
                    AddedToken = x;
                });

            var mockCommentsService = new Mock<ICommentService>();

            mockCommentsService
                .Setup(x => x.RecalculateLastNumber())
                .Callback(() => IsRecalculateLastNumberCalled = true);

            mockCommentsService
                .Setup(x => x.Add(It.IsAny<AddCommentModel>()))
                .Callback<AddCommentModel>(x => {
                    AddedComment = x;
                })
                .Returns(() => new NewComment { Id = Guid.Empty, Number = string.Empty });

            return (
                mockComments.Object,
                mockStoryRecords.Object,
                mockTokens.Object,
                mockCommentsService.Object
            );
        }
    }
}
