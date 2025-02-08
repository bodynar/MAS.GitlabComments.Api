namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.WebApi.Models;

    using Xunit;

    public sealed class AddTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            AddCommentApiModel model = new();

            BaseServiceResult<NewComment> result = TestedController.Add(model);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.Add(new()));
        }
    }
}
