namespace MAS.GitlabComments.Logic.Tests.SystemVariableActionExecutorTests
{
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.Logic.Services.Implementations;

    using Moq;

    /// <summary>
    /// Base class for <see cref="SystemVariableActionExecutor"/> tests
    /// </summary>
    public abstract class BaseSystemVariableActionExecutorTests
    {
        /// <summary>
        /// Instance of <see cref="SystemVariableActionExecutor"/> for tests
        /// </summary>
        protected SystemVariableActionExecutor TestedService { get; }

        /// <summary>
        /// Flag, which determines that function <see cref="ICommentService.RecalculateLastNumber"/> was called
        /// </summary>
        protected bool IsRecalculationCalled { get; private set; }

        /// <summary>
        /// Initializing <see cref="BaseSystemVariableProviderTests"/> with setup all required environment
        /// </summary>
        public BaseSystemVariableActionExecutorTests()
        {
            var mockDependency = GetMockCommentService();

            TestedService = new SystemVariableActionExecutor(mockDependency);
        }

        /// <summary>
        /// Configure mock object of system variable data provider for system variable service
        /// </summary>
        /// <returns>Configured mock object of <see cref="IDataProvider{TEntity}"/></returns>
        private ICommentService GetMockCommentService()
        {
            var mockDataProvider = new Mock<ICommentService>();

            mockDataProvider
                .Setup(x => x.RecalculateLastNumber())
                .Callback(() => IsRecalculationCalled = true);

            return mockDataProvider.Object;
        }
    }
}
