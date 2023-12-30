namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using MAS.GitlabComments.WebApi.Models;

    using Xunit;

    public sealed class UpdateIncompleteTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultError_WhenExceptionOccurs()
        {
            AssertBaseServiceResultError(() => TestedController.UpdateIncomplete());
        }

        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult result = TestedController.UpdateIncomplete();

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }
    }
}
