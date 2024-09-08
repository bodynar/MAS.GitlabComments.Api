namespace MAS.GitlabComments.WebApi.Tests.RetractionTokenControllerTests
{
    using MAS.GitlabComments.WebApi.Models;

    using Xunit;

    public sealed class RemoveExpiredTests : BaseRetractionTokenControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult result = TestedController.RemoveExpired();

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.RemoveExpired());
        }
    }
}
