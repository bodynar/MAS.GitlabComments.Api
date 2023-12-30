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

        #region Data provider mock members

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
        /// Result of <see cref="ICommentService.GetIncomplete"/> for manual mocking
        /// </summary>
        protected IEnumerable<IncompleteCommentData> ManualIncompleteData { get; set; }

        /// <summary>
        /// Use <see cref="ManualIncompleteData"/> for <see cref="ICommentService.GetIncomplete"/>
        /// </summary>
        protected bool UseManualIncompleteData { get; set; }

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
        /// Created entity id returned from <see cref="IDataProvider{TEntity}.Add(TEntity)"/>
        /// </summary>
        protected Guid ReturnedCreatedCommentId { get; set; } = Guid.Empty;

        /// <summary>
        /// Last called command of <see cref="IDataProvider{TEntity}"/> - pair of command name and arguments
        /// </summary>
        protected KeyValuePair<string, IEnumerable<object>>? LastCommand { get; private set; }

        #endregion

        #region System variable service

        /// <summary>
        /// Was <see cref="ISystemVariableProvider.Set{TValue}(string, TValue)"/> called for "LastCommentNumber" variable
        /// </summary>
        protected bool IsSetNumberVariableCalled { get; private set; }

        /// <summary>
        /// Value provided by mock of <see cref="ISystemVariableProvider.GetValue{TValue}(string)"/> for test case
        /// </summary>
        protected int IntVariableValue { get; } = 10;

        /// <summary>
        /// Last value of <see cref="ISystemVariableProvider.Set{TValue}(string, TValue)"/> for last comment number variable
        /// </summary>
        protected int? LastCommentNumber { get; private set; }

        /// <summary>
        /// Model got from <see cref="ISystemVariableProvider.Get(string)"/>
        /// </summary>
        protected SysVariable ReturnedSysVariable { get; set; }

        /// <summary>
        /// Value, read from second argument in call of <see cref="ISystemVariableProvider.Set{TValue}(string, TValue)"/>
        /// </summary>
        protected bool IsChangeNumberUniqueValue { get; private set; }

        #endregion

        /// <summary>
        /// Flag, what determine that <see cref="ITempDatabaseModifier.ApplyModifications"/> was called
        /// </summary>
        protected bool IsTempModifierWasCalled { get; private set; }

        /// <summary>
        /// Tested comment number template
        /// </summary>
        protected string CommentNumberTemplate { get; } = "!{0:00}";

        /// <summary>
        /// Queue of arguments from call <see cref="IDataProvider{TEntity}.Update(Guid, IDictionary{string, object})"/>
        /// </summary>
        protected IEnumerable<KeyValuePair<Guid, IDictionary<string, object>>> UpdateDataProviderArguments { get; private set; }
            = Enumerable.Empty<KeyValuePair<Guid, IDictionary<string, object>>>();

        /// <summary>
        /// Initializing <see cref="BaseCommentServiceTests"/> with setup'n all required environment
        /// </summary>
        public BaseCommentServiceTests()
        {
            var commentsProvider = GetMockCommentDataProvider();
            var storyRecordsProvider = GetMockStoryRecordDataProvider();
            var appSettings = GetMockAppSettings();
            var sysVariableProvider = GetMockSysVariableProvider();
            var tempModified = GetTempDatabaseModifier();

            TestedService = new CommentService(
                commentsProvider,
                storyRecordsProvider,
                appSettings,
                sysVariableProvider,
                tempModified
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
                    LastCommand = new KeyValuePair<string, IEnumerable<object>>(nameof(mockDataProvider.Object.Add), Array.Empty<object>());
                })
                .Returns(() => ReturnedCreatedCommentId);

            mockDataProvider // custom case
                .Setup(x => x.Select<CommentModel>(It.IsAny<SelectConfiguration>()))
                .Callback<SelectConfiguration>(config => LastSelectConfig = config)
                .Returns(() => new[] { ProjectedTestComment });

            mockDataProvider // custom case
                .Setup(x => x.Select<IncompleteCommentData>(It.IsAny<SelectConfiguration>()))
                .Callback<SelectConfiguration>(config => LastSelectConfig = config)
                .Returns(() => UseManualIncompleteData ? ManualIncompleteData : new[] { ProjectedIncompleteTestComment });

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
                    LastCommand = new KeyValuePair<string, IEnumerable<object>>(nameof(mockDataProvider.Object.Update), new object[] { id, data });
                    UpdateDataProviderArguments = UpdateDataProviderArguments.Union(new[] {
                        new KeyValuePair<Guid, IDictionary<string, object>>(
                            id, data
                        )
                    });
                });

            mockDataProvider
                .Setup(x => x.Delete(It.IsAny<Guid[]>()))
                .Callback<Guid[]>(id =>
                {
                    LastCommand = new KeyValuePair<string, IEnumerable<object>>(nameof(mockDataProvider.Object.Delete), new object[] { id });
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

            mock
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(() => ReturnedSysVariable);

            mock
                .Setup(x => x.Set("LastCommentNumber", It.IsAny<int>()))
                .Callback<string, int>((_, value) =>
                {
                    IsSetNumberVariableCalled = true;
                    LastCommentNumber = value;
                });

            mock
                .Setup(x => x.Set("IsChangeNumberUnique", It.IsAny<bool>()))
                .Callback<string, bool>((_, value) =>
                {
                    IsChangeNumberUniqueValue = value;
                });

            return mock.Object;
        }

        /// <summary>
        /// Configure mock object of <see cref="ITempDatabaseModifier"/> for comment service
        /// </summary>
        /// <returns>Configured mock object of <see cref="ITempDatabaseModifier"/></returns>
        private ITempDatabaseModifier GetTempDatabaseModifier()
        {
            var mock = new Mock<ITempDatabaseModifier>();

            mock
                .Setup(x => x.ApplyModifications())
                .Callback(() => IsTempModifierWasCalled = true)
            ;

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

            Assert.NotNull(LastCommand);

            var lastCommandAsKeyValuePair = LastCommand.Value;

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

        /// <summary>
        /// Assert for last <see cref="IDataProvider{TEntity}"/> called command
        /// </summary>
        /// <param name="serviceAction">Tested service action</param>
        /// <param name="expectedCommandName">Expected called command name</param>
        /// <param name="expectedCommandArguments">Expected arguments of called command</param>
        protected TResult ShouldExecuteCommand<TResult>(Func<TResult> serviceAction, string expectedCommandName, IEnumerable<object> expectedCommandArguments)
        {
            int expectedArgumentsCount = expectedCommandArguments.Count();

            var result = serviceAction.Invoke();

            Assert.NotNull(LastCommand);

            var lastCommandAsKeyValuePair = LastCommand.Value;

            Assert.Equal(expectedCommandName, lastCommandAsKeyValuePair.Key);
            Assert.Equal(expectedArgumentsCount, lastCommandAsKeyValuePair.Value.Count());

            for (int i = 0; i < expectedArgumentsCount; i++)
            {
                var expectedArgument = expectedCommandArguments.ElementAt(i);
                var actualArgument = lastCommandAsKeyValuePair.Value.ElementAt(i);

                Assert.NotNull(actualArgument);
                Assert.Equal(actualArgument, expectedArgument);
            }

            return result;
        }

        #region Common tests

        /// <summary>
        /// Common tests for same execution in several methods.
        /// Checks parameter validation <see cref="CommentService.GetCommentWithWithChecking"/>
        /// </summary>
        /// <param name="serviceAction">Tested service action</param>
        protected static void ShouldThrowArgumentNullException_WhenCommentIdIsDefaultInternal(Action serviceAction)
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
        protected void ShouldThrowEntityNotFoundException_WhenEntityNotFoundByIdInternal(Action serviceAction, string expectedErrorMessage)
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
