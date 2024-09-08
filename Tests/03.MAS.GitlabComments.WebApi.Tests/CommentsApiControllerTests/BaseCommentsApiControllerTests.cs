namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using System;
    using System.Linq;

    using MAS.GitlabComments.WebApi.Controllers;
    using MAS.GitlabComments.WebApi.Models;
    using MAS.GitlabComments.Logic.Services;

    using Microsoft.Extensions.Logging;

    using Moq;

    using Xunit;
    using MAS.GitlabComments.Logic.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for <see cref="CommentsApiController"/> tests
    /// </summary>
    public abstract class BaseCommentsApiControllerTests
    {
        /// <summary>
        /// Instance of <see cref="CommentsApiController"/> for tests
        /// </summary>
        protected CommentsApiController TestedController { get; }

        /// <summary>
        /// Should exception be thrown during comment service execution
        /// </summary>
        protected bool ShouldThrowExceptionDuringExecution { get; private set; }

        /// <summary>
        /// Exception message which will be thrown if <see cref="ShouldThrowExceptionDuringExecution"/> is true
        /// </summary>
        private const string ExceptionDuringExecutionText = "ExceptionTestMessage";

        /// <summary>
        /// Initializing <see cref="BaseCommentsApiControllerTests"/> with setup'n all required environment
        /// </summary>
        protected BaseCommentsApiControllerTests()
        {
            var (logger, commentService) = GetDependencies();
            TestedController = new CommentsApiController(logger, commentService);
        }

        #region Private members

        /// <summary>
        /// Configure mock objects of required dependencies for tested class
        /// </summary>
        /// <returns>Pair of configured mock object of <see cref="ILogger{TCategoryName}"/> and <see cref="ICommentService"/></returns>
        private (ILogger<CommentsApiController>, ICommentService) GetDependencies()
        {
            var mockLogger = new Mock<ILogger<CommentsApiController>>();

            var mockCommentsService = new Mock<ICommentService>();

            Action commentServiceExceptionCallback = () =>
            {
                if (ShouldThrowExceptionDuringExecution)
                {
                    throw new Exception(ExceptionDuringExecutionText);
                }
            };

            mockCommentsService
                .Setup(x => x.Add(It.IsAny<AddCommentModel>()))
                .Callback(commentServiceExceptionCallback)
                .Returns(default(NewComment));

            mockCommentsService
                .Setup(x => x.Get())
                .Callback(commentServiceExceptionCallback)
                .Returns(Enumerable.Empty<CommentModel>());

            mockCommentsService
                .Setup(x => x.Increment(It.IsAny<Guid>()))
                .Callback(commentServiceExceptionCallback);

            mockCommentsService
                .Setup(x => x.Update(It.IsAny<UpdateCommentModel>()))
                .Callback(commentServiceExceptionCallback);

            mockCommentsService
                .Setup(x => x.Delete(It.IsAny<Guid>()))
                .Callback(commentServiceExceptionCallback);

            mockCommentsService
                .Setup(x => x.GetIncomplete())
                .Callback(commentServiceExceptionCallback)
                .Returns(Enumerable.Empty<IncompleteCommentData>());

            mockCommentsService
                .Setup(x => x.UpdateIncomplete())
                .Callback(commentServiceExceptionCallback);

            mockCommentsService
                .Setup(x => x.MakeNumberColumnUnique())
                .Callback(commentServiceExceptionCallback);

            mockCommentsService
                .Setup(x => x.Merge(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<MergeCommentUpdateModel>()))
                .Callback(commentServiceExceptionCallback);

            return (mockLogger.Object, mockCommentsService.Object);
        }

        #endregion

        /// <summary>
        /// Testing exception-thrown case of execution
        /// </summary>
        /// <typeparam name="TResult">Result of controller action</typeparam>
        /// <param name="action">Controller action</param>
        protected void AssertBaseServiceResultError<TResult>(Func<TResult> action)
            where TResult : BaseServiceResult
        {
            ShouldThrowExceptionDuringExecution = true;

            TResult baseServiceResult = action.Invoke();

            Assert.NotNull(baseServiceResult);
            Assert.False(baseServiceResult.IsSuccess);
            Assert.Equal(ExceptionDuringExecutionText, baseServiceResult.ErrorMessage);
        }
    }
}
