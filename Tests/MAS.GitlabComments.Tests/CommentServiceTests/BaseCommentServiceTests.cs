namespace MAS.GitlabComments.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Exceptions;
    using MAS.GitlabComments.Models.Database;
    using MAS.GitlabComments.Services;
    using MAS.GitlabComments.Services.Implementations;

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
        /// Instance of <see cref="Comment"/> which will be returned by <see cref="IDataProvider{TEntity}"/> in <see cref="IDataProvider{TEntity}.Get"/> methods
        /// </summary>
        protected Comment ReturnedTestedComment { get; private set; }

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
            var dataProvider = GetMockDataProvider();
            TestedService = new CommentService(dataProvider);

            ReturnedTestedComment = new Comment
            {
                Id = Guid.Empty,
                AppearanceCount = 10,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                Message = nameof(Comment.Message),
                Description = nameof(Comment.Description),
            };
        }

        /// <summary>
        /// Configure mock object of data provider for comment service
        /// </summary>
        /// <returns>Configured mock object of <see cref="IDataProvider{TEntity}"/></returns>
        private IDataProvider<Comment> GetMockDataProvider()
        {
            var mockDataProvider = new Mock<IDataProvider<Comment>>();
            Action emptyCallback = () => { };

            mockDataProvider
                .Setup(x => x.Add(It.IsAny<Comment>()))
                .Callback(emptyCallback);

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

                    emptyCallback.Invoke();
                });

            mockDataProvider
                .Setup(x => x.Delete(It.IsAny<Guid[]>()))
                .Callback<Guid[]>(id =>
                {
                    lastCommand = new KeyValuePair<string, IEnumerable<object>>(nameof(mockDataProvider.Object.Delete), new object[] { id });

                    emptyCallback.Invoke();
                });

            return mockDataProvider.Object;
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
        protected void ShouldThrowArgumentNullExceptionWhenCommentIdIsDefaultInternal(Action serviceAction)
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
