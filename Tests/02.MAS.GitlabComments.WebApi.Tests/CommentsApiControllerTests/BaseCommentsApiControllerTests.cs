namespace MAS.GitlabComments.Tests.CommentsApiControllerTests
{
    using System;
    using System.Linq;

    using MAS.GitlabComments.Controllers;
    using MAS.GitlabComments.Models;
    using MAS.GitlabComments.Services;

    using Microsoft.Extensions.Logging;

    using Moq;

    using Xunit;

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
        protected bool ShouldThrowExceptionDuringExection { get; set; }

        /// <summary>
        /// Exception message which will be thrown if <see cref="ShouldThrowExceptionDuringExection"/> is true
        /// </summary>
        protected string ExceptionDuringExectionText { get; set; }

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
        /// Configure mock object of data provider for comment service
        /// </summary>
        /// <returns>Pair of configured mock object of <see cref="ILogger{TCategoryName}"/> and <see cref="ICommentService"/></returns>
        private (ILogger<CommentsApiController>, ICommentService) GetDependencies()
        {
            var mockLogger = new Mock<ILogger<CommentsApiController>>();

            var mockCommentsService = new Mock<ICommentService>();

            Action commentServiceExceptionCallback = () =>
            {
                if (ShouldThrowExceptionDuringExection)
                {
                    throw new Exception(ExceptionDuringExectionText);
                }
            };

            mockCommentsService
                .Setup(x => x.Add(It.IsAny<AddCommentModel>()))
                .Callback(commentServiceExceptionCallback)
                .Returns(Guid.Empty);

            mockCommentsService
                .Setup(x => x.Get())
                .Callback(commentServiceExceptionCallback)
                .Returns(Enumerable.Empty<CommentModel>());

            mockCommentsService
                .Setup(x => x.GetDescription(It.IsAny<Guid>()))
                .Callback(commentServiceExceptionCallback)
                .Returns(string.Empty);

            mockCommentsService
                .Setup(x => x.Increment(It.IsAny<Guid>()))
                .Callback(commentServiceExceptionCallback);

            mockCommentsService
                .Setup(x => x.Update(It.IsAny<UpdateCommentModel>()))
                .Callback(commentServiceExceptionCallback);

            mockCommentsService
                .Setup(x => x.Delete(It.IsAny<Guid>()))
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
            ExceptionDuringExectionText = "Tested exception message";
            ShouldThrowExceptionDuringExection = true;

            TResult baseServiceResult = action.Invoke();

            Assert.NotNull(baseServiceResult);
            Assert.False(baseServiceResult.IsSuccess);
            Assert.Equal(ExceptionDuringExectionText, baseServiceResult.ErrorMessage);
        }
    }
}
