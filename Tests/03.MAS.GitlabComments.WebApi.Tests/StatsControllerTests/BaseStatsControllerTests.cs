namespace MAS.GitlabComments.WebApi.Tests.StatsControllerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using System;
    using System.Linq;

    using MAS.GitlabComments.WebApi.Controllers;
    using MAS.GitlabComments.WebApi.Models;
    using MAS.GitlabComments.Logic.Services;

    using Moq;

    using Xunit;
    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.Base;

    /// <summary>
    /// Base class for <see cref="CommentsApiController"/> tests
    /// </summary>
    public abstract class BaseStatsControllerTests
    {
        /// <summary>
        /// Instance of <see cref="CommentsApiController"/> for tests
        /// </summary>
        protected StatsApiController TestedController { get; }

        /// <summary>
        /// Should exception be thrown during comment service execution
        /// </summary>
        protected bool ShouldThrowExceptionDuringExecution { get; set; }

        /// <summary>
        /// Exception message which will be thrown if <see cref="ShouldThrowExceptionDuringExecution"/> is true
        /// </summary>
        protected string ExceptionDuringExecutionText { get; set; }

        /// <summary>
        /// Initializing <see cref="BaseStatsControllerTests"/> with setup'n all required environment
        /// </summary>
        protected BaseStatsControllerTests()
        {
            var (logger, commentService) = GetDependencies();
            TestedController = new StatsApiController(logger, commentService);
        }

        #region Private members

        /// <summary>
        /// Configure mock objects of required dependencies for tested class
        /// </summary>
        /// <returns>Pair of configured mock object of <see cref="ILogger{TCategoryName}"/> and <see cref="ICommentService"/></returns>
        private (ILogger, ICommentStoryRecordService) GetDependencies()
        {
            var mockLogger = new Mock<ILogger>();

            var mockService = new Mock<ICommentStoryRecordService>();

            Action commentServiceExceptionCallback = () =>
            {
                if (ShouldThrowExceptionDuringExecution)
                {
                    throw new Exception(ExceptionDuringExecutionText);
                }
            };

            mockService
                .Setup(x => x.Get(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .Callback(commentServiceExceptionCallback)
                .Returns(Enumerable.Empty<StoryRecordViewModel>());

            return (mockLogger.Object, mockService.Object);
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
            ExceptionDuringExecutionText = "Tested exception message";
            ShouldThrowExceptionDuringExecution = true;

            TResult baseServiceResult = action.Invoke();

            Assert.NotNull(baseServiceResult);
            Assert.False(baseServiceResult.IsSuccess);
            Assert.Equal(ExceptionDuringExecutionText, baseServiceResult.ErrorMessage);
        }
    }
}
