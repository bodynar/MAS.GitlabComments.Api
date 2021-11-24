namespace MAS.GitlabComments.Tests.CommentsApiControllerTests
{
    using MAS.GitlabComments.Models;

    using Moq;

    using Xunit;

    public sealed class UpdateTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult result = TestedController.Update(It.IsAny<UpdateCommentModel>());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.Update(It.IsAny<UpdateCommentModel>()));
        }
    }
}
