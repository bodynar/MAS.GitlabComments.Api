namespace MAS.GitlabComments.WebApi.Tests.AppApiControllerTests
{
    using Xunit;

    public sealed class ExecuteActionOnVariableTests : BaseAppApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceError_WhenExceptionOccurs()
        {
            ShouldThrowAnException = true;
            var result = TestedController.ExecuteActionOnVariable(null);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(ExceptionTestMessage, result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceSuccess()
        {
            var result = TestedController.ExecuteActionOnVariable(null);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }
    }
}
