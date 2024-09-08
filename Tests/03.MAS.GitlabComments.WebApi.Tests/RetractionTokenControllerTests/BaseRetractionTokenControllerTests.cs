namespace MAS.GitlabComments.WebApi.Tests.RetractionTokenControllerTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.WebApi.Controllers;
    using MAS.GitlabComments.WebApi.Models;

    using Microsoft.Extensions.Logging;

    using Moq;

    using Xunit;

    /// <summary>
    /// Base class for <see cref="RetractionTokenController"/> tests
    /// </summary>
    public class BaseRetractionTokenControllerTests
    {
        /// <summary>
        /// Instance of <see cref="RetractionTokenController"/> for tests
        /// </summary>
        protected RetractionTokenController TestedController { get; }

        /// <summary>
        /// Should tested method throw an exception
        /// </summary>
        protected bool ShouldThrowAnException { get; set; }

        /// <summary>
        /// Return result of call <see cref="IRetractionTokenManager.GetAvailable()"/>
        /// </summary>
        protected IEnumerable<RetractionTokenReadModel> GetAvailableResult { get; }
            = new[] { new RetractionTokenReadModel() { Id = Guid.NewGuid(), CommentId = Guid.NewGuid(), } };

        /// <summary>
        /// Thrown exception message
        /// </summary>
        protected const string ExceptionTestMessage = "ExceptionTestMessage";

        /// <summary>
        /// Initializing <see cref="BaseRetractionTokenControllerTests"/> with setup of all required environment
        /// </summary>
        protected BaseRetractionTokenControllerTests()
        {
            var dependencies = GetDependencies();
            TestedController = new RetractionTokenController(dependencies.Item1, dependencies.Item2);
        }

        #region Private members

        /// <summary>
        /// Configure mock objects of required dependencies for tested class
        /// </summary>
        /// <returns>(Mock of logger, mock of <see cref="IRetractionTokenManager"/>></returns>
        private (ILogger<RetractionTokenController>, IRetractionTokenManager) GetDependencies()
        {
            var mockLogger = new Mock<ILogger<RetractionTokenController>>();
            var mockTokenManager = new Mock<IRetractionTokenManager>();

            mockTokenManager
                .Setup(x => x.Retract(It.IsAny<Guid>()))
                .Callback(() =>
                {
                    if (ShouldThrowAnException)
                    {
                        throw new Exception(ExceptionTestMessage);
                    }
                });

            mockTokenManager
                .Setup(x => x.Retract(It.IsAny<IEnumerable<Guid>>()))
                .Callback(() =>
                {
                    if (ShouldThrowAnException)
                    {
                        throw new Exception(ExceptionTestMessage);
                    }
                });

            mockTokenManager
                .Setup(x => x.GetAvailable())
                .Callback(() =>
                {
                    if (ShouldThrowAnException)
                    {
                        throw new Exception(ExceptionTestMessage);
                    }
                })
                .Returns(GetAvailableResult);

            mockTokenManager
                .Setup(x => x.RemoveExpired())
                .Callback(() =>
                {
                    if (ShouldThrowAnException)
                    {
                        throw new Exception(ExceptionTestMessage);
                    }
                });

            return (mockLogger.Object, mockTokenManager.Object);
        }

        /// <summary>
        /// Testing exception-thrown case of execution
        /// </summary>
        /// <typeparam name="TResult">Result of controller action</typeparam>
        /// <param name="action">Controller action</param>
        protected void AssertBaseServiceResultError<TResult>(Func<TResult> action)
            where TResult : BaseServiceResult
        {
            ShouldThrowAnException = true;

            TResult baseServiceResult = action.Invoke();

            Assert.NotNull(baseServiceResult);
            Assert.False(baseServiceResult.IsSuccess);
            Assert.Equal(ExceptionTestMessage, baseServiceResult.ErrorMessage);
        }

        #endregion
    }
}
