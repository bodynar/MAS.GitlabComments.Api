namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using MAS.GitlabComments.WebApi.Models;

    using Xunit;

    public sealed class UpdateCommentTableTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultError_WhenExceptionOccurs()
        {
            AssertBaseServiceResultError(() => TestedController.UpdateCommentTable());
        }

        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult result = TestedController.UpdateCommentTable();

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }
    }
}
