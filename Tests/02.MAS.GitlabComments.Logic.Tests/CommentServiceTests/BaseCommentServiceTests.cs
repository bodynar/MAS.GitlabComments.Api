namespace MAS.GitlabComments.Logic.Tests.CommentServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Exceptions;
    using MAS.GitlabComments.DataAccess.Select;
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.Logic.Services.Implementations;

    using Moq;

    using Xunit;

    /// <summary>
    /// Base class for <see cref="CommentService"/> tests
    /// </summary>
    public abstract class BaseCommentServiceTests
    {
        /// <summary>
        /// Instance of <see cref="CommentService"/> for tests
        /// </summary>
        protected CommentService TestedService { get; }

        /// <summary>
        /// Instance of <see cref="Comment"/> which will be returned by <see cref="IDataProvider{TEntity}"/> in <see cref="IDataProvider{TEntity}.Get"/> method
        /// </summary>
        protected Comment ReturnedTestedComment { get; private set; }

        /// <summary>
        /// Instance of <see cref="CommentModel"/> which will be returned by <see cref="IDataProvider{TEntity}"/> in <see cref="IDataProvider{TEntity}.Select{TProjection}(SelectConfiguration)"/> method 
        /// for getting all comments
        /// </summary>
        protected CommentModel ProjectedTestComment { get; private set; }

        /// <summary>
        /// Instance of <see cref="IncompleteCommentData"/> which will be returned by <see cref="IDataProvider{TEntity}"/> in <see cref="IDataProvider{TEntity}.Select{TProjection}(SelectConfiguration)"/> method
        /// for getting incomplete comments
        /// </summary>
        protected IncompleteCommentData ProjectedIncompleteTestComment { get; private set; }

        /// <summary>
        /// Instance of StoryRecord which was added last
        /// </summary>
        protected StoryRecord LastAddedStoryRecord { get; private set; }

        /// <summary>
        /// Instance of Comment which was added last
        /// </summary>
        protected Comment LastAddedComment { get; private set; }

        /// <summary>
        /// Select configuration of last <see cref="IDataProvider{TEntity}.Select{TProjection}(SelectConfiguration)"/> call
        /// </summary>
        protected SelectConfiguration LastSelectConfig { get; private set; }

        /// <summary>
        /// Tested comment number template
        /// </summary>
        protected string CommentNumberTemplate { get; } = "!{0:00}";

        /// <summary>
        /// Value provided by mock of <see cref="ISystemVariableProvider.GetValue{TValue}(string)"/> for test case
        /// </summary>
        protected int IntVariableValue { get; } = 10;

        /// <summary>
        /// Last called command back-field
        /// </summary>
        private KeyValuePair<string, IEnumerable<object>>? lastCommand;

        /// <summary>
        /// Last called command of <see cref="IDataProvider{TEntity}"/> - pair of command name and arguments
        /// </summary>
        protected KeyValuePair<string, IEnumerable<object>>? LastCommand
        {
            get
            {
                if (!lastCommand.HasValue)
                {
                    return null;
                }

                var copy = new KeyValuePair<string, IEnumerable<object>>(lastCommand.Value.Key, lastCommand.Value.Value);

                lastCommand = null;

                return copy;
            }
        }

        /// <summary>
        /// Initializing <see cref="BaseCommentServiceTests"/> with setup'n all required environment
        /// </summary>
        public BaseCommentServiceTests()
        {
            var commentsProvider = GetMockCommentDataProvider();
            var storyRecordsProvider = GetMockStoryRecordDataProvider();
            var appSettings = GetMockAppSettings();
            var sysVariableProvider = GetMockSysVariableProvider();

            TestedService = new CommentService(
                commentsProvider,
                storyRecordsProvider,
                appSettings,
                sysVariableProvider
            );

            ReturnedTestedComment = new Comment
            {
                Id = Guid.Empty,
                AppearanceCount = 10,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                Message = nameof(Comment.Message),
                Number = "TEST_0001",
                Description = nameof(Comment.Description),
                CommentWithLinkToRule = nameof(Comment.CommentWithLinkToRule),
            };

            ProjectedTestComment = new CommentModel
            {
                Id = Guid.Empty,
                AppearanceCount = 10,
                Message = nameof(CommentModel.Message),
                Description = nameof(CommentModel.Description),
                CommentWithLinkToRule = nameof(CommentModel.CommentWithLinkToRule),
                Number = "TEST_0001",
            };

            ProjectedIncompleteTestComment = new IncompleteCommentData
            {
                Id = Guid.Empty,
                AppearanceCount = 10,
            };
        }

        /// <summary>
        /// Configure mock object of Comment data provider for comment service
        /// </summary>
        /// <returns>Configured mock object of <see cref="IDataProvider{TEntity}"/></returns>
        private IDataProvider<Comment> GetMockCommentDataProvider()
        {
            var mockDataProvider = new Mock<IDataProvider<Comment>>();

            mockDataProvider
                .Setup(x => x.Add(It.IsAny<Comment>()))
                .Callback<Comment>((comment) =>
                {
                    LastAddedComment = comment;
                    lastCommand = new KeyValuePair<string, IEnumerable<object>>(nameof(mockDataProvider.Object.Add), Array.Empty<object>());
                })
                .Returns(() => Guid.Empty);

            mockDataProvider // custom case
                .Setup(x => x.Select<CommentModel>(It.IsAny<SelectConfiguration>()))
                .Callback<SelectConfiguration>(config => LastSelectConfig = config)
                .Returns(() => new[] { ProjectedTestComment });

            mockDataProvider // custom case
                .Setup(x => x.Select<IncompleteCommentData>(It.IsAny<SelectConfiguration>()))
                .Callback<SelectConfiguration>(config => LastSelectConfig = config)
                .Returns(() => new[] { ProjectedIncompleteTestComment });

            mockDataProvider
                .Setup(x => x.Get())
                .Returns(() => new[] { ReturnedTestedComment });

            mockDataProvider
                .Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns(() => ReturnedTestedComment);

            mockDataProvider
                .Setup(x => x.Update(It.IsAny<Guid>(), It.IsAny<IDictionary<string, object>>()))
                .Callback<Guid, IDictionary<string, object>>((id, data) =>
                {
                    lastCommand = new KeyValuePair<string, IEnumerable<object>>(nameof(mockDataProvider.Object.Update), new object[] { id, data });
                });

            mockDataProvider
                .Setup(x => x.Delete(It.IsAny<Guid[]>()))
                .Callback<Guid[]>(id =>
                {
                    lastCommand = new KeyValuePair<string, IEnumerable<object>>(nameof(mockDataProvider.Object.Delete), new object[] { id });
                });

            return mockDataProvider.Object;
        }

        /// <summary>
        /// Configure mock object of StoryRecord data provider for comment service
        /// </summary>
        /// <returns>Configured mock object of <see cref="IDataProvider{TEntity}"/></returns>
        private IDataProvider<StoryRecord> GetMockStoryRecordDataProvider()
        {
            var mockDataProvider = new Mock<IDataProvider<StoryRecord>>();

            mockDataProvider
                .Setup(x => x.Add(It.IsAny<StoryRecord>()))
                .Callback<StoryRecord>(x =>
                {
                    LastAddedStoryRecord = x;
                });


            return mockDataProvider.Object;
        }

        /// <summary>
        /// Configure mock object of Application settings container for comment service
        /// </summary>
        /// <returns>Configured mock object of <see cref="IApplicationSettings"/></returns>
        private IApplicationSettings GetMockAppSettings()
        {
            var mock = new Mock<IApplicationSettings>();

            mock
                .SetupGet(x => x.CommentNumberTemplate)
                .Returns(() => CommentNumberTemplate);

            return mock.Object;
        }

        /// <summary>
        /// Configure mock object of System variables provider for comment service
        /// </summary>
        /// <returns>Configured mock object of <see cref="ISystemVariableProvider"/></returns>
        private ISystemVariableProvider GetMockSysVariableProvider()
        {
            var mock = new Mock<ISystemVariableProvider>();

            mock
                .Setup(x => x.GetValue<int>(It.IsAny<string>()))
                .Returns(() => IntVariableValue);

            return mock.Object;
        }

        /// <summary>
        /// Assert for last <see cref="IDataProvider{TEntity}"/> called command
        /// </summary>
        /// <param name="serviceAction">Tested service action</param>
        /// <param name="expectedCommandName">Expected called command name</param>
        /// <param name="expectedCommandArguments">Expected arguments of called command</param>
        protected void ShouldExecuteCommand(Action serviceAction, string expectedCommandName, IEnumerable<object> expectedCommandArguments)
        {
            int expectedArgumentsCount = expectedCommandArguments.Count();

            serviceAction.Invoke();
            KeyValuePair<string, IEnumerable<object>>? lastCommand = LastCommand;

            Assert.NotNull(lastCommand);

            var lastCommandAsKeyValuePair = lastCommand.Value;

            Assert.Equal(expectedCommandName, lastCommandAsKeyValuePair.Key);
            Assert.Equal(expectedArgumentsCount, lastCommandAsKeyValuePair.Value.Count());

            for (int i = 0; i < expectedArgumentsCount; i++)
            {
                var expectedArgument = expectedCommandArguments.ElementAt(i);
                var actualArgument = lastCommandAsKeyValuePair.Value.ElementAt(i);

                Assert.NotNull(actualArgument);
                Assert.Equal(actualArgument, expectedArgument);
            }
        }

        #region Common tests

        /// <summary>
        /// Common tests for same execution in several methods.
        /// Checks parameter validation <see cref="CommentService.GetCommentWithWithChecking"/>
        /// </summary>
        /// <param name="serviceAction">Tested service action</param>
        protected static void ShouldThrowArgumentNullExceptionWhenCommentIdIsDefaultInternal(Action serviceAction)
        {
            var exception =
                Record.Exception(
                    () => serviceAction.Invoke()
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        /// <summary>
        /// Common tests for same execution in several methods.
        /// Checks gathered entity validation <see cref="CommentService.GetCommentWithWithChecking"/>
        /// </summary>
        /// <param name="serviceAction">Tested service action</param>
        /// <param name="expectedErrorMessage">Expected exception message</param>
        protected void ShouldThrowEntityNotFoundExceptionWhenEntityNotFoundByIdInternal(Action serviceAction, string expectedErrorMessage)
        {
            ReturnedTestedComment = null;

            var exception =
                Record.Exception(
                    () => serviceAction.Invoke()
                );

            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundException>(exception);
            Assert.Equal(expectedErrorMessage, exception.Message);
        }

        #endregion
    }
}
